using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public abstract class MsbRegionBase : MsbStruct
    {
        private static List<string> _baseFieldNames;
        public static List<string> BaseFieldNames
        {
            get
            {
                if (_baseFieldNames == null)
                {
                    _baseFieldNames = new List<string>
                    {
                        nameof(Name),
                        nameof(Index),

                        nameof(PosX),
                        nameof(PosY),
                        nameof(PosZ),

                        nameof(RotX),
                        nameof(RotY),
                        nameof(RotZ),
                    };
                }
                return _baseFieldNames;
            }
        }

        internal abstract void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict);

        public void DebugPushUnknownFieldReport(out string basetypeName, out string subtypeName, Dictionary<string, object> dict, Dictionary<string, object> dict_Subtype)
        {
            dict.Add(nameof(BASE_CONST_1), BASE_CONST_1);

            DebugPushUnknownFieldReport_Subtype(out string sn, dict_Subtype);
            basetypeName = "POINT_PARAM_ST";
            subtypeName = sn;
        }

        public string Name { get; set; } = "";

        internal int BASE_CONST_1 { get; set; } = 0;

        internal int SolvedIndex { get; set; } = 0;

        public int Index { get; set; } = -1;

        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;

        public float RotX { get; set; } = 0;
        public float RotY { get; set; } = 0;
        public float RotZ { get; set; } = 0;

        internal abstract (int, int, int) GetOffsetDeltas();

        /// <summary>
        /// -1 means that offset field is 0, otherwise its the amount added to the first of the four offsets.
        /// Example: [80,   84,   0,  88] would be (4, -1,  8)
        ///          [88,   92,  96, 100] would be (4,  8, 12)
        ///          [80,   84,  88,  96] would be (4,  8, 16)
        ///          [112, 116, 120, 132] would be (4,  8, 20)
        /// </summary>
        internal (int, int, int) OffsetDeltas => GetOffsetDeltas();
        protected abstract void SubtypeRead(DSBinaryReader bin);
        protected abstract void SubtypeWrite(DSBinaryWriter bin);
        internal abstract PointParamSubtype GetSubtypeValue();
        internal PointParamSubtype Type => GetSubtypeValue();

        protected override void InternalRead(DSBinaryReader bin)
        {
            Name = bin.ReadMsbString();
            BASE_CONST_1 = bin.ReadInt32();
            SolvedIndex = bin.ReadInt32();
            bin.AssertInt32((int)Type);

            PosX = bin.ReadSingle();
            PosY = bin.ReadSingle();
            PosZ = bin.ReadSingle();

            RotX = bin.ReadSingle();
            RotY = bin.ReadSingle();
            RotZ = bin.ReadSingle();

            int baseSubtypeDataOffset = bin.ReadInt32();

            bin.AssertInt32(OffsetDeltas.Item1 >= 0 
                ? baseSubtypeDataOffset + OffsetDeltas.Item1 : 0);
            bin.AssertInt32(OffsetDeltas.Item2 >= 0 
                ? baseSubtypeDataOffset + OffsetDeltas.Item2 : 0);
            bin.AssertInt32(OffsetDeltas.Item3 >= 0 
                ? baseSubtypeDataOffset + OffsetDeltas.Item3 : 0);

            bin.StepInMSB(baseSubtypeDataOffset);
            {
                SubtypeRead(bin);
            }
            bin.StepOut();
        }

        protected override void InternalWrite(DSBinaryWriter bin)
        {
            bin.Placeholder($"POINT_PARAM_ST|{Type}|{nameof(Name)}");
            bin.Write(BASE_CONST_1);
            bin.Write(SolvedIndex);
            bin.Write((int)Type);

            bin.Write(PosX);
            bin.Write(PosY);
            bin.Write(PosZ);

            bin.Write(RotX);
            bin.Write(RotY);
            bin.Write(RotZ);

            bin.Placeholder($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 1)");
            bin.Placeholder($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 2)");
            bin.Placeholder($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 3)");
            bin.Placeholder($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 4)");

            //PADDING?
            bin.Write((int)0);

            //bin.StartMSBStrings();
            {
                bin.Replace($"POINT_PARAM_ST|{Type}|{nameof(Name)}", bin.MsbOffset);
                bin.WriteMsbString(Name, terminate: true);

                bin.Pad(align: 0x04);
            }
            //bin.EndMSBStrings(blockSize: 0x10);

            var msbOffset = bin.MsbOffset;

            bin.Replace($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 1)",
                msbOffset);

            bin.Replace($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 2)", 
                OffsetDeltas.Item1 >= 0 ? msbOffset + OffsetDeltas.Item1 : 0);

            bin.Replace($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 3)", 
                OffsetDeltas.Item2 >= 0 ? msbOffset + OffsetDeltas.Item2 : 0);

            bin.Replace($"POINT_PARAM_ST|{Type}|(SUBTYPE DATA OFFSET 4)", 
                OffsetDeltas.Item3 >= 0 ? msbOffset + OffsetDeltas.Item3 : 0);

            SubtypeWrite(bin);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
