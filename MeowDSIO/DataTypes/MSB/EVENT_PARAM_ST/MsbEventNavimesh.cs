using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventNavimesh : MsbEventBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "Navimesh";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);
            dict.Add(nameof(SUB_CONST_3), SUB_CONST_3);
        }

        public int i_NvmRegion { get; set; } = 0;
        public string NvmRegion { get; set; } = MiscUtil.BAD_REF;

        internal int SUB_CONST_1 { get; set; } = 0;
        internal int SUB_CONST_2 { get; set; } = 0;
        internal int SUB_CONST_3 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.Navimesh;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            i_NvmRegion = bin.ReadInt32();
            SUB_CONST_1 = bin.ReadInt32();
            SUB_CONST_2 = bin.ReadInt32();
            SUB_CONST_3 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(i_NvmRegion);
            bin.Write(SUB_CONST_1);
            bin.Write(SUB_CONST_2);
            bin.Write(SUB_CONST_3);
        }
    }
}
