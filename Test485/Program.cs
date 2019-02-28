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
            Start();
            //System.Console.WriteLine("Address?");
            //var result = System.Console.ReadLine();
            //byte[] reader = System.Text.Encoding.UTF8.GetBytes(result); 
            //byte[] test = new byte[] { 0x41, 0x54 };
            //_port485.SendMessage(test);
            //GetReadeInfo(0);
            //SetPower(0, 0x0);


            //LCDStart(0x01);
            LCDStop(0xFF);
            System.Threading.Thread.Sleep(100);
            ClearScreen(1);
            System.Threading.Thread.Sleep(100);
            PrintScreen(1, 0, "__LOBU ACTIVE___");
            System.Threading.Thread.Sleep(100);
            PrintScreen(1, 1, "___SCAN  TAGS___");
            System.Threading.Thread.Sleep(100);



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
            //_port485 = new MonoSerialPort.SerialPortInput("COM1", 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true);
            _port485 = new MonoSerialPort.SerialPortInput("/dev/ttyUSB3", 115200, MonoSerialPort.Port.Parity.None, 8, MonoSerialPort.Port.StopBits.One, MonoSerialPort.Port.Handshake.None, true);
            _port485.MessageReceived += _port_MessageReceived;
            _port485.ConnectionStatusChanged += _port_ConnectionStatusChanged;
            _port485.Connect();
        }
        private static void _port_ConnectionStatusChanged(object sender, MonoSerialPort.ConnectionStatusChangedEventArgs args)
        {
            System.Console.WriteLine("Status:-> {0}", args.Connected);
        }

        static void _port_MessageReceived(object sender, MonoSerialPort.MessageReceivedEventArgs args)
        {
            System.Console.WriteLine("Reply:-> {0}", BitConverter.ToString(args.Data));
        }

        static byte Delimiter = (byte)'\n';
        static readonly byte Head0 = 0x43;
        static readonly byte Head1 = 0x54;
        static byte[] leftover, completeFrame;
        static void _port_MessageReceived2(object sender, MonoSerialPort.MessageReceivedEventArgs args)
        {
            //string data = ByteArrayToHexString(args.Data);
            //System.Console.WriteLine("Reply:-> {0}", System.Text.Encoding.Default.GetString(args.Data));
            //System.Console.WriteLine("Reply:-> {0}", BitConverter.ToString(args.Data));
            int offset = 0;
            //while (true)
            {
                ////int newlineIndex = Array.IndexOf(args.Data, Delimiter, offset);
                ////if (newlineIndex < offset)
                ////{
                ////    leftover = ConcatArray(leftover, args.Data, offset, args.Data.Length - offset);
                ////    return;
                ////}

                ////++newlineIndex;
                ////byte[] full_line = ConcatArray(leftover, args.Data, offset, newlineIndex - offset);
                ////leftover = null;
                ////offset = newlineIndex;
                //////LineReceived?.Invoke(full_line); // raise an event for further processing
                ////System.Console.WriteLine("Reply:-> {0}", BitConverter.ToString(full_line));


                //concatena array finchè byte 0 e byte 1 non sono uguali a 43 e 54 e la lunghennza non è > 4
                leftover = ConcatArray(leftover, args.Data, offset, args.Data.Length);
                if (leftover.Length < 4)
                    return;
                
                //riposta da client?
                if (leftover[0] == Head0 && leftover[1] == Head1)
                {
                    //calcola lunghezza frame
                    UInt16 length = BitConverter.ToUInt16(new byte[2] { leftover[3], leftover[2] }, 0);
                    if (leftover.Length - 4 >= length)
                    {   //manageframe
                        completeFrame = new byte[length + 4];
                        Array.Copy(leftover, 0, completeFrame, 0, length + 4);
                        //System.Console.WriteLine("manageframe:-> {0}", BitConverter.ToString(completeFrame));
                        leftover = leftover.Skip(length + 4).ToArray();
                        RfidEnetCustomReply reply = new RfidEnetCustomReply(completeFrame);

                        if (reply.Status)
                        {
                            switch (reply.Command)
                            {
                                case 0x80:
                                    //pressed button
                                    Console.WriteLine("pressed button {0}", reply.Payload[0]);
                                    break;
                                case 0x45:
                                    System.Console.WriteLine("TAG {0}", BitConverter.ToString(reply.Payload));
                                    //ISO18000TAG tag = new ISO18000TAG(e.DataStream);
                                    //if (tag.Status == 0x01 && tag.Command == 0x01)
                                    //{
                                    //    _rfidService.EnqueueTag(tag.Payload);
                                    //}
                                    break;
                                case 0x81:
                                    Console.WriteLine("Cleared");
                                    break;
                                case 0x82:
                                    Console.WriteLine("LCD Stopped");
                                    break;
                                case 0x84:
                                    Console.WriteLine("Printed screen");
                                    break;
                                default:
                                    System.Console.WriteLine("to discover:-> {0}", BitConverter.ToString(leftover));
                                    break;
                            }
                        }
                    }
                    else
                        System.Console.WriteLine("reply:-> {0}", BitConverter.ToString(leftover));
                }

                //i 2 byte sono la lunghezza del frame da leggere
                //int newlineIndex = Array.IndexOf(args.Data, new byte[2] { 0x43, 0x54 }, offset);
                //poi legge tanti byte quanti la lunghezza e compone il frame
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

        static void LCDStop(byte reader)
        {
            byte[] len = new byte[2] { 0x0, 0x03 };
            byte cmd = 0x82;
            byte[] request = new byte[7] { pre1, pre2, len[0], len[1], reader, cmd, 0x00 };
            var crc = CheckSum(request);
            request[request.Length - 1] = crc;
            _port485.SendMessage(request);
        }
        static void LCDStart(byte reader)
        {
            byte[] len = new byte[2] { 0x0, 0x03 };
            byte cmd = 0x83;
            byte[] request = new byte[7] { pre1, pre2, len[0], len[1], reader, cmd, 0x00 };
            var crc = CheckSum(request);
            request[request.Length - 1] = crc;
            _port485.SendMessage(request);
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

        static bool PrintScreen(byte reader, byte line, string text)
        {
            byte cmd = 0x84;
            text = EnsureMaximumLength(text, 16);
            byte[] data = new byte[text.Length + 2];
            data[0] = line;
            data[1] = 0x0;
            Array.Copy(Encoding.ASCII.GetBytes(text), 0, data, 2, text.Length);

            int totalRequest = 6 + data.Length + 1;
            UInt16 totalPayload = (UInt16)(totalRequest - 4);
            byte[] len = BitConverter.GetBytes(totalPayload).Reverse().ToArray();
            byte[] request = new byte[totalRequest];

            request[0] = pre1;
            request[1] = pre2;
            request[2] = len[0];
            request[3] = len[1];
            request[4] = reader;
            request[5] = cmd;

            Buffer.BlockCopy(data, 0, request, 6, data.Length);

            var crc = CheckSum(request);
            request[request.Length - 1] = crc;
            return _port485.SendMessage(request);
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

        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }
            else
            {
                return str;
            }
        }






        private static byte[] Crc16Cal(byte[] pucY)
        {
            const int POLYNOMIAL = 0x8408;
            const int PRESET_VALUE = 0xFFFF;
            ushort uiCrcValue = PRESET_VALUE;

            for (int ucI = 0; ucI < pucY.Length; ucI++)
            {
                uiCrcValue = (ushort)(uiCrcValue ^ pucY[ucI]);
                for (int ucJ = 0; ucJ < 8; ucJ++)
                {
                    if ((uiCrcValue & 0x0001) != 0)
                    {
                        uiCrcValue = (ushort)((uiCrcValue >> 1) ^ POLYNOMIAL);
                    }
                    else
                    {
                        uiCrcValue = (ushort)(uiCrcValue >> 1);
                    }
                }
            }

            byte lsb = (byte)(uiCrcValue & 0xFFu);
            byte msb = (byte)((uiCrcValue >> 8) & 0xFFu);
            return new byte[2] { lsb, msb };
        }
        private static void GetReadeInfo()//04 ff 21 19 95
        {
            byte cmd = 0x21;
            byte len = 0x04;
            var crc = Crc16Cal(new byte[] { len, _adrBroadcast, cmd });

            byte[] buffer = new byte[5] { len, _adrBroadcast, cmd, crc[0], crc[1] };

            _port485.SendMessage(buffer);
            //System.Console.WriteLine(BitConverter.ToString(buffer));
        }
    }
}

