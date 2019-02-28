using MonoSerialPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static SerialPortInput _port0, _port1;
        static byte byte_CarriageReturn = (byte)'\r';
        static byte byte_LineFeed = (byte)'\n';
        static string port1 = "/dev/ttySIM7000GNSS";
        static string port0 = "/dev/ttySIM7000AT";
        static string portRC = "/dev/COM1";

        static void Main(string[] args)
        {
            Start2();
            System.Console.WriteLine("Application ready!");
            System.Console.WriteLine("Press Q to exit");
            bool wait = true;       
            while (wait)
            { 
                var command = System.Console.ReadLine();
                System.Console.WriteLine();
                if (command == "exit")
                {
                    _port0.Disconnect();
                    _port0 = null;
                    wait = false;
                }
                else
                    Send(command);
                //var command = System.Console.ReadKey();
                //System.Console.WriteLine();
                //switch(command.Key)
                //{
                //    case ConsoleKey.A:
                //        Send("AT");
                //        break;
                //    case ConsoleKey.B:
                //        break;
                //    case ConsoleKey.C:
                //        Send("AT+CGREG?");
                //        Send("AT+CSQ");
                //        break;
                //    case ConsoleKey.G:
                //        Send("AT+CGATT?");
                //        break;
                //    case ConsoleKey.R:
                //        Send("AT+CSQ");
                //        break;
                //    case ConsoleKey.J:
                //        Send("AT+CGNSPWR=1");
                //        break;
                //    case ConsoleKey.K:
                //        Send("AT+CGNSPWR=0");
                //        break;
                //    case ConsoleKey.Q:
                //        _port0.Disconnect();
                //        _port0 = null;
                //        //_port1.Disconnect();
                //        //_port1 = null;
                //        wait = false;
                //        break;
                //}
            }
        }

        private static void Start2()
        {
            /// RC
            _port0 = new MonoSerialPort.SerialPortInput(portRC, 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true);
            _port0.MessageReceived += _port_MessageReceived0;
            _port0.ConnectionStatusChanged += _port_ConnectionStatusChanged0;
            _port0.Connect();


            //_port1 = new SerialPortLib2.SerialPortInput("/dev/ttyUSB1", 115200, SerialPortLib2.Port.Parity.None, 8, SerialPortLib2.Port.StopBits.One, SerialPortLib2.Port.Handshake.None, true, 100, true);
            //_port1.MessageReceived += _port_MessageReceived;
            //_port1.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            //_port1.Connect();

            //_port2 = new SerialPortLib2.SerialPortInput("/dev/ttyUSB2", 115200, SerialPortLib2.Port.Parity.None, 8, SerialPortLib2.Port.StopBits.One, SerialPortLib2.Port.Handshake.None, true);
            //_port2.MessageReceived += _port_MessageReceived;
            //_port2.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            //_port2.Connect();

            ////485
            //_port3 = new MonoSerialPort.SerialPortInput("/dev/ttySIM7000AT", 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true, false);
            //_port3.MessageReceived += _port_MessageReceived;
            //_port3.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            //_port3.Connect();

            //_port0 = new MonoSerialPort.SerialPortInput(port0, 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true, false);
            //_port0.MessageReceived += _port_MessageReceived0;
            //_port0.ConnectionStatusChanged += _port_ConnectionStatusChanged0;
            ////if (!_port0.Connect())
            //_port0.Connect();
            ////Console.WriteLine("port {0} does not exists", port);

            //_port1 = new MonoSerialPort.SerialPortInput(port1, 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true, false);
            //_port1.MessageReceived += _port_MessageReceived1;
            //_port1.ConnectionStatusChanged += _port_ConnectionStatusChanged1;
            ////if (!_port0.Connect())
            //_port1.Connect();
        }

        private static void _port_ConnectionStatusChanged0(object sender, MonoSerialPort.ConnectionStatusChangedEventArgs args)
        {
            //var port = (sender as MonoSerialPort.SerialPortInput);
            System.Console.WriteLine("{1} Status:-> {0}", args.Connected, portRC);
        }


        private static void _port_ConnectionStatusChanged1(object sender, MonoSerialPort.ConnectionStatusChangedEventArgs args)
        {
            System.Console.WriteLine("{1} Status:-> {0}", args.Connected, port1);
        }
        static void _port_MessageReceived0(object sender, MessageReceivedEventArgs args)
        {
            //string data = ByteArrayToHexString(args.Data);
            System.Console.WriteLine("{1} Reply:-> {0}", System.Text.Encoding.Default.GetString(args.Data), portRC);
        }

        static void _port_MessageReceived1(object sender, MessageReceivedEventArgs args)
        {
            //string data = ByteArrayToHexString(args.Data);
            System.Console.WriteLine("{1} Reply:-> {0}", System.Text.Encoding.Default.GetString(args.Data), port1);
        }

        static void Send(string data)
        {
            byte[] arr = Encoding.ASCII.GetBytes(data);
            byte[] packetArray = new byte[arr.Length + 2];
            arr.CopyTo(packetArray, 0);
            packetArray[arr.Length] = byte_CarriageReturn;
            packetArray[arr.Length+1] = byte_LineFeed;
            _port0.SendMessage(packetArray);
        }
    }
}
