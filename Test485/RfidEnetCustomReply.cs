using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test485
{
    public class RfidEnetCustomReply
    {
        private byte Header1 { get; set; }
        private byte Header2 { get; set; }
        private UInt16 Length { get; set; }
        public byte Address { get; private set; }
        public byte Command { get; private set; }
        public bool Status { get; private set; }
        private bool CrcOk { get; set; }
        public byte[] Payload { get; private set; }

        #region Utils
        private bool CheckSum(byte[] uBuff, byte crc)
        {
            byte uSum = (byte)uBuff.Sum(x => x);
            uSum = (byte)((~uSum) + 1);
            return uSum == crc;
        }
        #endregion

        byte[] _buffer = new byte[8192];
        public RfidEnetCustomReply(byte[] data)
        {
            if (data == null)
                return;

            if (data.Length < 7)
                Array.Copy(data, 0, _buffer, 0, data.Length - 1);
            else
            {
                this.Length = BitConverter.ToUInt16(new byte[2] { data[3], data[2] }, 0);
                byte crc = data[data.Length - 1];
                byte[] dataTocheck = new byte[data.Length - 1];
                Array.Copy(data, 0, dataTocheck, 0, data.Length - 1);
                this.CrcOk = CheckSum(dataTocheck, crc);
                this.Status = false;

                if (this.CrcOk)
                {
                    this.Address = data[4];
                    this.Command = data[5];
                    this.Status = data[6] == 0x01;
                    if (this.Length - 4 > 0)
                    {
                        Payload = new byte[this.Length - 4];
                        Array.Copy(data, 7, Payload, 0, this.Length - 4);
                    }
                }
            }

        }
    }
}
