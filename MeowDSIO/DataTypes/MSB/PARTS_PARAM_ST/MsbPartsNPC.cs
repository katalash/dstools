using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST
{
    public class MsbPartsNPC : MsbPartsBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "NPC";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);

            dict.Add(nameof(SubUnk1), SubUnk1);
            dict.Add(nameof(SUB_CONST_3), SUB_CONST_3);
            dict.Add(nameof(SubUnk2), SubUnk2);
            dict.Add(nameof(SubUnk3), SubUnk3);

            dict.Add(nameof(SUB_CONST_4), SUB_CONST_4);
            dict.Add(nameof(SUB_CONST_5), SUB_CONST_5);

            dict.Add(nameof(SubUnk4), SubUnk4);
            dict.Add(nameof(SubUnk5), SubUnk5);
            dict.Add(nameof(SubUnk6), SubUnk6);
            dict.Add(nameof(SubUnk7), SubUnk7);
            dict.Add(nameof(SubUnk8), SubUnk8);
            dict.Add(nameof(SubUnk9), SubUnk9);
            dict.Add(nameof(SubUnk10), SubUnk10);
            dict.Add(nameof(SubUnk11), SubUnk11);
        }

        internal int SUB_CONST_1 { get; set; } = 0;
        internal int SUB_CONST_2 { get; set; } = 0;

        public int ThinkParamID { get; set; } = 0;
        public int NPCParamID { get; set; } = 0;
        public int TalkID { get; set; } = 0;

        public byte SubUnk1 { get; set; } = 0;
        internal byte SUB_CONST_3 { get; set; } = 0;
        public byte SubUnk2 { get; set; } = 0;
        public byte SubUnk3 { get; set; } = 0;

        public int CharaInitID { get; set; } = 0;

        internal int i_HitName { get; set; } = 0;
        public string HitName { get; set; } = MiscUtil.BAD_REF;

        internal int SUB_CONST_4 { get; set; } = 0;
        internal int SUB_CONST_5 { get; set; } = 0;

        internal short SolvedMovePointIndex1 { get; set; } = -1;
        internal short SolvedMovePointIndex2 { get; set; } = -1;
        internal short SolvedMovePointIndex3 { get; set; } = -1;
        internal short SolvedMovePointIndex4 { get; set; } = -1;

        public string MovePoint1 { get; set; } = "";
        public string MovePoint2 { get; set; } = "";
        public string MovePoint3 { get; set; } = "";
        public string MovePoint4 { get; set; } = "";

        public sbyte SubUnk4 { get; set; } = 0;
        public sbyte SubUnk5 { get; set; } = 0;
        public sbyte SubUnk6 { get; set; } = 0;
        public sbyte SubUnk7 { get; set; } = 0;

        public sbyte SubUnk8 { get; set; } = 0;
        public sbyte SubUnk9 { get; set; } = 0;
        public sbyte SubUnk10 { get; set; } = 0;
        public sbyte SubUnk11 { get; set; } = 0;

        public int InitAnimID { get; set; } = 0;

        public int m17_Butterfly_Anim_Unk { get; set; } = 0;



        public override PartsParamSubtype GetSubtypeValue()
        {
            return PartsParamSubtype.NPCs;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SUB_CONST_1 = bin.ReadInt32();
            SUB_CONST_2 = bin.ReadInt32();

            ThinkParamID = bin.ReadInt32();
            NPCParamID = bin.ReadInt32();
            TalkID = bin.ReadInt32();

            SubUnk1 = bin.ReadByte();
            SUB_CONST_3 = bin.ReadByte();
            SubUnk2 = bin.ReadByte();
            SubUnk3 = bin.ReadByte();

            CharaInitID = bin.ReadInt32();
            i_HitName = bin.ReadInt32();

            SUB_CONST_4 = bin.ReadInt32();
            SUB_CONST_5 = bin.ReadInt32();

            SolvedMovePointIndex1 = bin.ReadInt16();
            SolvedMovePointIndex2 = bin.ReadInt16();
            SolvedMovePointIndex3 = bin.ReadInt16();
            SolvedMovePointIndex4 = bin.ReadInt16();

            SubUnk4 = bin.ReadSByte();
            SubUnk5 = bin.ReadSByte();
            SubUnk6 = bin.ReadSByte();
            SubUnk7 = bin.ReadSByte();
            SubUnk8 = bin.ReadSByte();
            SubUnk9 = bin.ReadSByte();
            SubUnk10 = bin.ReadSByte();
            SubUnk11 = bin.ReadSByte();

            InitAnimID = bin.ReadInt32();

            m17_Butterfly_Anim_Unk = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(SUB_CONST_1);
            bin.Write(SUB_CONST_2);

            bin.Write(ThinkParamID);
            bin.Write(NPCParamID);
            bin.Write(TalkID);

            bin.Write(SubUnk1);
            bin.Write(SUB_CONST_3);
            bin.Write(SubUnk2);
            bin.Write(SubUnk3);

            bin.Write(CharaInitID);
            bin.Write(i_HitName);

            bin.Write(SUB_CONST_4);
            bin.Write(SUB_CONST_5);

            bin.Write(SolvedMovePointIndex1);
            bin.Write(SolvedMovePointIndex2);
            bin.Write(SolvedMovePointIndex3);
            bin.Write(SolvedMovePointIndex4);

            bin.Write(SubUnk4);
            bin.Write(SubUnk5);
            bin.Write(SubUnk6);
            bin.Write(SubUnk7);
            bin.Write(SubUnk8);
            bin.Write(SubUnk9);
            bin.Write(SubUnk10);
            bin.Write(SubUnk11);

            bin.Write(InitAnimID);

            bin.Write(m17_Butterfly_Anim_Unk);
        }
    }
}
