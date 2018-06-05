using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static MonoSerialPort.SerialPortInput _port0, _port1,_port2,_port3;

        static void Main(string[] args)
        {
            Start2();

            byte[] test = new byte[] { 0x41, 0x54 };
            //_port0.SendMessage(test);
            //_port1.SendMessage(test);
            //_port2.SendMessage(test);
            _port3.SendMessage(test);
            System.Console.WriteLine("Application ready!");
            System.Console.WriteLine("Any key to exit");
            System.Console.ReadKey();
            _port3.Disconnect();
            _port3 = null;
        }

        private static void Start2()
        {
            ///// RC
            //_port0 = new SerialPortLib2.SerialPortInput("/dev/ttyUSB0", 115200, SerialPortLib2.Port.Parity.None, 8, SerialPortLib2.Port.StopBits.One, SerialPortLib2.Port.Handshake.None, true, 100, true);
            //_port0.MessageReceived += _port_MessageReceived;
            //_port0.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            //_port0.Connect();


            //_port1 = new SerialPortLib2.SerialPortInput("/dev/ttyUSB1", 115200, SerialPortLib2.Port.Parity.None, 8, SerialPortLib2.Port.StopBits.One, SerialPortLib2.Port.Handshake.None, true, 100, true);
            //_port1.MessageReceived += _port_MessageReceived;
            //_port1.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            //_port1.Connect();

            //_port2 = new SerialPortLib2.SerialPortInput("/dev/ttyUSB2", 115200, SerialPortLib2.Port.Parity.None, 8, SerialPortLib2.Port.StopBits.One, SerialPortLib2.Port.Handshake.None, true);
            //_port2.MessageReceived += _port_MessageReceived;
            //_port2.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            //_port2.Connect();

            //485
            _port3 = new MonoSerialPort.SerialPortInput("/dev/ttySIM7000AT", 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true, false);
            _port3.MessageReceived += _port_MessageReceived;
            _port3.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            _port3.Connect();
        }

        private static void _port_ConnectionStatusChanged(object sender, MonoSerialPort.ConnectionStatusChangedEventArgs args)
        {
            System.Console.WriteLine("Status:-> {0}", args.Connected);
        }

        static void _port_MessageReceived(object sender, MonoSerialPort.MessageReceivedEventArgs args)
        {
            //string data = ByteArrayToHexString(args.Data);
            System.Console.WriteLine("Reply:-> {0}", System.Text.Encoding.Default.GetString(args.Data));
        }
    }
}
