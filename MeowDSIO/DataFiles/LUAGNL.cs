using MeowDSIO.DataTypes.LUAGNL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class LUAGNL : DataFile
    {
        public List<StringRef> GlobalVariableNames = new List<StringRef>();

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            var offsetList = new List<int>();

            int nextOffset = -1;

            while (bin.Position < bin.Length)
            {
                nextOffset = bin.ReadInt32();

                if (nextOffset > 0)
                    offsetList.Add(nextOffset);
                else if (nextOffset == 0)
                    break;
                else if (nextOffset < 0)
                    throw new Exception($"Unexpected LUAGNL string data offset: {nextOffset}");
            }

            string nextString = null;

            GlobalVariableNames.Clear();

            for (int i = 0; i < offsetList.Count; i++)
            {
                bin.Position = offsetList[i];

                // We MUST read until reaching the terminator byte because From Software left the files very messy 
                // toward the end likely due to some resizing, with bits and pieces of random text scattered everywhere.
                nextString = bin.ReadStringAscii(length: null /* Read until terminator byte of 0. */);

                GlobalVariableNames.Add(nextString);
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            // Move to the offset where the actual strings begin.
            // Each offset is 4 bytes long, then there's a four byte separator value of 00 00 00 00.
            bin.Position = (GlobalVariableNames.Count * 4) + 4;

            var stringOffsets = new List<int>();

            foreach (var name in GlobalVariableNames)
            {
                stringOffsets.Add((int)bin.Position);
                bin.WriteStringAscii(name, terminate: true);
            }

            bin.Position = 0;

            foreach (var offset in stringOffsets)
            {
                bin.Write(offset);
            }

            bin.Write(0x00000000u);

            bin.Position = bin.Length;

            bin.Pad(0x10);
        }
    }
}
