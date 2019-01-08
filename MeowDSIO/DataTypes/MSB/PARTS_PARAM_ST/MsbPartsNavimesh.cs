using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsNavimesh : MsbPartsBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "Navimesh";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);
            dict.Add(nameof(SUB_CONST_3), SUB_CONST_3);
            dict.Add(nameof(SUB_CONST_4), SUB_CONST_4);
        }

        public int NaviMeshGroup1 { get; set; } = 0;
        public int NaviMeshGroup2 { get; set; } = 0;
        public int NaviMeshGroup3 { get; set; } = 0;
        public int NaviMeshGroup4 { get; set; } = 0;

        internal int SUB_CONST_1 { get; set; } = 0;
        internal int SUB_CONST_2 { get; set; } = 0;
        internal int SUB_CONST_3 { get; set; } = 0;
        internal int SUB_CONST_4 { get; set; } = 0;

        public override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.Navimeshes;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            NaviMeshGroup1 = bin.ReadInt32();
            NaviMeshGroup2 = bin.ReadInt32();
            NaviMeshGroup3 = bin.ReadInt32();
            NaviMeshGroup4 = bin.ReadInt32();

            SUB_CONST_1 = bin.ReadInt32();
            SUB_CONST_2 = bin.ReadInt32();
            SUB_CONST_3 = bin.ReadInt32();
            SUB_CONST_4 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(NaviMeshGroup1);
            bin.Write(NaviMeshGroup2);
            bin.Write(NaviMeshGroup3);
            bin.Write(NaviMeshGroup4);

            bin.Write(SUB_CONST_1);
            bin.Write(SUB_CONST_2);
            bin.Write(SUB_CONST_3);
            bin.Write(SUB_CONST_4);
        }
    }
}
