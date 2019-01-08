using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MTD
{
    public class ExternalMtdParam : Data
    {
        public int UnknownA01 { get; set; }
        public int UnknownA02 { get; set; }
        public int UnknownA03 { get; set; }
        public int UnknownA04 { get; set; }
        public string Name { get; set; }
        public int UnknownB { get; set; }
        public int ShaderDataIndex { get; set; }

        public override string ToString()
        {
            return $"[{ShaderDataIndex:D4}] {Name}";
        }

        public static ExternalMtdParam Read(DSBinaryReader bin)
        {
            var p = new ExternalMtdParam();

            p.UnknownA01 = bin.ReadInt32();
            p.UnknownA02 = bin.ReadInt32();
            p.UnknownA03 = bin.ReadInt32();
            p.UnknownA04 = bin.ReadInt32();


            bin.ReadMtdDelimiter();


            p.Name = bin.ReadMtdName();
            p.UnknownB = bin.ReadInt32();


            bin.ReadMtdDelimiter();


            p.ShaderDataIndex = bin.ReadInt32();

            return p;
        }

        public static void Write(DSBinaryWriter bin, ExternalMtdParam p)
        {

            bin.Write(p.UnknownA01);
            bin.Write(p.UnknownA02);
            bin.Write(p.UnknownA03);
            bin.Write(p.UnknownA04);


            bin.WriteDelimiter(0xA3);


            bin.WriteMtdName(p.Name, 0x35);
            bin.Write(p.UnknownB);


            bin.WriteDelimiter(0x35);


            bin.Write(p.ShaderDataIndex);
        }

    }
}
