using MonoSerialPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test485
{
    class Program
    {
        static readonly byte pre1 = 0x53;
        static readonly byte pre2 = 0x57;
        static SerialPortInput _port485;
        static byte _adrBroadcast = 0xFF;

        static void Main(string[] args)
        {
            ClearScreen(1);
            return;

            Start();
            //System.Console.WriteLine("Address?");
            //var result = System.Console.ReadLine();
            //byte[] reader = System.Text.Encoding.UTF8.GetBytes(result); 
            //byte[] test = new byte[] { 0x41, 0x54 };
            //_port485.SendMessage(test);
            //GetReadeInfo(0);
            //SetPower(0, 0x0);


            ClearScreen(0x01);
            System.Console.WriteLine("Application ready!");
            System.Console.WriteLine("Any key to exit");
            System.Console.ReadKey();
            _port485.Disconnect();
            _port485 = null;
        }
        

        private static void Start()
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
            _port485 = new MonoSerialPort.SerialPortInput("/dev/ttyUSB3", 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true);
            _port485.MessageReceived += _port_MessageReceived;
            _port485.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            _port485.Connect();
        }
        private static void _port_ConnectionStatusChanged(object sender, MonoSerialPort.ConnectionStatusChangedEventArgs args)
        {
            System.Console.WriteLine("Status:-> {0}", args.Connected);
        }

        static byte Delimiter = 0x00;// (byte)'\n';
        static byte[] leftover;
        static void _port_MessageReceived(object sender, MonoSerialPort.MessageReceivedEventArgs args)
        {
            System.Console.WriteLine("Reply:-> {0}", BitConverter.ToString(args.Data));
        }
        static void _port_MessageReceived2(object sender, MonoSerialPort.MessageReceivedEventArgs args)
        {
            //string data = ByteArrayToHexString(args.Data);
            //System.Console.WriteLine("Reply:-> {0}", System.Text.Encoding.Default.GetString(args.Data));
            //System.Console.WriteLine("Reply:-> {0}", BitConverter.ToString(args.Data));


            int offset = 0;
            while (true)
            {
                int newlineIndex = Array.IndexOf(args.Data, Delimiter, offset);
                if (newlineIndex < offset)
                {
                    leftover = ConcatArray(leftover, args.Data, offset, args.Data.Length - offset);
                    return;
                }

                ++newlineIndex;
                byte[] full_line = ConcatArray(leftover, args.Data, offset, newlineIndex - offset);
                leftover = null;
                offset = newlineIndex;
                //LineReceived?.Invoke(full_line); // raise an event for further processing
                System.Console.WriteLine("Reply:-> {0}", BitConverter.ToString(full_line));
            }
        }

        static byte[] ConcatArray(byte[] head, byte[] tail, int tailOffset, int tailCount)
        {
            byte[] result;
            if (head == null)
            {
                result = new byte[tailCount];
                Array.Copy(tail, tailOffset, result, 0, tailCount);
            }
            else
            {
                result = new byte[head.Length + tailCount];
                head.CopyTo(result, 0);
                Array.Copy(tail, tailOffset, result, head.Length, tailCount);
            }

            return result;
        }

        static void GetReadeInfo(byte reader)//04 ff 21 19 95
        {
            byte cmd = 0x21;
            byte len = 0x04;
            var crc = CheckSum(new byte[] { len, reader, cmd });

            byte[] buffer = new byte[4] { len, reader, cmd, crc };

            _port485.SendMessage(buffer);
            //System.Console.WriteLine(CommonHelper.ByteArrayToHexString(buffer));
        }
        static void SetPower(byte reader, byte powerdB) //power = 0-30
        {
            byte cmd = 0x2F;
            byte len = 0x05;
            var crc = CheckSum(new byte[] { len, reader, cmd, powerdB });

            byte[] buffer = new byte[5] { len, reader, cmd, powerdB, crc };
            _port485.SendMessage(buffer);
        }

        static void ClearScreen(byte reader)
        {
            byte[] len = new byte[2] { 0x0, 0x03 };
            byte cmd = 0x81;
            byte[] request = new byte[7] { pre1, pre2, len[0], len[1], reader, cmd, 0x00 };
            var crc = CheckSum(request);
            request[request.Length-1] = crc;
            _port485.SendMessage(request);
        }


        static byte CheckSum(byte[] uBuff)
        {
            //byte i, uSum = 0;
            //for (i = 0; i < uBuff.Length; i++)
            //{
            //    uSum = (byte)(uSum + uBuff[i]);
            //}
            //uSum = (byte)((~uSum) + 1);
            //return uSum;

            byte uSum = (byte)uBuff.Sum(x=>x);
            uSum = (byte)((~uSum) + 1);
            return uSum;
        }

    }
}
