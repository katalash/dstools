using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventEnvironment : MsbEventBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "Environment";

            dict.Add(nameof(SubUnk1), SubUnk1);
            dict.Add(nameof(SubUnk2), SubUnk2);
            dict.Add(nameof(SubUnk3), SubUnk3);
            dict.Add(nameof(SubUnk4), SubUnk4);
            dict.Add(nameof(SubUnk5), SubUnk5);
            dict.Add(nameof(SubUnk6), SubUnk6);
            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);
        }

        public int SubUnk1 { get; set; } = 0;
        public float SubUnk2 { get; set; } = 0;
        public float SubUnk3 { get; set; } = 0;
        public float SubUnk4 { get; set; } = 0;
        public float SubUnk5 { get; set; } = 0;
        public float SubUnk6 { get; set; } = 0;
        internal float SUB_CONST_1 { get; set; } = 0;
        internal float SUB_CONST_2 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.Environment;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SubUnk1 = bin.ReadInt32();
            SubUnk2 = bin.ReadSingle();
            SubUnk3 = bin.ReadSingle();
            SubUnk4 = bin.ReadSingle();
            SubUnk5 = bin.ReadSingle();
            SubUnk6 = bin.ReadSingle();
            SUB_CONST_1 = bin.ReadSingle();
            SUB_CONST_2 = bin.ReadSingle();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(SubUnk1);
            bin.Write(SubUnk2);
            bin.Write(SubUnk3);
            bin.Write(SubUnk4);
            bin.Write(SubUnk5);
            bin.Write(SubUnk6);
            bin.Write(SUB_CONST_1);
            bin.Write(SUB_CONST_2);
        }
    }
}
