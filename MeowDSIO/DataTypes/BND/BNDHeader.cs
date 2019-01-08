using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.BND
{
    public class BNDHeader
    {
        public const int BndVersion_ByteLength = 0x4;
        public BndVersion BndVersion { get; set; } = BndVersion.BND3;

        public const int Signature_ByteLength = 0x08;
        public const string Signature_Default = "07D7R6";
        public string Signature { get; set; }

        public byte Format { get; set; }
        public bool IsBigEndian_Maybe { get; set; }
        public bool IsPS3_Maybe { get; set; }
        public byte UnkFlag01 { get; set; }

        public const int UnknownBytes01_Length = 0x08;
        public byte[] UnknownBytes01 { get; set; }

        public const int BND4_Unknown1_Length = 8;
        public byte[] BND4_Unknown1 { get; set; }

        public const int BND4_Unknown2_Length = 8;
        public byte[] BND4_Unknown2 { get; set; }

        //public const int BND4_Unknown3_Length = 4;
        //public byte[] BND4_Unknown3 { get; set; }

        //public const int BND4_Unknown4_Length =  4;
        //public byte[] BND4_Unknown4 { get; set; }

        public const int BND4_Unknown5_Length = 15;
        public byte[] BND4_Unknown5 { get; set; }

        public bool BND4_IsUnicode = true;

        public ulong BND4_Padding = 0xFFFFFFFF00000040;

        public static readonly byte[] SupportedFormatValues = new byte[]
        {
            //DeS:
            0x00,
            0x0E,
            0x2E,

            //DaS PC:
            0x70,
            0x74,
            0x54,
        };
    }
}
