using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class MCP : DataFile
    {
        public class Box
        {
            public int MapID { get; set; }
            public int Index { get; set; }
            public List<int> Numbers { get; set; } = new List<int>();
            public float MinX { get; set; }
            public float MinY { get; set; }
            public float MinZ { get; set; }
            public float MaxX { get; set; }
            public float MaxY { get; set; }
            public float MaxZ { get; set; }
        }

        public int Version { get; set; } = 0x004085D1;
        public List<Box> Boxes { get; set; } = new List<Box>();


        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            bin.AssertInt32(2);
            Version = bin.ReadInt32();
            int boxCount = bin.ReadInt32();
            int boxOffset = bin.ReadInt32();
            bin.Position = boxOffset;
            Boxes = new List<Box>();
            for (int i = 0; i < boxCount; i++)
            {
                var box = new Box();
                box.MapID = bin.ReadInt32();
                box.Index = bin.ReadInt32();
                int numCount = bin.ReadInt32();
                int numOffset = bin.ReadInt32();
                bin.StepIn(numOffset);
                {
                    for (int j = 0; j < numCount; j++)
                    {
                        box.Numbers.Add(bin.ReadInt32());
                    }
                }
                bin.StepOut();
                box.MinX = bin.ReadSingle();
                box.MinY = bin.ReadSingle();
                box.MinZ = bin.ReadSingle();
                box.MaxX = bin.ReadSingle();
                box.MaxY = bin.ReadSingle();
                box.MaxZ = bin.ReadSingle();

                Boxes.Add(box);
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            throw new NotImplementedException();
        }
    }
}
