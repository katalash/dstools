using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.FMG
{
    public class FMGHeader
    {
        public byte UnkFlag01 { get; set; } = 0x01;
        public byte UnkFlag02 { get; set; } = 0x00;
        public byte UnkFlag03 { get; set; } = 0x01;

        public bool IsBigEndian { get; set; } = false;

        public const byte ENDIAN_FLAG_LITTLE = 0x00;
        public const byte ENDIAN_FLAG_BIG = 0xFF;

        public byte UnkFlag04 { get; set; } = 0x00;
        public byte UnkFlag05 { get; set; } = 0x00;
    }
}
