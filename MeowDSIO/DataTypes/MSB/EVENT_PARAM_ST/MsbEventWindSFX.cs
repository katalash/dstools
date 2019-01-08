using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventWindSFX : MsbEventBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "WindSFX";

            dict.Add(nameof(SubUnk1), SubUnk1);
            dict.Add(nameof(SubUnk2), SubUnk2);
            dict.Add(nameof(SubUnk3), SubUnk3);
            dict.Add(nameof(SubUnk4), SubUnk4);
            dict.Add(nameof(SubUnk5), SubUnk5);
            dict.Add(nameof(SubUnk6), SubUnk6);
            dict.Add(nameof(SubUnk7), SubUnk7);
            dict.Add(nameof(SubUnk8), SubUnk8);
            dict.Add(nameof(SubUnk9), SubUnk9);
            dict.Add(nameof(SubUnk10), SubUnk10);
            dict.Add(nameof(SubUnk11), SubUnk11);
            dict.Add(nameof(SubUnk12), SubUnk12);
            dict.Add(nameof(SubUnk13), SubUnk13);
            dict.Add(nameof(SubUnk14), SubUnk14);
            dict.Add(nameof(SubUnk15), SubUnk15);
            dict.Add(nameof(SubUnk16), SubUnk16);
        }

        public float SubUnk1 { get; set; } = 0;
        public float SubUnk2 { get; set; } = 0;
        public float SubUnk3 { get; set; } = 0;
        public float SubUnk4 { get; set; } = 0;
        public float SubUnk5 { get; set; } = 0;
        public float SubUnk6 { get; set; } = 0;
        public float SubUnk7 { get; set; } = 0;
        public float SubUnk8 { get; set; } = 0;
        public float SubUnk9 { get; set; } = 0;
        public float SubUnk10 { get; set; } = 0;
        public float SubUnk11 { get; set; } = 0;
        public float SubUnk12 { get; set; } = 0;
        public float SubUnk13 { get; set; } = 0;
        public float SubUnk14 { get; set; } = 0;
        public float SubUnk15 { get; set; } = 0;
        public float SubUnk16 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.WindSFX;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SubUnk1 = bin.ReadSingle();
            SubUnk2 = bin.ReadSingle();
            SubUnk3 = bin.ReadSingle();
            SubUnk4 = bin.ReadSingle();
            SubUnk5 = bin.ReadSingle();
            SubUnk6 = bin.ReadSingle();
            SubUnk7 = bin.ReadSingle();
            SubUnk8 = bin.ReadSingle();
            SubUnk9 = bin.ReadSingle();
            SubUnk10 = bin.ReadSingle();
            SubUnk11 = bin.ReadSingle();
            SubUnk12 = bin.ReadSingle();
            SubUnk13 = bin.ReadSingle();
            SubUnk14 = bin.ReadSingle();
            SubUnk15 = bin.ReadSingle();
            SubUnk16 = bin.ReadSingle();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(SubUnk1);
            bin.Write(SubUnk2);
            bin.Write(SubUnk3);
            bin.Write(SubUnk4);
            bin.Write(SubUnk5);
            bin.Write(SubUnk6);
            bin.Write(SubUnk7);
            bin.Write(SubUnk8);
            bin.Write(SubUnk9);
            bin.Write(SubUnk10);
            bin.Write(SubUnk11);
            bin.Write(SubUnk12);
            bin.Write(SubUnk13);
            bin.Write(SubUnk14);
            bin.Write(SubUnk15);
            bin.Write(SubUnk16);
        }
    }
}
