using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FINS_CSharp
{
    public class FINS
    {
        public List<byte> command;

        public enum MemoryType
        {
            D,
            E
        }

        static void BytesFromShort(short number, out byte byte1, out byte byte2)
        {
            byte2 = (byte)(number >> 8);
            byte1 = (byte)(number & 255);
        }

        private Client client;
        public static string remoteIp;
        public static byte remoteIpLast;

        public static string localIp;
        public static byte localIpLast;

        public static void TalksTo(string remoteIp)
        {
            FINS.remoteIp = remoteIp;
            FINS.remoteIpLast = byte.Parse(remoteIp.Split('.')[3].ToString());
        }

        public static void IsAt(string localIp)
        {
            FINS.localIpLast = byte.Parse(localIp.Split('.')[3].ToString());
        }

        public FINS(Client c)
        {
            this.client = c;
            command = new List<byte>();
        }

        public FINS()
        {
            command = new List<byte>();
        }

        public FINS Create()
        {
            command.Add(0x80);
            command.Add(0x00);              // ICF RSV 
            command.Add(0x02);
            command.Add(0x00);              // GCT DNA
            command.Add(FINS.remoteIpLast);
            command.Add(0x00);              // DA1 DA2            
            command.Add(0x00);
            command.Add(FINS.localIpLast);  // SNA SN1
            command.Add(0x00);
            command.Add(0x01);              // SN2 SID
            return this;
        }

        private FINS Zone(FINS.MemoryType memType)
        {
            switch (memType)
            {
                case MemoryType.D:
                    command.Add(0x82);
                    break;
                case MemoryType.E:
                    command.Add(0x90);
                    break;
            }

            return this;
        }

        public FINS Read(FINS.MemoryType memType)
        {
            command.Add(0x01);
            command.Add(0x01);
            return Zone(memType);
        }

        public FINS From(short address)
        {
            byte b1, b2;
            BytesFromShort(address, out b1, out b2);
            command.Add(b2);
            command.Add(b1);
            command.Add(0x00);

            return this;
        }

        public FINS Limit(short number)
        {
            byte b1, b2;
            BytesFromShort(number, out b1, out b2);
            command.Add(b2);
            command.Add(b1);
            return this;
        }
       
        public FINS WriteTo(FINS.MemoryType memType)
        {
            command.Add(0x01);
            command.Add(0x02);
            return Zone(memType);
        }

        public FINS Data(int[] i)
        {
            byte[] result = new byte[i.Length * sizeof(int)];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);
            return this.Data(result);            
        }

        public FINS Data(float[] i)
        {
            byte[] result = new byte[i.Length * sizeof(float)];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);
            return this.Data(result);
        }

        public FINS Data(char[] i)
        {
            byte[] result = new byte[i.Length * sizeof(char)];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);
            return this.Data(result);
        }

        public FINS Data(string i)
        {
            byte[] result = new byte[i.Length * sizeof(char)];
            Buffer.BlockCopy(i.ToCharArray(), 0, result, 0, result.Length);
            return this.Data(result);
        }

        public FINS Data(byte[] b)
        {
            byte b1, b2;
            BytesFromShort((short) (b.Length / 2), out b1, out b2);
            command.Add(b2);
            command.Add(b1);
            command.AddRange(b);
            return this;
        }

        public byte[] Execute()
        {
            return command.ToArray();
        }

        public void Finish()
        {
            this.client.Send(this.Execute());
        }
    }
}