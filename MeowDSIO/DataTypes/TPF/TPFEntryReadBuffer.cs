using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TPF
{
    public struct TPFEntryReadBuffer
    {
        public uint TpfFlags;

        public int Offset;
        public int Size;
        public uint FlagsA;
        public uint FlagsB;
        public int NameOffset;

        public TPFEntry ReadNext(DSBinaryReader bin)
        {
            Offset = bin.ReadInt32();
            Size = bin.ReadInt32();
            FlagsA = bin.ReadUInt32();

            if (TpfFlags == 0x00020300) //Dark Souls
            {
                NameOffset = bin.ReadInt32();
                FlagsB = bin.ReadUInt32();
            }
            else if (TpfFlags == 0x02010200 || TpfFlags == 0x02010000) //Demon's Souls
            {
                FlagsB = bin.ReadUInt32();
                NameOffset = bin.ReadInt32();
            }

            var newEntry = new TPFEntry()
            {
                FlagsA = FlagsA,
                FlagsB = FlagsB
            };

            bin.StepIn(NameOffset);
            {
                newEntry.Name = bin.ReadStringShiftJIS();
            }
            bin.StepOut();

            bin.StepIn(Offset);
            {
                newEntry.DDSBytes = bin.ReadBytes(Size);
            }
            bin.StepOut();

            return newEntry;
        }
    }
}
