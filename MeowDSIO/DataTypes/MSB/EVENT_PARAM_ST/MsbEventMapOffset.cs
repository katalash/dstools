using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventMapOffset : MsbEventBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "MapOffset";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
        }

        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Z { get; set; } = 0;

        internal float SUB_CONST_1 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.MapOffset;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            X = bin.ReadSingle();
            Y = bin.ReadSingle();
            Z = bin.ReadSingle();
            SUB_CONST_1 = bin.ReadSingle();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(X);
            bin.Write(Y);
            bin.Write(Z);
            bin.Write(SUB_CONST_1);
        }
    }
}
