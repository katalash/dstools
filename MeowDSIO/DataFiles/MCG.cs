using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class McgPoint
    {
        public float PosX  { get; set; }
        public float PosY  { get; set; }
        public float PosZ  { get; set; }

        // These lists are synced.
        // NearbyPathIndices[1] is the path that leads to NearbyPointIndices[1]
        // NearbyPathIndices[2] is the path that leads to NearbyPointIndices[2]
        // etc
        public List<int> NearbyPointIndices { get; set; } = new List<int>();
        public List<int> NearbyPathIndices { get; set; } = new List<int>();

        public int UnkE  { get; set; }
        public int UnkF  { get; set; }

        public string GetDebugReport()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{nameof(UnkE)} = {UnkE}");
            sb.AppendLine($"{nameof(UnkF)} = {UnkF}");

            return sb.ToString();
        }
    }

    public class McgPath
    {
        public int UnkA  { get; set; }
        public List<int> IndicesNotInMcgA { get; set; } = new List<int>();
        public int UnkC  { get; set; }
        public List<int> IndicesNotInMcgB  { get; set; } = new List<int>();
        public int MCPBoxIndex  { get; set; }
        public byte UnkF { get; set; }
        public byte UnkG { get; set; }
        public byte UnkH { get; set; }
        public byte UnkI { get; set; }
        public float UnkFloat  { get; set; }

        public string GetDebugReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(UnkA)} = {UnkA}");
            sb.AppendLine($"{nameof(IndicesNotInMcgA)} = {{{string.Join(", ", IndicesNotInMcgA)}}}");
            sb.AppendLine($"{nameof(UnkC)} = {UnkC}");
            sb.AppendLine($"{nameof(IndicesNotInMcgB)} = {{{string.Join(", ", IndicesNotInMcgB)}}}");
            sb.AppendLine($"{nameof(MCPBoxIndex)} = {MCPBoxIndex}");
            sb.AppendLine($"{nameof(UnkF)} = {UnkF}");
            sb.AppendLine($"{nameof(UnkG)} = {UnkG}");
            sb.AppendLine($"{nameof(UnkH)} = {UnkH}");
            sb.AppendLine($"{nameof(UnkI)} = {UnkI}");
            sb.AppendLine($"{nameof(UnkFloat)} = {UnkFloat}");

            return sb.ToString();
        }
    }

    public class MCG : DataFile
    {
        public List<McgPoint> Points = new List<McgPoint>();
        public List<McgPath> Paths = new List<McgPath>();

        List<(long Offset, string VariableName)> DEBUG_IndexMap = 
            new List<(long Offset, string VariableName)>();

        private int ReadIndex(DSBinaryReader bin, string DEBUG_IndexMapStr)
        {
            int result = -1;
            var indexOffset = bin.ReadInt32();
            bin.StepIn(indexOffset);
            {
                result = bin.ReadInt32();
            }
            bin.StepOut();

            DEBUG_IndexMap.Add((Offset: indexOffset, VariableName: $"{DEBUG_IndexMapStr} = {result}"));

            return result;
        }

        private List<int> ReadIndices(DSBinaryReader bin, string DEBUG_IndexMapStr, int count)
        {
            List<int> result = new List<int>();
            var indexOffset = bin.ReadInt32();
            bin.StepIn(indexOffset);
            {
                for (int i = 0; i < count; i++)
                {
                    var newInt = bin.ReadInt32();
                    DEBUG_IndexMap.Add((Offset: (indexOffset + (i * 4)), VariableName: $"{DEBUG_IndexMapStr}[{i}] = {newInt}"));
                    result.Add(newInt);
                }
                
            }
            bin.StepOut();

            

            return result;
        }

        private McgPoint ReadMcgPoint(DSBinaryReader bin, int DEBUG_ArrayIndex)
        {
            var result = new McgPoint();

            //result.UnkA = bin.ReadInt32();
            int indicesCount = bin.ReadInt32();
            result.PosX = bin.ReadSingle();
            result.PosY = bin.ReadSingle();
            result.PosZ = bin.ReadSingle();
            result.NearbyPointIndices = ReadIndices(bin, $"{nameof(McgPoint)}[{DEBUG_ArrayIndex}].{nameof(McgPoint.NearbyPointIndices)}", indicesCount);
            result.NearbyPathIndices = ReadIndices(bin, $"{nameof(McgPoint)}[{DEBUG_ArrayIndex}].{nameof(McgPoint.NearbyPathIndices)}", indicesCount);
            result.UnkE = bin.ReadInt32();
            result.UnkF = bin.ReadInt32();

            return result;
        }

        private McgPath ReadMcgPath(DSBinaryReader bin, int DEBUG_ArrayIndex)
        {
            var result = new McgPath();

            result.UnkA = bin.ReadInt32();
            int indicesA_Count = bin.ReadInt32();
            result.IndicesNotInMcgA = ReadIndices(bin, $"{nameof(McgPath)}[{DEBUG_ArrayIndex}].{nameof(McgPath.IndicesNotInMcgA)}", indicesA_Count);
            result.UnkC = bin.ReadInt32();
            int indicesB_Count = bin.ReadInt32();
            result.IndicesNotInMcgB = ReadIndices(bin, $"{nameof(McgPath)}[{DEBUG_ArrayIndex}].{nameof(McgPath.IndicesNotInMcgB)}", indicesB_Count);
            result.MCPBoxIndex = bin.ReadInt32();
            result.UnkF = bin.ReadByte();
            result.UnkG = bin.ReadByte();
            result.UnkH = bin.ReadByte();
            result.UnkI = bin.ReadByte();
            result.UnkFloat = bin.ReadSingle();

            return result;
        }

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            DEBUG_IndexMap = new List<(long Offset, string VariableName)>();

            bin.AssertInt32(1);
            bin.AssertInt32(0);
            int pointCount = bin.ReadInt32();
            int pointOffset = bin.ReadInt32();
            int pathCount = bin.ReadInt32();
            int pathOffset = bin.ReadInt32();
            bin.AssertInt32(0);
            bin.AssertInt32(0);

            Points = new List<McgPoint>();
            Paths = new List<McgPath>();

            bin.StepIn(pointOffset);
            {
                for (int i = 0; i < pointCount; i++)
                {
                    Points.Add(ReadMcgPoint(bin, i));
                }
            }
            bin.StepOut();

            bin.StepIn(pathOffset);
            {
                for (int i = 0; i < pathCount; i++)
                {
                    Paths.Add(ReadMcgPath(bin, i));
                }
            }
            bin.StepOut();
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            throw new NotImplementedException();
        }
    }
}
