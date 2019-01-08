using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.FMG
{
    public struct FMGChunkHeaderBuffer
    {
        public int StringOffsetsBeginOffset;

        public int FirstStringIndex;
        public int FirstStringID;
        public int LastStringID;

        private int count;
        private int[] buffer;

        public FMGChunkHeaderBuffer(int stringOffsetsBeginOffset)
        {
            StringOffsetsBeginOffset = stringOffsetsBeginOffset;
            FirstStringIndex = -1;
            FirstStringID = -1;
            LastStringID = -1;

            count = 0;
            buffer = new int[64];
        }

        public void ReadEntries(DSBinaryReader bin, Dictionary<int, string> _entries)
        {
            count = (LastStringID - FirstStringID) + 1;

            if (count > buffer.Length)
            {
                Array.Resize(ref buffer, count * 2 /*Extra "wiggle room"*/);
            }

            bin.StepIn(StringOffsetsBeginOffset + (FirstStringIndex * 4));
            {
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = bin.ReadInt32();
                }

                for (int i = 0; i < count; i++)
                {
                    string stringContents = null;

                    if (buffer[i] == 0)
                    {
                        stringContents = DataFiles.FMG.NullString;
                    }
                    else
                    {
                        bin.Position = buffer[i];
                        stringContents = bin.ReadStringUnicode(length: null);

                        if (string.IsNullOrWhiteSpace(stringContents.Trim()))
                            stringContents = DataFiles.FMG.EmptyString;
                    }

                    if (stringContents == null)
                    {
                        throw new Exception(":trashcat:");
                    }

                    _entries.Add(FirstStringID + i, stringContents);
                }
            }
            bin.StepOut();
        }
    }
}
