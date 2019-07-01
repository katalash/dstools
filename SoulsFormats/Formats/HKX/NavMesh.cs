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
        public class NVMFace : IHKXSerializable
        {
            public int StartEdgeIndex;
            public int StartUserEdgeIndex;
            public short NumEdges;
            public short NumUserEdges;
            public short ClusterIndex;

            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                StartEdgeIndex = br.ReadInt32();
                StartUserEdgeIndex = br.ReadInt32();
                NumEdges = br.ReadInt16();
                NumUserEdges = br.ReadInt16();
                ClusterIndex = br.ReadInt16();
                br.ReadInt16(); // Padding
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                throw new NotImplementedException();
            }
        }

        public class NVMEdge : IHKXSerializable
        {
            public int A;
            public int B;
            public uint OppositeEdge;
            public uint OppositeFace;
            public byte Flags;
            public short UserEdgeCost;

            public override void Read(HKX hkx, HKXSection section, HKXObject source, BinaryReaderEx br, HKXVariation variation)
            {
                A = br.ReadInt32();
                B = br.ReadInt32();
                OppositeEdge = br.ReadUInt32();
                OppositeFace = br.ReadUInt32();
                Flags = br.ReadByte();
                br.ReadByte(); // Padding
                UserEdgeCost = br.ReadInt16();
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                throw new NotImplementedException();
            }
        }

        public class HKAINavMesh : HKXObject
        {
            public HKArray<NVMFace> Faces;
            public HKArray<NVMEdge> Edges;
            public HKArray<HKVector4> Vertices;
            public HKArray<HKUInt> FaceData;
            public HKArray<HKUInt> EdgeData;
            int FaceDataStriding;
            int EdgeDataStriding;
            byte Flags;
            Vector4 AABBMin;
            Vector4 AABBMax;
            float ErosionRadius;
            ulong UserData;

            public override void Read(HKX hkx, HKXSection section, BinaryReaderEx br, HKXVariation variation)
            {
                SectionOffset = (uint)br.Position;

                AssertPointer(hkx, br);
                AssertPointer(hkx, br);

                Faces = new HKArray<NVMFace>(hkx, section, this, br, variation);
                Edges = new HKArray<NVMEdge>(hkx, section, this, br, variation);
                Vertices = new HKArray<HKVector4>(hkx, section, this, br, variation);
                br.ReadUInt64s(2); // hkaiStreamingSet seems unused
                FaceData = new HKArray<HKUInt>(hkx, section, this, br, variation);
                EdgeData = new HKArray<HKUInt>(hkx, section, this, br, variation);
                FaceDataStriding = br.ReadInt32();
                EdgeDataStriding = br.ReadInt32();
                Flags = br.ReadByte();
                br.AssertByte(0); // Padding
                br.AssertUInt16(0); // Padding
                AABBMin = br.ReadVector4();
                AABBMax = br.ReadVector4();
                ErosionRadius = br.ReadSingle();
                UserData = br.ReadUInt64();
                br.ReadUInt64(); // Padding

                DataSize = (uint)br.Position - SectionOffset;
                ResolveDestinations(hkx, section);
            }

            public override void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKXVariation variation)
            {
                SectionOffset = (uint)bw.Position - sectionBaseOffset;

                DataSize = (uint)bw.Position - sectionBaseOffset - SectionOffset;
            }
        }
    }
}
