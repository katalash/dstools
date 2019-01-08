using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public partial class DSBinaryWriter : BinaryWriter
    {

        private byte[] PrepareBytes(byte[] b)
        {
            //Check if the BitConverter is expecting little endian
            if (BitConverter.IsLittleEndian)
            {
                //It's expecting little endian so we must reverse our big endian bytes
                Array.Reverse(b);
            }
            return b;
        }

        private void WritePreparedBytes(byte[] b)
        {
            base.Write(PrepareBytes(b));
        }

        public override void Write(char[] chars)
        {
            if (!BigEndian)
            {
                base.Write(chars);
                return;
            }

            for (int i = 0; i < chars.Length; i++)
            {
                WritePreparedBytes(BitConverter.GetBytes(chars[i]));
            }
        }
        public override void Write(long value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(uint value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(int value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(ushort value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(short value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(decimal value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            int[] chunks = Decimal.GetBits(value);
            byte[] bytes = new byte[16];

            for (int i = 0; i < 16; i += 4)
            {
                byte[] b = BitConverter.GetBytes(chunks[i / 4]);
                for (int j = 0; j < 4; j++)
                {
                    bytes[i + j] = b[j];
                }
            }

            WritePreparedBytes(bytes);
        }
        public override void Write(double value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(float value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }
        public override void Write(char ch)
        {
            if (!BigEndian)
            {
                base.Write(ch);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(ch));
        }
        private new void Write(string value)
        {
            //Make generic string write method unavailable to the outside world.
            //Caller must instead call one of the more specific string write methods.
        }
        public override void Write(ulong value)
        {
            if (!BigEndian)
            {
                base.Write(value);
                return;
            }

            WritePreparedBytes(BitConverter.GetBytes(value));
        }

    }
}
