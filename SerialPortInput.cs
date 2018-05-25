using MonoSerialPort.Port;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MonoSerialPort
{
    /// <summary>
    /// Serial port I/O
    /// </summary>
    public class SerialPortInput
    {
        #region Private Fields

        private SerialPort _serialPort;
        private string _portName = "";
        private int _defaultBaudRate = 115200;
        private Parity _defaultParity = Parity.None;
        private int _defaultDataBits = 8;
        private StopBits _defaultStopBits = StopBits.One;
        private bool _isVirtualPort = false;
        private Handshake _handshake = Handshake.None;
        private int _readerTaskTimeWait = 100;        

        // Read/Write error state variable
        //private bool gotReadWriteError = true;

        // Serial port tasks
        private CancellationTokenSource _cancellationTokenSource;
        //private Thread reader;
        //private Thread connectionWatcher;

        //private readonly object accessLock = new object();
        //private bool disconnectRequested = false;
        
        #endregion

        #region Public Events

        /// <summary>
        /// Connected state changed event.
        /// </summary>
        public delegate void ConnectionStatusChangedEventHandler(object sender, ConnectionStatusChangedEventArgs args);
        /// <summary>
        /// Occurs when connected state changed.
        /// </summary>
        public event ConnectionStatusChangedEventHandler ConnectionStatusChanged;

        /// <summary>
        /// Message received event.
        /// </summary>
        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs args);
        /// <summary>
        /// Occurs when message received.
        /// </summary>
        public event MessageReceivedEventHandler MessageReceived;

        #endregion

        #region Public Members

        public SerialPortInput(string portName):this(portName, false)
        {
        }

        public SerialPortInput(string portName, bool isVirtualPort)
            :this(portName, 115200, Parity.None, 8, StopBits.One, Handshake.None, isVirtualPort)
        {
        }

        public SerialPortInput(string portName, 
            int baudRate, 
            Parity parity, 
            int dataBits, 
            StopBits stopBits, 
            Handshake handshake, 
            bool isVirtualPort,
            int readerTaskTime = 100)
        {
            this._isVirtualPort = isVirtualPort;
            this._defaultBaudRate = baudRate;
            this._defaultParity = parity;
            this._defaultDataBits = dataBits;
            this._defaultStopBits = stopBits;
            this._portName = portName;
            this._handshake = handshake;
            this._readerTaskTimeWait = readerTaskTime;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Perform a connection/reconnection to the serial port.
        /// </summary>
        public bool Connect()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                return false;

            Close();
            if (!Open())
                Connect();
            //connectionWatcher = new Thread(new ThreadStart(ConnectionWatcherTask));
            //connectionWatcher.Start();
            //Task.Factory.StartNew(() => ConnectionWatcherTask(_cancellationTokenSource.Token), _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);

            return IsConnected;
        }

        /// <summary>
        /// Disconnect the serial port.
        /// </summary>
        public void Disconnect()
        {
            //if (disconnectRequested)
            //    return;
            //disconnectRequested = true;            
            Close();
            //lock (accessLock)
            //{
            //    if (connectionWatcher != null)
            //    {
            //        if (!connectionWatcher.Join(5000))
            //            connectionWatcher.Abort();
            //        connectionWatcher = null;
            //    }
            //    disconnectRequested = false;
            //}
        }

        /// <summary>
        /// Gets a value indicating whether the serial port is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get { return _serialPort != null && !_cancellationTokenSource.IsCancellationRequested; }
        }

        /// <summary>
        /// Sets the serial port options.        
        /// </summary>
        /// <param name="portname">Portname.</param>
        /// <param name="baudrate">Baudrate.</param>
        public void SetPort(string portname, int baudrate = 115200, Handshake handshake = Handshake.None, int readerTaskTime = 100)
        {
            if (!string.IsNullOrEmpty(_portName) && _portName != portname)
            {
                // Port changed, set to error so that the connection watcher will reconnect
                // using the new port
                //gotReadWriteError = true;
                Connect();
            }
            _portName = portname;
            _defaultBaudRate = baudrate;
            _handshake = handshake;
            _readerTaskTimeWait = readerTaskTime;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <returns><c>true</c>, if message was sent, <c>false</c> otherwise.</returns>
        /// <param name="message">Message.</param>
        public bool SendMessage(byte[] message)
        {
            bool success = false;
            if (IsConnected)
            {
                try
                {
                    _serialPort.Write(message, 0, message.Length);
                    success = true;
                }
                catch (Exception e)
                {
#if DEBUG
                    System.Console.WriteLine(e);
#endif
                }
            }
            return success;
        }


        public static string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }
        #endregion


        #region Serial Port handling

        private bool Open()
        {
            //lock (accessLock)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                try
                {
                    if (!Environment.OSVersion.Platform.ToString().StartsWith("Win") && !System.IO.File.Exists(_portName))
                    {
                        // port does not exist
                        return false;
                    }

                    _serialPort = new SerialPort
                    {
                        IsVirtualPort = _isVirtualPort,
                        PortName = _portName,
                        BaudRate = _defaultBaudRate,
                        Parity = _defaultParity,
                        DataBits = _defaultDataBits,
                        StopBits = _defaultStopBits,
                        Handshake = _handshake
                    };
                    _serialPort.ErrorReceived += HanldeErrorReceived;

                    // We are not using serialPort.DataReceived event for receiving data since this is not working under Linux/Mono.
                    // We use the readerTask instead (see below).
                    _serialPort.Open();

                    //gotReadWriteError = false;
                    // Start the Reader task
                    Task.Factory.StartNew(() => ReaderTask(_cancellationTokenSource.Token), _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
                    //reader = new Thread(ReaderTask);
                    //reader.Start();
                    OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(true));                  
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(e);
#endif
                    //gotReadWriteError = true;
                    Thread.Sleep(1000);
                    return false;
                }
            }
            return true;
        }

        private void Close()
        {
            if (IsConnected)           
            //lock (accessLock)
            {
                // Stop the Reader task
                _cancellationTokenSource.Cancel();
                if (_serialPort != null)
                {
                    _serialPort.ErrorReceived -= HanldeErrorReceived;
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(false));
                    }
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }
        }

        private void HanldeErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //logger.Error(e.EventType);
        }


        public Stream Stream
        {
            get { return this._serialPort.BaseStream; }
        }
        #endregion

        #region Background Tasks

        private async Task ReaderTask(CancellationToken cancellationToken)
        {
            while (IsConnected && !cancellationToken.IsCancellationRequested)
            {
                int msglen = 0;
                try
                {
                    msglen = _serialPort.BytesToRead;
                    if (msglen > 0)
                    {
                        byte[] message = new byte[msglen];
                        //
                        int readbytes = 0;
                        while (_serialPort.Read(message, readbytes, msglen - readbytes) <= 0)
                        {
                            //do nothing to read the whole data
                        }
                        System.Console.WriteLine("Reply:-> {0}", System.Text.Encoding.Default.GetString(message));
                        if (MessageReceived != null)
                        {
                            OnMessageReceived(new MessageReceivedEventArgs(message));
                        }
                    }
                    else
                    {
                        await Task.Delay(_readerTaskTimeWait);
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(e);
#endif
                    Connect();
                    await Task.Delay(1000);
                }
            }
        }

        //private void ConnectionWatcherTask(CancellationToken cancellationToken)
        //{
        //    // This task takes care of automatically reconnecting the interface
        //    // when the connection is drop or if an I/O error occurs
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        if (gotReadWriteError)
        //        {
        //            try
        //            {
        //                Close();
        //                // wait 1 sec before reconnecting
        //                Thread.Sleep(1000);
        //                if (!cancellationToken.IsCancellationRequested)
        //                {
        //                    try
        //                    {
        //                        Open();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Console.WriteLine(e);
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e);
        //            }
        //        }
        //        if (!cancellationToken.IsCancellationRequested)
        //            Thread.Sleep(1000);
        //    }
        //}

        #endregion

        #region Events Raising

        /// <summary>
        /// Raises the connected state changed event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnConnectionStatusChanged(ConnectionStatusChangedEventArgs args)
        {
            ConnectionStatusChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the message received event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
        {
            MessageReceived?.Invoke(this, args);
        }

        #endregion

    }
}
