using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public struct PaddedRegion
    {
        public long StartOffset;
        public long DesiredLength;
        public byte? Padding;
        public PaddedRegion(long StartOffset, long DesiredLength, byte? Padding)
        {
            this.StartOffset = StartOffset;
            this.DesiredLength = DesiredLength;
            this.Padding = Padding;
        }

        public void AdvanceReaderToEnd(DSBinaryReader bin)
        {
            byte nextByte = 0;
            while (bin.Position < (StartOffset + DesiredLength))
            {
                nextByte = bin.ReadByte();
                if (Padding.HasValue && nextByte != Padding.Value)
                    throw new Exceptions.DSReadException(bin, "Read a value that isn't padding in a section of the file expected to be only padding." +
                        $"\nValue Expected (hex): {Padding:X2}\nValue Read (hex): {nextByte:X2}");
            }
        }

        public void AdvanceReaderToEnd(DSBinaryReader bin, out byte foundPadding)
        {
            byte nextByte = 0;

            foundPadding = 0;

            while (bin.Position < (StartOffset + DesiredLength))
            {
                nextByte = bin.ReadByte();
                if (Padding.HasValue && nextByte != Padding.Value)
                    throw new Exceptions.DSReadException(bin, "Read a value that isn't padding in a section of the file expected to be only padding." +
                        $"\nValue Expected (hex): {Padding:X2}\nValue Read (hex): {nextByte:X2}");
                foundPadding = nextByte;
            }
        }

        public void AdvanceWriterToEnd(DSBinaryWriter bin)
        {
            while (bin.Position < (StartOffset + DesiredLength))
            {
                bin.Write(Padding ?? 0);
            }
        }


    }
}
