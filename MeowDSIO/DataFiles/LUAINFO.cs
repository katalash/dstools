using MeowDSIO.DataTypes.LUAINFO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class LUAINFO : DataFile
    {
        public List<Goal> Goals = new List<Goal>();
        public LUAINFOHeader Header = new LUAINFOHeader();

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            Header.Signature = bin.ReadBytes(LUAINFOHeader.Signature_Length);

            if (Header.Signature.Where((x, i) => x != LUAINFOHeader.Signature_Default[i]).Any())
            {
                throw new Exception($"Invalid signature in this LUAINFO file header: " +
                    $"[{string.Join(",", Header.Signature.Select(x => x.ToString("X8")))}] " +
                    $"(ASCII: '{Encoding.ASCII.GetString(Header.Signature)}')");
            }

            Header.UnknownA = bin.ReadBytes(LUAINFOHeader.UnknownA_Length);

            int goalCount = bin.ReadInt32();

            Header.UnknownB = bin.ReadBytes(LUAINFOHeader.UnknownB_Length);

            Goals.Clear();

            List<int> goalNameOffsets = new List<int>();
            List<int> goalInterruptNameOffsets = new List<int>();

            for (int i = 0; i < goalCount; i++)
            {
                var nextGoal = new Goal();
                nextGoal.ID = bin.ReadInt32();

                goalNameOffsets.Add(bin.ReadInt32());
                goalInterruptNameOffsets.Add(bin.ReadInt32());

                nextGoal.IsBattleInterrupt = bin.ReadBoolean();
                nextGoal.IsLogicInterrupt = bin.ReadBoolean();
                nextGoal.Unknown1 = bin.ReadByte();
                nextGoal.Unknown2 = bin.ReadByte();

                Goals.Add(nextGoal);
            }

            for (int i = 0; i < goalCount; i++)
            {
                if (goalNameOffsets[i] > 0)
                {
                    bin.Position = goalNameOffsets[i];
                    Goals[i].Name = bin.ReadStringAscii();
                }
                else
                {
                    Goals[i].Name = null;
                }

                if (goalInterruptNameOffsets[i] > 0)
                {
                    bin.Position = goalInterruptNameOffsets[i];
                    Goals[i].LogicInterruptName = bin.ReadStringAscii();
                }
                else
                {
                    Goals[i].LogicInterruptName = null;
                }
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            bin.Write(Header.Signature, LUAINFOHeader.Signature_Length);
            bin.Write(Header.UnknownA, LUAINFOHeader.UnknownA_Length);
            bin.Write(Goals.Count);
            bin.Write(Header.UnknownB, LUAINFOHeader.UnknownB_Length);

            List<int> goalNameOffsets = new List<int>();
            List<int> goalInterruptNameOffsets = new List<int>();

            var OFF_GoalStart = bin.Position;

            // Each goal entry is 0x10 long. Skip past where they go and write the strings first.
            bin.Position += Goals.Count * 0x10;

            for (int i = 0; i < Goals.Count; i++)
            {
                if (Goals[i].Name != null)
                {
                    goalNameOffsets.Add((int)bin.Position);
                    bin.WriteStringAscii(Goals[i].Name, true);
                }
                else
                {
                    goalNameOffsets.Add(0);
                }

                if (Goals[i].LogicInterruptName != null)
                {
                    goalInterruptNameOffsets.Add((int)bin.Position);
                    bin.WriteStringAscii(Goals[i].LogicInterruptName, true);
                }
                else
                {
                    goalInterruptNameOffsets.Add(0);
                }
            }

            var OFF_AfterStrings = bin.Position;

            bin.Position = OFF_GoalStart;

            for (int i = 0; i < Goals.Count; i++)
            {
                bin.Write(Goals[i].ID);
                bin.Write(goalNameOffsets[i]);
                bin.Write(goalInterruptNameOffsets[i]);
                bin.Write(Goals[i].IsBattleInterrupt);
                bin.Write(Goals[i].IsLogicInterrupt);
                bin.Write(Goals[i].Unknown1);
                bin.Write(Goals[i].Unknown2);
            }

            bin.Position = OFF_AfterStrings;

            bin.Pad(0x10);
        }
    }
}
