using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST
{
    public class MsbEventGenerator : MsbEventBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "Generator";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);
            dict.Add(nameof(SUB_CONST_3), SUB_CONST_3);
            dict.Add(nameof(SUB_CONST_4), SUB_CONST_4);
            dict.Add(nameof(SUB_CONST_5), SUB_CONST_5);
            dict.Add(nameof(SUB_CONST_6), SUB_CONST_6);
            dict.Add(nameof(SUB_CONST_7), SUB_CONST_7);

            dict.Add(nameof(SUB_CONST_8), SUB_CONST_8);
            dict.Add(nameof(SUB_CONST_9), SUB_CONST_9);
            dict.Add(nameof(SUB_CONST_10), SUB_CONST_10);
            dict.Add(nameof(SUB_CONST_11), SUB_CONST_11);
            dict.Add(nameof(SUB_CONST_12), SUB_CONST_12);
            dict.Add(nameof(SUB_CONST_13), SUB_CONST_13);
            dict.Add(nameof(SUB_CONST_14), SUB_CONST_14);
            dict.Add(nameof(SUB_CONST_15), SUB_CONST_15);
            dict.Add(nameof(SUB_CONST_16), SUB_CONST_16);
            dict.Add(nameof(SUB_CONST_17), SUB_CONST_17);
            dict.Add(nameof(SUB_CONST_18), SUB_CONST_18);
            dict.Add(nameof(SUB_CONST_19), SUB_CONST_19);
            dict.Add(nameof(SUB_CONST_20), SUB_CONST_20);
            dict.Add(nameof(SUB_CONST_21), SUB_CONST_21);
            dict.Add(nameof(SUB_CONST_22), SUB_CONST_22);
            dict.Add(nameof(SUB_CONST_23), SUB_CONST_23);
        }

        public short MaxNum { get; set; } = 0;
        public short LimitNum { get; set; } = 0;
        public short MinGenNum { get; set; } = 0;
        public short MaxGenNum { get; set; } = 0;
        public float MinInterval { get; set; } = 0;
        public float MaxInterval { get; set; } = 0;
        public int InitialSpawnNum { get; set; } = 0;

        internal int SUB_CONST_1 { get; set; } = 0;
        internal int SUB_CONST_2 { get; set; } = 0;
        internal int SUB_CONST_3 { get; set; } = 0;
        internal int SUB_CONST_4 { get; set; } = 0;
        internal int SUB_CONST_5 { get; set; } = 0;
        internal int SUB_CONST_6 { get; set; } = 0;
        internal int SUB_CONST_7 { get; set; } = 0;

        internal int InternalSpawnPoint1 { get; set; } = -1;
        internal int InternalSpawnPoint2 { get; set; } = -1;
        internal int InternalSpawnPoint3 { get; set; } = -1;
        internal int InternalSpawnPoint4 { get; set; } = -1;
        internal int InternalSpawnPart1 { get; set; } = -1;
        internal int InternalSpawnPart2 { get; set; } = -1;
        internal int InternalSpawnPart3 { get; set; } = -1;
        internal int InternalSpawnPart4 { get; set; } = -1;
        internal int InternalSpawnPart5 { get; set; } = -1;
        internal int InternalSpawnPart6 { get; set; } = -1;
        internal int InternalSpawnPart7 { get; set; } = -1;
        internal int InternalSpawnPart8 { get; set; } = -1;
        internal int InternalSpawnPart9 { get; set; } = -1;
        internal int InternalSpawnPart10 { get; set; } = -1;
        internal int InternalSpawnPart11 { get; set; } = -1;
        internal int InternalSpawnPart12 { get; set; } = -1;
        internal int InternalSpawnPart13 { get; set; } = -1;
        internal int InternalSpawnPart14 { get; set; } = -1;
        internal int InternalSpawnPart15 { get; set; } = -1;
        internal int InternalSpawnPart16 { get; set; } = -1;
        internal int InternalSpawnPart17 { get; set; } = -1;
        internal int InternalSpawnPart18 { get; set; } = -1;
        internal int InternalSpawnPart19 { get; set; } = -1;
        internal int InternalSpawnPart20 { get; set; } = -1;
        internal int InternalSpawnPart21 { get; set; } = -1;
        internal int InternalSpawnPart22 { get; set; } = -1;
        internal int InternalSpawnPart23 { get; set; } = -1;
        internal int InternalSpawnPart24 { get; set; } = -1;
        internal int InternalSpawnPart25 { get; set; } = -1;
        internal int InternalSpawnPart26 { get; set; } = -1;
        internal int InternalSpawnPart27 { get; set; } = -1;
        internal int InternalSpawnPart28 { get; set; } = -1;
        internal int InternalSpawnPart29 { get; set; } = -1;
        internal int InternalSpawnPart30 { get; set; } = -1;
        internal int InternalSpawnPart31 { get; set; } = -1;
        internal int InternalSpawnPart32 { get; set; } = -1;

        public string SpawnPoint1 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPoint2 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPoint3 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPoint4 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart1 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart2 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart3 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart4 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart5 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart6 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart7 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart8 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart9 { get; set; }  = MiscUtil.BAD_REF;
        public string SpawnPart10 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart11 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart12 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart13 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart14 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart15 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart16 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart17 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart18 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart19 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart20 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart21 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart22 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart23 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart24 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart25 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart26 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart27 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart28 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart29 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart30 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart31 { get; set; } = MiscUtil.BAD_REF;
        public string SpawnPart32 { get; set; } = MiscUtil.BAD_REF;

        internal int SUB_CONST_8 { get; set; } = 0;
        internal int SUB_CONST_9 { get; set; } = 0;
        internal int SUB_CONST_10 { get; set; } = 0;
        internal int SUB_CONST_11 { get; set; } = 0;
        internal int SUB_CONST_12 { get; set; } = 0;
        internal int SUB_CONST_13 { get; set; } = 0;
        internal int SUB_CONST_14 { get; set; } = 0;
        internal int SUB_CONST_15 { get; set; } = 0;
        internal int SUB_CONST_16 { get; set; } = 0;
        internal int SUB_CONST_17 { get; set; } = 0;
        internal int SUB_CONST_18 { get; set; } = 0;
        internal int SUB_CONST_19 { get; set; } = 0;
        internal int SUB_CONST_20 { get; set; } = 0;
        internal int SUB_CONST_21 { get; set; } = 0;
        internal int SUB_CONST_22 { get; set; } = 0;
        internal int SUB_CONST_23 { get; set; } = 0;

        protected override EventParamSubtype GetSubtypeValue()
        {
            return EventParamSubtype.Generators;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            MaxNum = bin.ReadInt16();
            LimitNum = bin.ReadInt16();
            MinGenNum = bin.ReadInt16();
            MaxGenNum = bin.ReadInt16();
            MinInterval = bin.ReadSingle();
            MaxInterval = bin.ReadSingle();
            InitialSpawnNum = bin.ReadInt32();
            SUB_CONST_1 = bin.ReadInt32();
            SUB_CONST_2 = bin.ReadInt32();
            SUB_CONST_3 = bin.ReadInt32();
            SUB_CONST_4 = bin.ReadInt32();
            SUB_CONST_5 = bin.ReadInt32();
            SUB_CONST_6 = bin.ReadInt32();
            SUB_CONST_7 = bin.ReadInt32();

            InternalSpawnPoint1  = bin.ReadInt32();
            InternalSpawnPoint2  = bin.ReadInt32();
            InternalSpawnPoint3  = bin.ReadInt32();
            InternalSpawnPoint4  = bin.ReadInt32();
            InternalSpawnPart1   = bin.ReadInt32();
            InternalSpawnPart2   = bin.ReadInt32();
            InternalSpawnPart3   = bin.ReadInt32();
            InternalSpawnPart4   = bin.ReadInt32();
            InternalSpawnPart5   = bin.ReadInt32();
            InternalSpawnPart6   = bin.ReadInt32();
            InternalSpawnPart7   = bin.ReadInt32();
            InternalSpawnPart8   = bin.ReadInt32();
            InternalSpawnPart9   = bin.ReadInt32();
            InternalSpawnPart10  = bin.ReadInt32();
            InternalSpawnPart11  = bin.ReadInt32();
            InternalSpawnPart12  = bin.ReadInt32();
            InternalSpawnPart13  = bin.ReadInt32();
            InternalSpawnPart14  = bin.ReadInt32();
            InternalSpawnPart15  = bin.ReadInt32();
            InternalSpawnPart16  = bin.ReadInt32();
            InternalSpawnPart17  = bin.ReadInt32();
            InternalSpawnPart18  = bin.ReadInt32();
            InternalSpawnPart19  = bin.ReadInt32();
            InternalSpawnPart20  = bin.ReadInt32();
            InternalSpawnPart21  = bin.ReadInt32();
            InternalSpawnPart22  = bin.ReadInt32();
            InternalSpawnPart23  = bin.ReadInt32();
            InternalSpawnPart24  = bin.ReadInt32();
            InternalSpawnPart25  = bin.ReadInt32();
            InternalSpawnPart26  = bin.ReadInt32();
            InternalSpawnPart27  = bin.ReadInt32();
            InternalSpawnPart28  = bin.ReadInt32();
            InternalSpawnPart29  = bin.ReadInt32();
            InternalSpawnPart30  = bin.ReadInt32();
            InternalSpawnPart31  = bin.ReadInt32();
            InternalSpawnPart32 = bin.ReadInt32();

            SUB_CONST_8 = bin.ReadInt32();
            SUB_CONST_9 = bin.ReadInt32();
            SUB_CONST_10 = bin.ReadInt32();
            SUB_CONST_11 = bin.ReadInt32();
            SUB_CONST_12 = bin.ReadInt32();
            SUB_CONST_13 = bin.ReadInt32();
            SUB_CONST_14 = bin.ReadInt32();
            SUB_CONST_15 = bin.ReadInt32();
            SUB_CONST_16 = bin.ReadInt32();
            SUB_CONST_17 = bin.ReadInt32();
            SUB_CONST_18 = bin.ReadInt32();
            SUB_CONST_19 = bin.ReadInt32();
            SUB_CONST_20 = bin.ReadInt32();
            SUB_CONST_21 = bin.ReadInt32();
            SUB_CONST_22 = bin.ReadInt32();
            SUB_CONST_23 = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(MaxNum);
            bin.Write(LimitNum);
            bin.Write(MinGenNum);
            bin.Write(MaxGenNum);
            bin.Write(MinInterval);
            bin.Write(MaxInterval);
            bin.Write(InitialSpawnNum);
            bin.Write(SUB_CONST_1);
            bin.Write(SUB_CONST_2);
            bin.Write(SUB_CONST_3);
            bin.Write(SUB_CONST_4);
            bin.Write(SUB_CONST_5);
            bin.Write(SUB_CONST_6);
            bin.Write(SUB_CONST_7);

            bin.Write(InternalSpawnPoint1);
            bin.Write(InternalSpawnPoint2);
            bin.Write(InternalSpawnPoint3);
            bin.Write(InternalSpawnPoint4);
            bin.Write(InternalSpawnPart1);
            bin.Write(InternalSpawnPart2);
            bin.Write(InternalSpawnPart3);
            bin.Write(InternalSpawnPart4);
            bin.Write(InternalSpawnPart5);
            bin.Write(InternalSpawnPart6);
            bin.Write(InternalSpawnPart7);
            bin.Write(InternalSpawnPart8);
            bin.Write(InternalSpawnPart9);
            bin.Write(InternalSpawnPart10);
            bin.Write(InternalSpawnPart11);
            bin.Write(InternalSpawnPart12);
            bin.Write(InternalSpawnPart13);
            bin.Write(InternalSpawnPart14);
            bin.Write(InternalSpawnPart15);
            bin.Write(InternalSpawnPart16);
            bin.Write(InternalSpawnPart17);
            bin.Write(InternalSpawnPart18);
            bin.Write(InternalSpawnPart19);
            bin.Write(InternalSpawnPart20);
            bin.Write(InternalSpawnPart21);
            bin.Write(InternalSpawnPart22);
            bin.Write(InternalSpawnPart23);
            bin.Write(InternalSpawnPart24);
            bin.Write(InternalSpawnPart25);
            bin.Write(InternalSpawnPart26);
            bin.Write(InternalSpawnPart27);
            bin.Write(InternalSpawnPart28);
            bin.Write(InternalSpawnPart29);
            bin.Write(InternalSpawnPart30);
            bin.Write(InternalSpawnPart31);
            bin.Write(InternalSpawnPart32);

            bin.Write(SUB_CONST_8);
            bin.Write(SUB_CONST_9);
            bin.Write(SUB_CONST_10);
            bin.Write(SUB_CONST_11);
            bin.Write(SUB_CONST_12);
            bin.Write(SUB_CONST_13);
            bin.Write(SUB_CONST_14);
            bin.Write(SUB_CONST_15);
            bin.Write(SUB_CONST_16);
            bin.Write(SUB_CONST_17);
            bin.Write(SUB_CONST_18);
            bin.Write(SUB_CONST_19);
            bin.Write(SUB_CONST_20);
            bin.Write(SUB_CONST_21);
            bin.Write(SUB_CONST_22);
            bin.Write(SUB_CONST_23);
        }
    }
}
