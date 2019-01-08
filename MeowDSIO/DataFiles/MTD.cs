using MeowDSIO.DataTypes.MTD;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class MTD : DataFile
    {
        public int UnknownA { get; set; } = 0;
        public int UnknownB { get; set; } = 0;
        public int UnknownC { get; set; } = 0;
        public int UnknownD01 { get; set; } = 0;
        public int UnknownD02 { get; set; } = 0;
        public int UnknownD03 { get; set; } = 0;
        public int UnknownD04 { get; set; } = 0;
        public int UnknownD05 { get; set; } = 0;
        public int UnknownD06 { get; set; } = 0;
        public int UnknownD07 { get; set; } = 0;

        public const int Signature_Length = 4;
        public static readonly byte[] Signature_Default = new byte[4] { 0x4D, 0x54, 0x44, 0x20 };
        byte[] Signature { get; set; } = new byte[Signature_Length];

        public int UnknownE01 { get; set; } = 0;
        public int UnknownE02 { get; set; } = 0;
        public int UnknownE03 { get; set; } = 0;
        public int UnknownE04 { get; set; } = 0;

        public int UnknownF01 { get; set; } = 0;
        public int UnknownF02 { get; set; } = 0;
        public int UnknownF03 { get; set; } = 0;

        public string SourceSpxName { get; set; } = null;
        public string Description { get; set; } = null;

        public const int UnknownG_Length = 32;
        public byte[] UnknownG { get; set; } = new byte[UnknownG_Length];

        public List<InternalMtdParam> InternalParams { get; set; } = new List<InternalMtdParam>();

        public const int UnknownH_Length = 4;
        public byte[] UnknownH { get; set; } = new byte[UnknownH_Length];

        public List<ExternalMtdParam> ExternalParams { get; set; } = new List<ExternalMtdParam>();

        public int UnknownI01 { get; set; } = 0;
        public int UnknownI02 { get; set; } = 0;
        public int UnknownI03 { get; set; } = 0;
        public int UnknownI04 { get; set; } = 0;
        public int UnknownI05 { get; set; } = 0;
        public int UnknownI06 { get; set; } = 0;

        public object this[string internalParamName]
        {
            get
            {
                var match = InternalParams.Where(x => x.Name == internalParamName);
                if (!match.Any())
                    return null;

                return match.First().Value;
            }
            set
            {
                var match = InternalParams.Where(x => x.Name == internalParamName);
                if (!match.Any())
                    throw new ArgumentException($"Parameter '{internalParamName}' does not exist within this MTD.", nameof(internalParamName));

                match.First().Value = value;
            }
        }

        public bool HasParam(string internalParamName)
        {
            return InternalParams.Any(x => x.Name == internalParamName);
        }

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            UnknownA = bin.ReadInt32();
            bin.ReadInt32(); //File Size
            UnknownB = bin.ReadInt32();
            UnknownC = bin.ReadInt32();

            UnknownD01 = bin.ReadInt32();
            UnknownD02 = bin.ReadInt32();
            UnknownD03 = bin.ReadInt32();
            UnknownD04 = bin.ReadInt32();
            UnknownD05 = bin.ReadInt32();
            UnknownD06 = bin.ReadInt32();
            UnknownD07 = bin.ReadInt32();

            Signature = bin.ReadBytes(Signature_Length);
            for (int i = 0; i < Signature_Length; i++)
            {
                if (Signature[i] != Signature_Default[i])
                {
                    throw new Exception("MTD signature invalid.");
                }
            }

            UnknownE01 = bin.ReadInt32();
            UnknownE02 = bin.ReadInt32();
            UnknownE03 = bin.ReadInt32();
            UnknownE04 = bin.ReadInt32();

            bin.ReadInt32(); //Data Size

            UnknownF01 = bin.ReadInt32();
            UnknownF02 = bin.ReadInt32();
            UnknownF03 = bin.ReadInt32();

            SourceSpxName = bin.ReadMtdName();
            Description = bin.ReadMtdName();


            UnknownG = bin.ReadBytes(UnknownG_Length);


            InternalParams.Clear();
            int internalParamCount = bin.ReadInt32();
            for (int i = 0; i < internalParamCount; i++)
            {
                InternalParams.Add(InternalMtdParam.Read(bin));
            }


            UnknownH = bin.ReadBytes(UnknownH_Length);


            ExternalParams.Clear();
            int externalParamCount = bin.ReadInt32();
            for (int i = 0; i < externalParamCount; i++)
            {
                ExternalParams.Add(ExternalMtdParam.Read(bin));
            }

            UnknownI01 = bin.ReadInt32();
            UnknownI02 = bin.ReadInt32();
            UnknownI03 = bin.ReadInt32();
            UnknownI04 = bin.ReadInt32();
            UnknownI05 = bin.ReadInt32();
            UnknownI06 = bin.ReadInt32();

            //TODO: Add real progress.
            prog?.Report((1, 1)); //PLACEHOLDER
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            bin.Write(UnknownA);

            var LOC_FileSize = bin.Position;
            bin.Placeholder();

            bin.Write(UnknownB);
            bin.Write(UnknownC);

            bin.Write(UnknownD01);
            bin.Write(UnknownD02);
            bin.Write(UnknownD03);
            bin.Write(UnknownD04);
            bin.Write(UnknownD05);
            bin.Write(UnknownD06);
            bin.Write(UnknownD07);

            bin.Write(Signature, Signature_Length);

            bin.Write(UnknownE01);
            bin.Write(UnknownE02);
            bin.Write(UnknownE03);
            bin.Write(UnknownE04);

            var LOC_DataSize = bin.Position;

            bin.Placeholder();

            bin.Write(UnknownF01);
            bin.Write(UnknownF02);
            bin.Write(UnknownF03);

            bin.WriteMtdName(SourceSpxName, 0xA3);
            bin.WriteMtdName(Description, 0x03);

            bin.Write(UnknownG, UnknownG_Length);

            bin.Write(InternalParams.Count);

            foreach (var p in InternalParams)
            {
                InternalMtdParam.Write(bin, p);
            }

            bin.Write(UnknownH, UnknownH_Length);

            bin.Write(ExternalParams.Count);

            foreach (var p in ExternalParams)
            {
                ExternalMtdParam.Write(bin, p);
            }

            bin.Write(UnknownI01);
            bin.Write(UnknownI02);
            bin.Write(UnknownI03);
            bin.Write(UnknownI04);
            bin.Write(UnknownI05);
            bin.Write(UnknownI06);

            int fileSize = (int)(bin.Position - 0x08);

            bin.Position = LOC_FileSize;
            bin.Write(fileSize);

            bin.Position = LOC_DataSize;
            bin.Write(fileSize - 0x44);

            //TODO: Add real progress.
            prog?.Report((1, 1)); //PLACEHOLDER
        }

        public static IList<Type> ParamTypes = new List<Type>
        {
            typeof(int),
            typeof(bool),
            typeof(float),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
        };

        public static IReadOnlyDictionary<Type, string> ParamNamesByType = new Dictionary<Type, string>
        {
            [typeof(int)] = "int",
            [typeof(bool)] = "bool",
            [typeof(float)] = "float",
            [typeof(Vector2)] = "float2",
            [typeof(Vector3)] = "float3",
            [typeof(Vector4)] = "float4",
        };

        public static IReadOnlyDictionary<Type, int> ParamSizesByType = new Dictionary<Type, int>
        {
            [typeof(int)] = 0x4,
            [typeof(bool)] = 0x1,
            [typeof(float)] = 0x4,
            [typeof(Vector2)] = 0x8,
            [typeof(Vector3)] = 0xC,
            [typeof(Vector4)] = 0x10,
        };

        public static IReadOnlyDictionary<string, Type> ParamTypesByName;

        static MTD()
        {
            ParamTypesByName = ParamNamesByType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }
    }
}
