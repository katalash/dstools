using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public abstract class MsbPartsBase : MsbStruct
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
                        nameof(ModelName),
                        nameof(PlaceholderModel),
                        nameof(PosX),
                        nameof(PosY),
                        nameof(PosZ),

                        nameof(RotX),
                        nameof(RotY),
                        nameof(RotZ),

                        nameof(ScaleX),
                        nameof(ScaleY),
                        nameof(ScaleZ),

                        nameof(DrawGroup1),
                        nameof(DrawGroup2),
                        nameof(DrawGroup3),
                        nameof(DrawGroup4),

                        nameof(DispGroup1),
                        nameof(DispGroup2),
                        nameof(DispGroup3),
                        nameof(DispGroup4),

                        nameof(EntityID),

                        nameof(LightID),
                        nameof(FogID),
                        nameof(ScatterID),
                        nameof(LensFlareID),
                        nameof(ShadowID),
                        nameof(DofID),
                        nameof(ToneMapID),
                        nameof(ToneCorrectID),
                        nameof(LanternID),
                        nameof(LodParamID),

                        nameof(IsShadowSrc),
                        nameof(IsShadowDest),
                        nameof(IsShadowOnly),
                        nameof(DrawByReflectCam),
                        nameof(DrawOnlyReflectCam),
                        nameof(IsUseDepthBiasFloat),
                        nameof(DisablePointLightEffect),
                    };
                }
                return _baseFieldNames;
            }
        }

        internal abstract void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict);

        public void DebugPushUnknownFieldReport(out string basetypeName, out string subtypeName, Dictionary<string, object> dict, Dictionary<string, object> dict_Subtype)
        {
            dict.Add(nameof(BASE_CONST_1), BASE_CONST_1);
            dict.Add(nameof(BASE_CONST_2), BASE_CONST_2);
            dict.Add(nameof(BASE_CONST_3), BASE_CONST_3);
            dict.Add(nameof(BASE_CONST_4), BASE_CONST_4);

            DebugPushUnknownFieldReport_Subtype(out string sn, dict_Subtype);
            basetypeName = "PARTS_PARAM_ST";
            subtypeName = sn;
        }

        public string Name { get; set; } = "";
        public int Index { get; set; } = 0;

        internal int i_ModelName { get; set; } = -1;
        public string ModelName { get; set; }  = "";

        public string PlaceholderModel { get; set; } = "";

        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;

        public float RotX { get; set; } = 0;
        public float RotY { get; set; } = 0;
        public float RotZ { get; set; } = 0;

        public float ScaleX { get; set; } = 0;
        public float ScaleY { get; set; } = 0;
        public float ScaleZ { get; set; } = 0;

        public int DrawGroup1 { get; set; } = 0;
        public int DrawGroup2 { get; set; } = 0;
        public int DrawGroup3 { get; set; } = 0;
        public int DrawGroup4 { get; set; } = 0;

        public int DispGroup1 { get; set; } = 0;
        public int DispGroup2 { get; set; } = 0;
        public int DispGroup3 { get; set; } = 0;
        public int DispGroup4 { get; set; } = 0;

        internal int BASE_CONST_1 { get; set; } = 0;

        //BASE DATA

        public int EntityID { get; set; } = -1;
        public sbyte LightID { get; set; } = 0;
        public sbyte FogID { get; set; } = 0;
        public sbyte ScatterID { get; set; } = 0;
        public sbyte LensFlareID { get; set; } = 0;
        public sbyte ShadowID { get; set; } = 0;
        public sbyte DofID { get; set; } = 0;
        public sbyte ToneMapID { get; set; } = 0;
        public sbyte ToneCorrectID { get; set; } = 0;
        public sbyte LanternID { get; set; } = 0;
        public sbyte LodParamID { get; set; } = 0;

        internal byte BASE_CONST_2 { get; set; }

        public bool IsShadowSrc { get; set; } = false;
        public bool IsShadowDest { get; set; } = false;
        public bool IsShadowOnly { get; set; } = false;
        public bool DrawByReflectCam { get; set; } = false;
        public bool DrawOnlyReflectCam { get; set; } = false;
        public bool IsUseDepthBiasFloat { get; set; } = false;
        public bool DisablePointLightEffect { get; set; } = false;

        internal byte BASE_CONST_3 { get; set; }
        internal byte BASE_CONST_4 { get; set; }

        protected abstract void SubtypeRead(DSBinaryReader bin);
        protected abstract void SubtypeWrite(DSBinaryWriter bin);
        public abstract PartsParamSubtype GetSubtypeValue();
        internal PartsParamSubtype Type => GetSubtypeValue();

        protected override void InternalRead(DSBinaryReader bin)
        {
            Name = bin.ReadMsbString();

            bin.AssertInt32((int)Type);

            Index = bin.ReadInt32();
            i_ModelName = bin.ReadInt32();

            PlaceholderModel = bin.ReadMsbString();

            PosX = bin.ReadSingle();
            PosY = bin.ReadSingle();
            PosZ = bin.ReadSingle();

            RotX = bin.ReadSingle();
            RotY = bin.ReadSingle();
            RotZ = bin.ReadSingle();

            ScaleX = bin.ReadSingle();
            ScaleY = bin.ReadSingle();
            ScaleZ = bin.ReadSingle();

            DrawGroup1 = bin.ReadInt32();
            DrawGroup2 = bin.ReadInt32();
            DrawGroup3 = bin.ReadInt32();
            DrawGroup4 = bin.ReadInt32();

            DispGroup1 = bin.ReadInt32();
            DispGroup2 = bin.ReadInt32();
            DispGroup3 = bin.ReadInt32();
            DispGroup4 = bin.ReadInt32();

            int baseDataOffset = bin.ReadInt32();
            int subtypeDataOffset = bin.ReadInt32();

            BASE_CONST_1 = bin.ReadInt32();

            bin.StepInMSB(baseDataOffset);
            {
                EntityID = bin.ReadInt32();
                LightID = bin.ReadSByte();
                FogID = bin.ReadSByte();
                ScatterID = bin.ReadSByte();
                LensFlareID = bin.ReadSByte();
                ShadowID = bin.ReadSByte();
                DofID = bin.ReadSByte();
                ToneMapID = bin.ReadSByte();
                ToneCorrectID = bin.ReadSByte();
                LanternID = bin.ReadSByte();
                LodParamID = bin.ReadSByte();
                BASE_CONST_2 = bin.ReadByte();
                IsShadowSrc = bin.ReadBoolean();
                IsShadowDest = bin.ReadBoolean();
                IsShadowOnly = bin.ReadBoolean();
                DrawByReflectCam = bin.ReadBoolean();
                DrawOnlyReflectCam = bin.ReadBoolean();
                IsUseDepthBiasFloat = bin.ReadBoolean();
                DisablePointLightEffect = bin.ReadBoolean();
                
                BASE_CONST_3 = bin.ReadByte();
                BASE_CONST_4 = bin.ReadByte();

            }
            bin.StepOut();

            bin.StepInMSB(subtypeDataOffset);
            {
                SubtypeRead(bin);
            }
            bin.StepOut();
        }

        protected override void InternalWrite(DSBinaryWriter bin)
        {
            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(Name)}");

            bin.Write((int)Type);

            bin.Write(Index);
            bin.Write(i_ModelName);

            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(PlaceholderModel)}");

            bin.Write(PosX);
            bin.Write(PosY);
            bin.Write(PosZ);

            bin.Write(RotX);
            bin.Write(RotY);
            bin.Write(RotZ);

            bin.Write(ScaleX);
            bin.Write(ScaleY);
            bin.Write(ScaleZ);

            bin.Write(DrawGroup1);
            bin.Write(DrawGroup2);
            bin.Write(DrawGroup3);
            bin.Write(DrawGroup4);

            bin.Write(DispGroup1);
            bin.Write(DispGroup2);
            bin.Write(DispGroup3);
            bin.Write(DispGroup4);

            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|(BASE DATA OFFSET)");
            bin.Placeholder($"PARTS_PARAM_ST|{Type}|{Index}|(SUBTYPE DATA OFFSET)");

            bin.Write(BASE_CONST_1);

            int nameByteCount = DSBinaryWriter.ShiftJISEncoding.GetByteCount(Name);
            int placeholderModelByteCount = DSBinaryWriter.ShiftJISEncoding.GetByteCount(PlaceholderModel);

            int blockSize = (nameByteCount + 1) + (placeholderModelByteCount + 1);

            if (string.IsNullOrEmpty(PlaceholderModel))
            {
                blockSize += 5;
            }

            blockSize = (blockSize + 3) & (-0x4);

            bin.StartMSBStrings();
            {
                bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(Name)}", bin.MsbOffset);
                bin.WriteMsbString(Name, terminate: true);

                bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|{nameof(PlaceholderModel)}", bin.MsbOffset);
                bin.WriteMsbString(PlaceholderModel, terminate: true);
            }
            bin.EndMSBStrings(blockSize);

            bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|(BASE DATA OFFSET)", bin.MsbOffset);

            bin.Write(EntityID);
            bin.Write(LightID);
            bin.Write(FogID);
            bin.Write(ScatterID);
            bin.Write(LensFlareID);
            bin.Write(ShadowID);
            bin.Write(DofID);
            bin.Write(ToneMapID);
            bin.Write(ToneCorrectID);
            bin.Write(LanternID);
            bin.Write(LodParamID);
            bin.Write(BASE_CONST_2);
            bin.Write(IsShadowSrc);
            bin.Write(IsShadowDest);
            bin.Write(IsShadowOnly);
            bin.Write(DrawByReflectCam);
            bin.Write(DrawOnlyReflectCam);
            bin.Write(IsUseDepthBiasFloat);
            bin.Write(DisablePointLightEffect);
            
            bin.Write(BASE_CONST_3);
            bin.Write(BASE_CONST_4);



            bin.Replace($"PARTS_PARAM_ST|{Type}|{Index}|(SUBTYPE DATA OFFSET)", bin.MsbOffset);
            SubtypeWrite(bin);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
