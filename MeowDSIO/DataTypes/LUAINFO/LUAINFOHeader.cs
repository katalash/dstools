using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.LUAINFO
{
    public class LUAINFOHeader
    {
        public const int Signature_Length = 4;
        //"LUAI"
        public static readonly byte[] Signature_Default = new byte[4] { 0x4C, 0x55, 0x41, 0x49 };
        public byte[] Signature { get; set; } = new byte[Signature_Length];


        public const int UnknownA_Length = 4;
        //Seems to always be 01 00 00 00
        public byte[] UnknownA = new byte[UnknownA_Length];


        public const int UnknownB_Length = 4;
        //Seems to always be 00 00 00 00
        public byte[] UnknownB = new byte[UnknownB_Length];
    }
}
