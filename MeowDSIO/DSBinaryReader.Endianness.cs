using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public partial class DSBinaryReader : BinaryReader
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

        private byte[] GetPreparedBytes(int count)
        {
            byte[] b = base.ReadBytes(count);
            if (b.Length != count)
                throw new EndOfStreamException(); //lol microsoft
            return PrepareBytes(b);
        }

        public override char ReadChar()
        {
            if (!BigEndian)
                return base.ReadChar();

            return BitConverter.ToChar(GetPreparedBytes(2), 0);
        }
        public override char[] ReadChars(int count)
        {
            if (!BigEndian)
                return base.ReadChars(count);

            char[] chr = new char[count];

            for (int i = 0; i < count; i++)
            {
                chr[i] = BitConverter.ToChar(GetPreparedBytes(2), 0);
            }

            return chr;
        }
        public override decimal ReadDecimal()
        {
            if (!BigEndian)
                return base.ReadDecimal();

            byte[] b = GetPreparedBytes(16);
            int[] chunks = new int[4];

            for (int i = 0; i < 16; i += 4)
            {
                chunks[i / 4] = BitConverter.ToInt32(b, i);
            }

            return new decimal(chunks);
        }
        public override double ReadDouble()
        {
            if (!BigEndian)
                return base.ReadDouble();

            return BitConverter.ToDouble(GetPreparedBytes(8), 0);
        }
        public override short ReadInt16()
        {
            if (!BigEndian)
                return base.ReadInt16();

            return BitConverter.ToInt16(GetPreparedBytes(2), 0);
        }
        public override int ReadInt32()
        {
            if (!BigEndian)
                return base.ReadInt32();

            return BitConverter.ToInt32(GetPreparedBytes(4), 0);
        }
        public override long ReadInt64()
        {
            if (!BigEndian)
                return base.ReadInt64();

            return BitConverter.ToInt64(GetPreparedBytes(8), 0);
        }
        public override float ReadSingle()
        {
            if (!BigEndian)
                return base.ReadSingle();

            return BitConverter.ToSingle(GetPreparedBytes(4), 0);
        }
        public override ushort ReadUInt16()
        {
            if (!BigEndian)
                return base.ReadUInt16();

            return BitConverter.ToUInt16(GetPreparedBytes(2), 0);
        }
        public override uint ReadUInt32()
        {
            if (!BigEndian)
                return base.ReadUInt32();

            return BitConverter.ToUInt32(GetPreparedBytes(4), 0);
        }
        public override ulong ReadUInt64()
        {
            if (!BigEndian)
                return base.ReadUInt64();

            return BitConverter.ToUInt64(GetPreparedBytes(4), 0);
        }

    }
}
