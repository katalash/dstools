using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsObject : MsbPartsBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "Object";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SubUnk1), SubUnk1);
            dict.Add(nameof(SubUnk2), SubUnk2);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);
            dict.Add(nameof(SUB_CONST_3), SUB_CONST_3);
            dict.Add(nameof(SubUnk3), SubUnk3);
            dict.Add(nameof(SubUnk4), SubUnk4);
            dict.Add(nameof(SubUnk5), SubUnk5);
            dict.Add(nameof(SUB_CONST_4), SUB_CONST_4);
        }

        internal int SUB_CONST_1 { get; set; } = 0;

        internal int i_PartName { get; set; } = 0;
        public string PartName { get; set; } = MiscUtil.BAD_REF;

        public byte SubUnk1 { get; set; } = 0;
        public byte SubUnk2 { get; set; } = 0;
        internal byte SUB_CONST_2 { get; set; } = 0;
        internal byte SUB_CONST_3 { get; set; } = 0;

        public short SubUnk3 { get; set; } = 0;
        public short SubUnk4 { get; set; } = 0;

        public int SubUnk5 { get; set; } = 0;
        internal int SUB_CONST_4 { get; set; } = 0;


        public override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.Objects;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SUB_CONST_1 = bin.ReadInt32();
            i_PartName = bin.ReadInt32();

            SubUnk1 = bin.ReadByte();
            SubUnk2 = bin.ReadByte();
            SUB_CONST_2 = bin.ReadByte();
            SUB_CONST_3 = bin.ReadByte();

            SubUnk3 = bin.ReadInt16();
            SubUnk4 = bin.ReadInt16();

            SubUnk5 = bin.ReadInt32();
            SUB_CONST_4 = bin.ReadInt32();

        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(SUB_CONST_1);
            bin.Write(i_PartName);

            bin.Write(SubUnk1);
            bin.Write(SubUnk2);
            bin.Write(SUB_CONST_2);
            bin.Write(SUB_CONST_3);

            bin.Write(SubUnk3);
            bin.Write(SubUnk4);

            bin.Write(SubUnk5);
            bin.Write(SUB_CONST_4);
        }
    }
}
