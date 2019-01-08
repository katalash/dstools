using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SoulsFormats
{
    public partial class HKX
    {
        // From's basic collision class
        public class FSNPCustomParamCompressedMeshShape : HKXObject
        {
            public byte Unk10;
            public byte Unk11;
            public byte Unk12;
            public byte Unk13;
            public int Unk14;
            public HKArray<HKUInt> Unk68;
            public int Unk78;
            public HKArray<HKUInt> Unk80;
            public int Unk90;
            public HKArray<HKUInt> UnkA8;

            public HKXGlobalReference MeshShapeData;
            public HKXGlobalReference CustomParam;

            public override void Read(HKX hkx, HKXSection section, BinaryReaderEx br, HKXVariation variation)
            {
                SectionOffset = (uint)br.Position;

                br.AssertUInt64(0);
                br.AssertUInt64(0);
                Unk10 = br.ReadByte();
                Unk11 = br.ReadByte();
                Unk12 = br.ReadByte();
                Unk13 = br.ReadByte();
                Unk14 = br.ReadInt32();
                br.AssertUInt64(0);
                br.AssertUInt64(0);
                if (variation == HKXVariation.HKXDS3)
                {
                    br.AssertUInt64(0);
                }
                br.AssertUInt32(0xFFFFFFFF);
                br.AssertUInt32(0);

                // A seemingly empty array
                br.AssertUInt64(0);
                br.AssertUInt32(0);
                br.AssertUInt32(0x80000000);

                // A seemingly empty array
                br.AssertUInt64(0);
                br.AssertUInt32(0);
                br.AssertUInt32(0x80000000);

                br.AssertUInt32(0xFFFFFFFF);
                br.AssertUInt32(0);

                MeshShapeData = ResolveGlobalReference(section, br);

                Unk68 = new HKArray<HKUInt>(hkx, section, this, br, variation);
                Unk78 = br.ReadInt32();
                br.AssertUInt32(0);

                Unk80 = new HKArray<HKUInt>(hkx, section, this, br, variation);
                Unk90 = br.ReadInt32();
                br.AssertUInt32(0);

                br.AssertUInt64(0);

                CustomParam = ResolveGlobalReference(section, br);

                UnkA8 = new HKArray<HKUInt>(hkx, section, this, br, variation);
                if (variation == HKXVariation.HKXDS3)
                {
                    br.AssertUInt64(0);
                }

                DataSize = (uint)br.Position - SectionOffset;
                ResolveDestinations(hkx, section);
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                SectionOffset = (uint)bw.Position - sectionBaseOffset;
                bw.WriteInt64(0);
                bw.WriteInt64(0);
                bw.WriteByte(Unk10);
                bw.WriteByte(Unk11);
                bw.WriteByte(Unk12);
                bw.WriteByte(Unk13);
                bw.WriteInt32(Unk14);
                bw.WriteInt64(0);
                bw.WriteInt64(0);
                if (variation == HKXVariation.HKXDS3)
                {
                    bw.WriteInt64(0);
                }
                bw.WriteUInt32(0xFFFFFFFF);
                bw.WriteInt32(0);

                bw.WriteUInt64(0);
                bw.WriteUInt32(0);
                bw.WriteUInt32(0x80000000);

                bw.WriteUInt64(0);
                bw.WriteUInt32(0);
                bw.WriteUInt32(0x80000000);

                bw.WriteUInt32(0xFFFFFFFF);
                bw.WriteInt32(0);

                MeshShapeData.WritePlaceholder(bw, sectionBaseOffset);

                Unk68.Write(hkx, section, bw, sectionBaseOffset, variation);
                bw.WriteInt32(Unk78);
                bw.WriteInt32(0);

                Unk80.Write(hkx, section, bw, sectionBaseOffset, variation);
                bw.WriteInt32(Unk90);
                bw.WriteInt32(0);

                bw.WriteUInt64(0);

                CustomParam.WritePlaceholder(bw, sectionBaseOffset);

                UnkA8.Write(hkx, section, bw, sectionBaseOffset, variation);
                if (variation == HKXVariation.HKXDS3)
                {
                    bw.WriteInt64(0);
                }

                DataSize = (uint)bw.Position - sectionBaseOffset - SectionOffset;

                Unk68.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                Unk80.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                UnkA8.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
            }

            public HKNPCompressedMeshShapeData GetMeshShapeData()
            {
                return (HKNPCompressedMeshShapeData)MeshShapeData.DestObject;
            }
        }

        // Weird 5-byte structure
        public class UnknownStructure1 : IHKXSerializable
        {
            public byte Unk0;
            public byte Unk1;
            public byte Unk2;
            public byte Unk3;
            public byte Unk4;

            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                Unk0 = br.ReadByte();
                Unk1 = br.ReadByte();
                Unk2 = br.ReadByte();
                Unk3 = br.ReadByte();
                Unk4 = br.ReadByte();
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                bw.WriteByte(Unk0);
                bw.WriteByte(Unk1);
                bw.WriteByte(Unk2);
                bw.WriteByte(Unk3);
                bw.WriteByte(Unk4);
            }
        }

        public class MeshPrimitive : IHKXSerializable
        {
            public byte Idx0;
            public byte Idx1;
            public byte Idx2;
            public byte Idx3;

            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                Idx0 = br.ReadByte();
                Idx1 = br.ReadByte();
                Idx2 = br.ReadByte();
                Idx3 = br.ReadByte();
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                bw.WriteByte(Idx0);
                bw.WriteByte(Idx1);
                bw.WriteByte(Idx2);
                bw.WriteByte(Idx3);
            }
        }

        public class LargeCompressedVertex : IHKXSerializable
        {
            public ulong vertex;
            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                vertex = br.ReadUInt64();
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                bw.WriteUInt64(vertex);
            }

            // Decompress quantized vertex using collision mesh bounding box as quantization grid boundaries
            public Vector3 Decompress(Vector4 bbMin, Vector4 bbMax)
            {
                float scaleX = (bbMax.X - bbMin.X) / (float)((1 << 21) - 1);
                float scaleY = (bbMax.Y - bbMin.Y) / (float)((1 << 21) - 1);
                float scaleZ = (bbMax.Z - bbMin.Z) / (float)((1 << 22) - 1);
                float x = ((float)(vertex & 0x1FFFFF)) * scaleX + bbMin.X;
                float y = ((float)((vertex >> 21) & 0x1FFFFF)) * scaleY + bbMin.Y;
                float z = ((float)((vertex >> 42) & 0x3FFFFF)) * scaleZ + bbMin.Z;
                return new Vector3(x, y, z);
            }
        }

        public class SmallCompressedVertex : IHKXSerializable
        {
            public uint vertex;
            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                vertex = br.ReadUInt32();
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                bw.WriteUInt32(vertex);
            }

            // Decompress quantized vertex using collision mesh bounding box as quantization grid boundaries
            /*public Vector3 Decompress(Vector4 bbMin, Vector4 bbMax)
            {
                float scaleX = (bbMax.X - bbMin.X) / (float)((1 << 11) - 1);
                float scaleY = (bbMax.Y - bbMin.Y) / (float)((1 << 11) - 1);
                float scaleZ = (bbMax.Z - bbMin.Z) / (float)((1 << 10) - 1);
                float x = ((float)(vertex & 0x7FF)) * scaleX + bbMin.X;
                float y = ((float)((vertex >> 11) & 0x7FF)) * scaleY + bbMin.Y;
                float z = ((float)((vertex >> 22) & 0x3FF)) * scaleZ + bbMin.Z;
                return new Vector3(x, y, z);
            }*/

            public Vector3 Decompress(Vector3 scale, Vector3 offset)
            {
                float x = ((float)(vertex & 0x7FF)) * scale.X + offset.X;
                float y = ((float)((vertex >> 11) & 0x7FF)) * scale.Y + offset.Y;
                float z = ((float)((vertex >> 22) & 0x3FF)) * scale.Z + offset.Z;
                return new Vector3(x, y, z);
            }
        }

        public class CollisionMeshChunk : IHKXSerializable
        {
            HKXObject SourceObject;

            public HKArray<HKUInt> Unk0;
            public Vector4 Unk10;
            public Vector4 Unk20;
            public float Unk30;
            public float Unk34;
            public float Unk38;
            public float Unk3C;
            public float Unk40;
            public float Unk44;

            public Vector3 SmallVertexOffset;
            public Vector3 SmallVertexScale;

            public uint SmallVerticesBase;

            public int VertexIndicesIndex;
            public byte VertexIndicesLength;

            public int ByteIndicesIndex;
            public byte ByteIndicesLength;

            public int Unk54Index;
            public byte Unk54Length;

            uint Unk58;
            uint Unk5C;

            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                Unk0 = new HKArray<HKUInt>(hkx, section, source, br, variation);
                Unk10 = br.ReadVector4();
                Unk20 = br.ReadVector4();
                SmallVertexOffset = br.ReadVector3();
                SmallVertexScale = br.ReadVector3();

                SmallVerticesBase = br.ReadUInt32();

                uint vertexIndices = br.ReadUInt32();
                VertexIndicesIndex = (int)(vertexIndices >> 8);
                VertexIndicesLength = (byte)(vertexIndices & 0xFF);

                uint unk50 = br.ReadUInt32();
                ByteIndicesIndex = (int)(unk50 >> 8);
                ByteIndicesLength = (byte)(unk50 & 0xFF);

                uint unk54 = br.ReadUInt32();
                Unk54Index = (int)(unk54 >> 8);
                Unk54Length = (byte)(unk54 & 0xFF);

                Unk58 = br.ReadUInt32();
                Unk5C = br.ReadUInt32();
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                Unk0.Write(hkx, section, bw, sectionBaseOffset, variation);
                bw.WriteVector4(Unk10);
                bw.WriteVector4(Unk20);
                bw.WriteVector3(SmallVertexOffset);
                bw.WriteVector3(SmallVertexScale);
                bw.WriteUInt32(SmallVerticesBase);

                bw.WriteUInt32(((uint)(VertexIndicesIndex) << 8) | (uint)(VertexIndicesLength));
                bw.WriteUInt32(((uint)(ByteIndicesIndex) << 8) | (uint)(ByteIndicesLength));
                bw.WriteUInt32(((uint)(Unk54Index) << 8) | (uint)(Unk54Length));

                bw.WriteUInt32(Unk58);
                bw.WriteUInt32(Unk5C);
            }

            internal override void WriteReferenceData(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                Unk0.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
            }
        }

        public class UnknownStructure2 : IHKXSerializable
        {
            public uint Unk0;
            public uint Unk4;
            public uint Unk8;
            public uint UnkC;
            public uint Unk10;
            public uint Unk14;
            public uint Unk18;
            public uint Unk1C;
            public uint Unk20;
            public uint Unk24;
            public uint Unk28;
            public uint Unk2C;
            public uint Unk30;
            public uint Unk34;
            public uint Unk38;
            public uint Unk3C;
            public uint Unk40;
            public uint Unk44;
            public uint Unk48;
            public uint Unk4C;
            public uint Unk50;
            public uint Unk54;
            public uint Unk58;
            public uint Unk5C;
            public uint Unk60;
            public uint Unk64;
            public uint Unk68;
            public uint Unk6C;

            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                Unk0 = br.ReadUInt32();
                Unk4 = br.ReadUInt32();
                Unk8 = br.ReadUInt32();
                UnkC = br.ReadUInt32();
                Unk10 = br.ReadUInt32();
                Unk14 = br.ReadUInt32();
                Unk18 = br.ReadUInt32();
                Unk1C = br.ReadUInt32();
                Unk20 = br.ReadUInt32();
                Unk24 = br.ReadUInt32();
                Unk28 = br.ReadUInt32();
                Unk2C = br.ReadUInt32();
                Unk30 = br.ReadUInt32();
                Unk34 = br.ReadUInt32();
                Unk38 = br.ReadUInt32();
                Unk3C = br.ReadUInt32();
                Unk40 = br.ReadUInt32();
                Unk44 = br.ReadUInt32();
                Unk48 = br.ReadUInt32();
                Unk4C = br.ReadUInt32();
                Unk50 = br.ReadUInt32();
                Unk54 = br.ReadUInt32();
                Unk58 = br.ReadUInt32();
                Unk5C = br.ReadUInt32();
                Unk60 = br.ReadUInt32();
                Unk64 = br.ReadUInt32();
                Unk68 = br.ReadUInt32();
                Unk6C = br.ReadUInt32();
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                bw.WriteUInt32(Unk0);
                bw.WriteUInt32(Unk4);
                bw.WriteUInt32(Unk8);
                bw.WriteUInt32(UnkC);
                bw.WriteUInt32(Unk10);
                bw.WriteUInt32(Unk14);
                bw.WriteUInt32(Unk18);
                bw.WriteUInt32(Unk1C);
                bw.WriteUInt32(Unk20);
                bw.WriteUInt32(Unk24);
                bw.WriteUInt32(Unk28);
                bw.WriteUInt32(Unk2C);
                bw.WriteUInt32(Unk30);
                bw.WriteUInt32(Unk34);
                bw.WriteUInt32(Unk38);
                bw.WriteUInt32(Unk3C);
                bw.WriteUInt32(Unk40);
                bw.WriteUInt32(Unk44);
                bw.WriteUInt32(Unk48);
                bw.WriteUInt32(Unk4C);
                bw.WriteUInt32(Unk50);
                bw.WriteUInt32(Unk54);
                bw.WriteUInt32(Unk58);
                bw.WriteUInt32(Unk5C);
                bw.WriteUInt32(Unk60);
                bw.WriteUInt32(Unk64);
                bw.WriteUInt32(Unk68);
                bw.WriteUInt32(Unk6C);
            }
        }

        // Collision data
        public class HKNPCompressedMeshShapeData : HKXObject
        {
            public HKArray<UnknownStructure1> Unk10;

            public Vector4 BoundingBoxMin;
            public Vector4 BoundingBoxMax;

            public uint Unk40;
            public uint Unk44;
            public uint Unk48;
            public uint Unk4C;

            public HKArray<CollisionMeshChunk> Chunks;
            public HKArray<MeshPrimitive> MeshIndices;
            public HKArray<HKUShort> VertexIndices;
            public HKArray<SmallCompressedVertex> SmallVertices;
            public HKArray<LargeCompressedVertex> LargeVertices;
            public HKArray<HKUInt> UnkA0;
            public ulong UnkB0;
            public HKArray<UnknownStructure2> UnkB8;
            public ulong UnkC8;

            public override void Read(HKX hkx, HKXSection section, BinaryReaderEx br, HKXVariation variation)
            {
                SectionOffset = (uint)br.Position;

                br.AssertUInt64(0);
                br.AssertUInt64(0);

                Unk10 = new HKArray<UnknownStructure1>(hkx, section, this, br, variation);
                BoundingBoxMin = br.ReadVector4();
                BoundingBoxMax = br.ReadVector4();

                Unk40 = br.ReadUInt32();
                Unk44 = br.ReadUInt32();
                Unk48 = br.ReadUInt32();
                Unk4C = br.ReadUInt32();

                Chunks = new HKArray<CollisionMeshChunk>(hkx, section, this, br, variation);
                MeshIndices = new HKArray<MeshPrimitive>(hkx, section, this, br, variation);
                VertexIndices = new HKArray<HKUShort>(hkx, section, this, br, variation);
                SmallVertices = new HKArray<SmallCompressedVertex>(hkx, section, this, br, variation);
                LargeVertices = new HKArray<LargeCompressedVertex>(hkx, section, this, br, variation);
                UnkA0 = new HKArray<HKUInt>(hkx, section, this, br, variation);
                UnkB0 = br.ReadUInt64();
                UnkB8 = new HKArray<UnknownStructure2>(hkx, section, this, br, variation);
                UnkC8 = br.AssertUInt64(0);

                DataSize = (uint)br.Position - SectionOffset;
                ResolveDestinations(hkx, section);
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                SectionOffset = (uint)bw.Position - sectionBaseOffset;

                bw.WriteUInt64(0);
                bw.WriteUInt64(0);

                Unk10.Write(hkx, section, bw, sectionBaseOffset, variation);
                bw.WriteVector4(BoundingBoxMin);
                bw.WriteVector4(BoundingBoxMax);

                bw.WriteUInt32(Unk40);
                bw.WriteUInt32(Unk44);
                bw.WriteUInt32(Unk48);
                bw.WriteUInt32(Unk4C);

                Chunks.Write(hkx, section, bw, sectionBaseOffset, variation);
                MeshIndices.Write(hkx, section, bw, sectionBaseOffset, variation);
                VertexIndices.Write(hkx, section, bw, sectionBaseOffset, variation);
                SmallVertices.Write(hkx, section, bw, sectionBaseOffset, variation);
                LargeVertices.Write(hkx, section, bw, sectionBaseOffset, variation);
                UnkA0.Write(hkx, section, bw, sectionBaseOffset, variation);
                bw.WriteUInt64(UnkB0);
                UnkB8.Write(hkx, section, bw, sectionBaseOffset, variation);
                bw.WriteUInt64(UnkC8);

                DataSize = (uint)bw.Position - sectionBaseOffset - SectionOffset;
                Unk10.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                Chunks.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                MeshIndices.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                VertexIndices.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                SmallVertices.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                LargeVertices.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                UnkA0.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
                UnkB8.WriteReferenceData(hkx, section, bw, sectionBaseOffset, variation);
            }
        };
    }
}
