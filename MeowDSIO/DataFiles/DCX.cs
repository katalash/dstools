using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    //Special thanks to TKGP for doing the Read/Write
    public class DCX : DataFile
    {
        public int UnkA { get; set; } = 0x24;
        public int UnkB { get; set; } = 0x24;
        
        //public bool IsDarkSouls3 { get; set; } = false;
        public byte[] Data;

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            bin.BigEndian = true;

            bin.AssertStringAscii("DCX\0", 4);
            bin.AssertInt32(0x10000);
            bin.AssertInt32(0x18);
            //bin.AssertInt32(0x24);
            //bin.AssertInt32(0x24);
            UnkA = bin.ReadInt32();
            UnkB = bin.ReadInt32();
            int headerLength = bin.ReadInt32();
            bin.AssertStringAscii("DCS\0", 4);
            int uncompressedSize = bin.ReadInt32();
            int compressedSize = bin.ReadInt32();
            bin.AssertStringAscii("DCP\0", 4);
            bin.AssertStringAscii("DFLT", 4);
            bin.AssertInt32(0x20);
            bin.AssertInt32(0x9000000);
            bin.AssertInt32(0x0);
            bin.AssertInt32(0x0);
            bin.AssertInt32(0x0);
            // These look suspiciously like flags
            bin.AssertInt32(0x00010100);
            bin.AssertStringAscii("DCA\0", 4);
            int compressedHeaderLength = bin.ReadInt32();
            // Some kind of magic values for zlib
            bin.AssertByte(0x78);
            bin.AssertByte(0xDA);

            // Size includes 78DA
            byte[] compressed = bin.ReadBytes(compressedSize - 2);
            Data = new byte[uncompressedSize];

            using (MemoryStream cmpStream = new MemoryStream(compressed))
            using (DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Decompress))
            using (MemoryStream dcmpStream = new MemoryStream(Data))
                dfltStream.CopyTo(dcmpStream);

        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            bin.BigEndian = true;

            byte[] compressed;
            using (MemoryStream cmpStream = new MemoryStream())
            using (MemoryStream dcmpStream = new MemoryStream(Data))
            {
                // Deflate stream must be closed before the compressed stream can be used,
                // so don't try to streamline this byte[] out again
                // Also, WinForms is the best and WPF is for dweebs
                DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Compress);
                dcmpStream.CopyTo(dfltStream);
                dfltStream.Close();
                compressed = cmpStream.ToArray();
            }

            bin.WriteStringAscii("DCX\0", terminate: false);
            bin.Write(0x10000);
            bin.Write(0x18);
            bin.Write(UnkA);
            bin.Write(UnkB);
            bin.Write(0x2C);
            bin.WriteStringAscii("DCS\0", terminate: false);
            bin.Write(Data.Length);
            // Size includes 78DA
            bin.Write(compressed.Length + 2);
            bin.WriteStringAscii("DCP\0", terminate: false);
            bin.WriteStringAscii("DFLT", terminate: false);
            bin.Write(0x20);
            bin.Write(0x9000000);
            bin.Write(0x0);
            bin.Write(0x0);
            bin.Write(0x0);
            bin.Write(0x00010100);
            bin.WriteStringAscii("DCA\0", terminate: false);
            bin.Write(0x8);
            bin.Write((byte)0x78);
            bin.Write((byte)0xDA);

            bin.Write(compressed);
        }
    }
}
