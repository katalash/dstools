using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventNpcWorldInvitation : MsbEventBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "NpcWorldInvitation";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
        }

        public int NPCHostEntityID { get; set; } = 0;
        public int EventFlagID { get; set; } = 0;
        internal int i_SpawnPoint { get; set; } = 0;
        public string SpawnPoint { get; set; } = MiscUtil.BAD_REF;

        internal int SUB_CONST_1 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.BlackEyeOrbInvasions;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            NPCHostEntityID = bin.ReadInt32();
            EventFlagID = bin.ReadInt32();
            i_SpawnPoint = bin.ReadInt32();
            SUB_CONST_1 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(NPCHostEntityID);
            bin.Write(EventFlagID);
            bin.Write(i_SpawnPoint);
            bin.Write(SUB_CONST_1);
        }
    }
}
