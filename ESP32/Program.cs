using MonoSerialPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESP32
{
    class Program
    {
        static SerialPortInput _portESP32;

        static void Main(string[] args)
        {
            System.Console.WriteLine("Application ready!");
            Start();
            Write("AT+GMR\r\n");


            System.Console.WriteLine("Any key to exit");
            System.Console.ReadKey();
            _portESP32.Disconnect();
            _portESP32 = null;
        }

        static bool Write(string data)
        {
            byte[] packetArray = Encoding.ASCII.GetBytes(data);
            return _portESP32.SendMessage(packetArray);
        }

        private static void Start()
        {
            //COM50 /dev/ttyUSB9
            _portESP32 = new MonoSerialPort.SerialPortInput("COM50", 115200, 
                MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, false);
            _portESP32.MessageReceived += _port_MessageReceived;
            _portESP32.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            _portESP32.Connect();
        }

        private static void _port_MessageReceived(object sender, MonoSerialPort.MessageReceivedEventArgs args)
        {
            System.Console.WriteLine("Reply:-> {0}", Encoding.ASCII.GetString(args.Data));
        }
        private static void _port_ConnectionStatusChanged(object sender, MonoSerialPort.ConnectionStatusChangedEventArgs args)
        {
            System.Console.WriteLine("Status:-> {0}", args.Connected);
        }
    }
}
