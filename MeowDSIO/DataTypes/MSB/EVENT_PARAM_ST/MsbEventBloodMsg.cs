using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventBloodMsg : MsbEventBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "Messages";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SubUnk1), SubUnk1);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);
        }

        public short MsgID { get; set; } = 0;

        internal short SUB_CONST_1 { get; set; } = 2;

        public short SubUnk1 { get; set; } = 0;

        internal short SUB_CONST_2 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.BloodMsg;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            MsgID = bin.ReadInt16();
            SUB_CONST_1 = bin.ReadInt16();
            SubUnk1 = bin.ReadInt16();
            SUB_CONST_2 = bin.ReadInt16();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(MsgID);
            bin.Write(SUB_CONST_1);
            bin.Write(SubUnk1);
            bin.Write(SUB_CONST_2);
        }
    }
}
