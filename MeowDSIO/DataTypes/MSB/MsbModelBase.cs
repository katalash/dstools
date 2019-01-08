using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST
{
    public abstract class MsbModelBase : MsbStruct
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
                        nameof(PlaceholderModel),
                        nameof(Index),
                        nameof(InstanceCount),
                    };
                }
                return _baseFieldNames;
            }
        }

        public string Name { get; set; } = null;

        internal ModelParamSubtype ModelType => GetSubtypeValue();

        protected abstract ModelParamSubtype GetSubtypeValue();

        public string PlaceholderModel { get; set; } = null;
        public int Index { get; set; } = 0;
        public int InstanceCount { get; set; } = 0;
        internal int BASE_CONST_1 { get; set; } = 0;
        internal int BASE_CONST_2 { get; set; } = 0;
        internal int BASE_CONST_3 { get; set; } = 0;

        protected override void InternalRead(DSBinaryReader bin)
        {
            Name = bin.ReadMsbString();
            bin.AssertInt32((int)ModelType);
            Index = bin.ReadInt32();
            PlaceholderModel = bin.ReadMsbString();
            InstanceCount = bin.ReadInt32();
            BASE_CONST_1 = bin.ReadInt32();
            BASE_CONST_2 = bin.ReadInt32();
            BASE_CONST_3 = bin.ReadInt32();
        }

        protected override void InternalWrite(DSBinaryWriter bin)
        {
            bin.Placeholder($"MODEL_PARAM_ST|0|{nameof(Name)}");
            bin.Write((int)ModelType);
            bin.Write(Index);
            bin.Placeholder($"MODEL_PARAM_ST|0|{nameof(PlaceholderModel)}");
            bin.Write(InstanceCount);
            bin.Write(BASE_CONST_1);
            bin.Write(BASE_CONST_2);
            bin.Write(BASE_CONST_3);

            bin.Replace($"MODEL_PARAM_ST|0|{nameof(Name)}", bin.MsbOffset);
            bin.WriteMsbString(Name, terminate: true);

            bin.Replace($"MODEL_PARAM_ST|0|{nameof(PlaceholderModel)}", bin.MsbOffset);
            bin.WriteMsbString(PlaceholderModel, terminate: true);

            bin.Pad(align: 0x04);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
