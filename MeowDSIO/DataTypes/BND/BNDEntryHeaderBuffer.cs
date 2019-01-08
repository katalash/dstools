using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.BND
{
    public struct BNDEntryHeaderBuffer
    {
        public byte UnkFlag1;
        public bool IsCompressed;
        public int CompressedFileSize;
        public int FileOffset;
        public int FileID;
        public int FileNameOffset;
        public int UncompressedFileSize;

        public void Reset()
        {
            UnkFlag1 = 0x40;
            IsCompressed = false;
            CompressedFileSize = -1;
            FileOffset = -1;
            FileID = -1;
            FileNameOffset = -1;
            UncompressedFileSize = -1;
        }

        public BNDEntry GetEntry(DSBinaryReader bin)
        {
            if (FileOffset < 0 || FileOffset > bin.Length)
            {
                throw new Exception("Invalid BND3 Entry File Offset.");
            }

            bin.StepIn(FileOffset);
            var bytes = bin.ReadBytes(CompressedFileSize);
            bin.StepOut();

            string fileName = null;

            if (FileNameOffset > -1)
            {
                bin.StepIn(FileNameOffset);
                {
                    fileName = bin.ReadStringShiftJIS();
                }
                bin.StepOut();
            }

            return new BNDEntry(FileID, fileName, bytes)
            {
                IsCompressed = this.IsCompressed,
                UnkFlag1 = this.UnkFlag1
            };
        }
    }
}
