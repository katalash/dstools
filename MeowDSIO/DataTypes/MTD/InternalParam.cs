using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MTD
{
    public class InternalMtdParam
    {
        public int UnknownA01 { get; set; }
        public int UnknownA02 { get; set; }
        public int UnknownA03 { get; set; }
        public int UnknownA04 { get; set; }
        public string Name { get; set; }

        public const int UnknownB_Length = 8;
        public byte[] UnknownB { get; set; }

        private object __val;
        public object Value
        {
            get => __val;
            set
            {
                var newValType = value.GetType();
                if (!DataFiles.MTD.ParamTypes.Contains(newValType))
                    throw new ArgumentException("InternalParam's Value is not a valid value type for an MTD InternalParam's value.");
                __val = value;
                ValueType = newValType;
            }
        }

        public Type ValueType { get; private set; }

        public const int UnknownC_Length = 0x10;
        public byte[] UnknownC { get; set; }

        public const int UnknownD_Length = 0x4;
        public byte[] UnknownD { get; set; }

        public override string ToString()
        {
            return $"{DataFiles.MTD.ParamNamesByType[ValueType]} {Name} = {Value.ToString()}";
        }


        public static InternalMtdParam Read(DSBinaryReader bin)
        {
            InternalMtdParam p = new InternalMtdParam();

            p.UnknownA01 = bin.ReadInt32();
            p.UnknownA02 = bin.ReadInt32();
            p.UnknownA03 = bin.ReadInt32();
            p.UnknownA04 = bin.ReadInt32();


            bin.ReadMtdDelimiter();


            p.Name = bin.ReadMtdName();
            var valueTypeString = bin.ReadMtdName();
            Type valueType = DataFiles.MTD.ParamTypesByName[valueTypeString];

            p.UnknownB = bin.ReadBytes(UnknownB_Length);
            bin.ReadInt32(); //int DataSize;
            p.UnknownC = bin.ReadBytes(UnknownC_Length);

            if (valueType == typeof(int))
                p.Value = bin.ReadInt32();
            else if (valueType == typeof(bool))
                p.Value = bin.ReadBoolean();
            else if (valueType == typeof(float))
                p.Value = bin.ReadSingle();
            else if (valueType == typeof(Vector2))
                p.Value = bin.ReadVector2();
            else if (valueType == typeof(Vector3))
                p.Value = bin.ReadVector3();
            else if (valueType == typeof(Vector4))
                p.Value = bin.ReadVector4();

            bin.ReadMtdDelimiter();

            p.UnknownD = bin.ReadBytes(UnknownD_Length);

            return p;
        }

        public static void Write(DSBinaryWriter bin, InternalMtdParam p)
        {
            bin.Write(p.UnknownA01);
            bin.Write(p.UnknownA02);
            bin.Write(p.UnknownA03);
            bin.Write(p.UnknownA04);


            bin.WriteDelimiter(0xA3);


            bin.WriteMtdName(p.Name, 0xA3);
            bin.WriteMtdName(DataFiles.MTD.ParamNamesByType[p.ValueType], 0x4);


            bin.Write(p.UnknownB, UnknownB_Length);

            int valueSize = DataFiles.MTD.ParamSizesByType[p.ValueType];
            bin.Write(valueSize + UnknownC_Length);

            bin.Write(p.UnknownC, UnknownC_Length);

            if (p.ValueType == typeof(int))
                bin.Write((int)p.Value);
            else if (p.ValueType == typeof(bool))
                bin.Write((bool)p.Value);
            else if (p.ValueType == typeof(float))
                bin.Write((float)p.Value);
            else if (p.ValueType == typeof(Vector2))
                bin.Write((Vector2)p.Value);
            else if (p.ValueType == typeof(Vector3))
                bin.Write((Vector3)p.Value);
            else if (p.ValueType == typeof(Vector4))
                bin.Write((Vector4)p.Value);


            bin.WriteDelimiter(4);


            bin.Write(p.UnknownD, UnknownD_Length);
        }
    }
}
