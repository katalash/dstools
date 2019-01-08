using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class ESD : DataFile
    {
        public class EzHeader
        {
            public int Unk0 = 1;
            public int Unk1 = 1;
            public int Unk2 = 1;
            public int Unk3 = 0x54;
            //int FileSize
            public int Unk4 = 6;
            public int Unk5 = 0x2C;
            public int Unk6 = 1;
            public int Unk7 = 0x10;
            public int Unk8 = 2;
            public int Unk9 = 0x24;
            //int NumStateEntry
            public int Unk11 = 0x1C;
            //int NumNextEntry
            public int Unk13 = 0x10;
            //int NumFuncEntry
            public int Unk15 = 8;
            //int NumFunctionParamEntry
            //int OffsetTransitionEntry
            //int NumTransitionEntry
            //int OffsetEndOfBS0
            //pad0
            //OffsetEndOfBS1
            //pad1
            //OffsetEndOfBS2
            //pad2
            public int Unk18 = 1;

            public const int UNK_BYTES_COUNT = 16;
            public byte[] UnkBytes;
        }

        public class EzCommandArg
        {
            public const int SIZE = sizeof(int) * 2;

            public byte[] Bytes;

            public override string ToString()
            {
                return Evaluate();
                if (Bytes.Length <= 128)
                    return $"{{ {string.Join(" ", Bytes.Select(x => x.ToString("X2")))} }}";
                else
                    return $"{{ {Bytes.Length} BYTES }}";
            }

            private static string GetComp(byte comp)
            {
                switch (comp)
                {
                    case 0x91: return "<=";
                    case 0x92: return ">=";
                    case 0x93: return "<";
                    case 0x94: return ">";
                    case 0x95: return "==";
                    case 0x96: return "!=";
                }
                throw new ArgumentException("Not a valid comparison byte");
            }

            public string Evaluate()
            {
                Stack<object> stack = new Stack<object>();

                object[] reg = new object[8];

                for (int i = 0; i < Bytes.Length; i++)
                {
                    var b = Bytes[i];
                    if (b >= 0x3F && b <= 0x7F)
                    {
                        stack.Push(b - 64);
                    }
                    else if (b == 0xA5)
                    {
                        var sb = new StringBuilder();
                        char nextChar = '?';
                        do
                        {
                            nextChar = BitConverter.ToChar(Bytes, i + 1);
                            if (nextChar != (char)0)
                                sb.Append(nextChar);
                            i += 2;
                        }
                        while (nextChar != (char)0);
                        stack.Push(sb.ToString());
                    }
                    else if (b == 0x80)
                    {
                        stack.Push(BitConverter.ToSingle(Bytes, i + 1));
                        i += 4;
                    }
                    else if (b == 0x81)
                    {
                        stack.Push(BitConverter.ToDouble(Bytes, i + 1));
                        i += 8;
                    }
                    else if (b == 0x82)
                    {
                        stack.Push(BitConverter.ToInt32(Bytes, i + 1));
                        i += 4;
                    }
                    else if (b == 0x84)
                    {
                        var id = Convert.ToInt32(stack.Pop());
                        stack.Push(EzCommand.GetString(id));
                    }
                    else if (b == 0x85)
                    {
                        var arg1 = stack.Pop();
                        var id = Convert.ToInt32(stack.Pop());
                        stack.Push(EzCommand.GetString(id, arg1));
                    }
                    else if (b == 0x86)
                    {
                        var arg2 = stack.Pop();
                        var arg1 = stack.Pop();
                        var id = Convert.ToInt32(stack.Pop());
                        stack.Push(EzCommand.GetString(id, arg1, arg2));
                    }
                    else if (b >= 0x91 && b <= 0x96)
                    {
                        var item2 = stack.Pop();
                        var item1 = stack.Pop();
                        stack.Push($"({item1}) {GetComp(b)} ({item2})");
                    }
                    else if (b == 0x98)
                    {
                        var item2 = stack.Pop();
                        var item1 = stack.Pop();
                        stack.Push($"({item1}) && ({item2})");
                    }
                    else if (b == 0x99)
                    {
                        var item2 = stack.Pop();
                        var item1 = stack.Pop();
                        stack.Push($"({item1}) || ({item2})");
                    }
                    else if (b == 0xA6)
                    {
                        stack.Push($"[CONT.]");
                    }
                    else if (b >= 0xA7 && b <= 0xAE)
                    {
                        byte regIndex = (byte)(b - 0xA7);
                        var item = stack.Peek();
                        stack.Push($"{item} => REG[{regIndex}]");
                        reg[regIndex] = item;
                    }
                    else if (b >= 0xAF && b <= 0xB6)
                    {
                        byte regIndex = (byte)(b - 0xAF);
                        stack.Push($"REG[{regIndex}] => {reg[regIndex]}");
                    }
                    else if (b == 0xB7)
                    {
                        stack.Push($"[STOP IF FALSE]");
                    }
                    else if (b == 0xA1)
                    {
                        //stack.Push("\n");
                    }
                    else if (b >= 0x8C && b <= 0x8F)
                    {
                        var item2 = stack.Pop();
                        var item1 = stack.Pop();
                        stack.Push($"({item1}) <op 0x{b:X2}> ({item2})");
                    }
                    else
                    {
                        stack.Push($"<?0x{b:X2}?>");
                    }
                }
                return string.Join(", ", stack);
            }
        }

        public class EzCommand
        {
            public const int SIZE = sizeof(int) * 4;

            public static string GetString(int id, params object[] args)
            {
                return $"{nameof(EzCommand)}<{id}>({string.Join(", ", args)})";
            }

            public int Unk0 = 1;
            public int ID;
            public List<EzCommandArg> Args = new List<EzCommandArg>();

            public override string ToString()
            {
                return $"{nameof(EzCommand)}<{ID}>({string.Join(", ", Args)})";
            }
        }

        public class EzState
        {
            public const int SIZE = sizeof(int) * 9;

            public int Index;
            public List<EzSubState> SubStatesA = new List<EzSubState>();
            public List<EzCommand> OnCommands = new List<EzCommand>();
            public List<EzCommand> OffCommands = new List<EzCommand>();
            public List<EzSubState> SubStatesB = new List<EzSubState>();

            public override string ToString()
            {
                return $"{nameof(EzState)} <IDX {Index}>";
            }
        }

        public class EzSubState
        {
            public const int SIZE = sizeof(int) * 7;

            public EzState NextState;
            public List<EzCommand> Commands = new List<EzCommand>();
            public List<EzSubState> SubStates = new List<EzSubState>();
            public byte[] PackedData;

            public override string ToString()
            {
                return $"--> {NextState?.ToString() ?? "<NULL>"}";
            }
        }

        const int FILE_OFFSET = 0x6C;

        public class ESDLoader
        {
            private readonly DSBinaryReader bin;

            public ESDLoader(DSBinaryReader b)
            {
                bin = b;
            }

            public Dictionary<int, EzCommand> DictEzCommand = new Dictionary<int, EzCommand>();
            public Dictionary<int, EzCommandArg> DictEzCommandArg = new Dictionary<int, EzCommandArg>();
            public Dictionary<int, EzState> DictEzState = new Dictionary<int, EzState>();
            public Dictionary<int, EzSubState> DictEzSubState = new Dictionary<int, EzSubState>();

            public EzCommandArg GetEzCommandArg(int offset)
            {
                if (DictEzCommandArg.ContainsKey(offset))
                    return DictEzCommandArg[offset];

                var esfp = new EzCommandArg();

                DictEzCommandArg.Add(offset, esfp);

                bin.StepIn(offset);
                {
                    int bytesOffset = bin.ReadInt32();
                    int bytesCount = bin.ReadInt32();

                    bin.StepIn(FILE_OFFSET + bytesOffset);
                    {
                        esfp.Bytes = bin.ReadBytes(bytesCount);
                    }
                    bin.StepOut();
                }
                bin.StepOut();

                return esfp;
            }

            public EzCommand GetEzCommand(int offset)
            {
                if (DictEzCommand.ContainsKey(offset))
                    return DictEzCommand[offset];

                var esf = new EzCommand();

                DictEzCommand.Add(offset, esf);

                bin.StepIn(offset);
                {
                    esf.Unk0 = bin.ReadInt32();
                    esf.ID = bin.ReadInt32();
                    int argOffset = bin.ReadInt32();
                    int argCount = bin.ReadInt32();

                    if (argOffset != -1)
                    {
                        for (int i = 0; i < argCount; i++)
                        {
                            esf.Args.Add(GetEzCommandArg(FILE_OFFSET + argOffset + EzCommandArg.SIZE * i));
                        }
                    }
                }
                bin.StepOut();

                return esf;
            }

            public EzSubState GetEzSubState(int offset)
            {
                if (DictEzSubState.ContainsKey(offset))
                    return DictEzSubState[offset];

                var ess = new EzSubState();

                DictEzSubState.Add(offset, ess);

                bin.StepIn(offset);
                {
                    int nextStateOffset = bin.ReadInt32();

                    int commandOffset = bin.ReadInt32();
                    int commandCount = bin.ReadInt32();

                    int transitionOffset = bin.ReadInt32();
                    int transitionCount = bin.ReadInt32();

                    int packedDataOffset = bin.ReadInt32();
                    int packedDataCount = bin.ReadInt32();

                    if (nextStateOffset != -1)
                    {
                        ess.NextState = GetEzState(FILE_OFFSET + nextStateOffset);
                    }

                    if (commandOffset != -1)
                    {
                        for (int i = 0; i < commandCount; i++)
                        {
                            ess.Commands.Add(GetEzCommand(FILE_OFFSET + commandOffset + EzCommand.SIZE * i));
                        }
                    }

                    if (transitionOffset != -1)
                    {
                        bin.StepIn(FILE_OFFSET + transitionOffset);
                        {
                            for (int i = 0; i < transitionCount; i++)
                            {
                                int substateOffset = bin.ReadInt32();
                                ess.SubStates.Add(GetEzSubState(FILE_OFFSET + substateOffset));
                            }
                        }
                        bin.StepOut();
                    }

                    if (packedDataOffset != -1)
                    {
                        bin.StepIn(FILE_OFFSET + packedDataOffset);
                        {
                            ess.PackedData = bin.ReadBytes(packedDataCount);
                        }
                        bin.StepOut();
                    }
                }

                bin.StepOut();

                return ess;
            }

            public EzState GetEzState(int offset)
            {
                if (DictEzState.ContainsKey(offset))
                    return DictEzState[offset];

                var s = new EzState();

                DictEzState.Add(offset, s);

                bin.StepIn(offset);
                {
                    s.Index = bin.ReadInt32();

                    int transOffsetA = bin.ReadInt32();
                    int transCountA = bin.ReadInt32();

                    int cmdOffsetA = bin.ReadInt32();
                    int cmdCountA = bin.ReadInt32();

                    int cmdOffsetB = bin.ReadInt32();
                    int cmdCountB = bin.ReadInt32();

                    int transOffsetB = bin.ReadInt32();
                    int transCountB = bin.ReadInt32();

                    //bin.AssertInt32(-1);
                    //bin.AssertInt32(0);

                    if (transOffsetA != -1)
                    {
                        bin.StepIn(FILE_OFFSET + transOffsetA);
                        {
                            for (int i = 0; i < transCountA; i++)
                            {
                                int substateOffset = bin.ReadInt32();
                                s.SubStatesA.Add(GetEzSubState(FILE_OFFSET + substateOffset));
                            }
                        }
                        bin.StepOut();
                    }

                    if (cmdOffsetA != -1)
                    {
                        for (int i = 0; i < cmdCountA; i++)
                        {
                            s.OnCommands.Add(GetEzCommand(FILE_OFFSET + cmdOffsetA + EzCommand.SIZE * i));
                        }
                    }

                    if (cmdOffsetB != -1)
                    {
                        for (int i = 0; i < cmdCountB; i++)
                        {
                            s.OffCommands.Add(GetEzCommand(FILE_OFFSET + cmdOffsetB + EzCommand.SIZE * i));
                        }
                    }

                    if (transOffsetB != -1)
                    {
                        bin.StepIn(FILE_OFFSET + transOffsetB);
                        {
                            for (int i = 0; i < transCountB; i++)
                            {
                                int substateOffset = bin.ReadInt32();
                                s.SubStatesB.Add(GetEzSubState(FILE_OFFSET + substateOffset));
                            }
                        }
                        bin.StepOut();
                    }
                }
                bin.StepOut();

                return s;
            }
        }

        public const int MAGIC_DS1_PTDE = 0x4C535366; //fSSL

        public EzHeader Header { get; set; } = new EzHeader();

        public List<EzState> StatesA { get; set; } = new List<EzState>();
        public List<EzState> StatesB { get; set; } = new List<EzState>();

        public List<EzSubState> SubStateBank { get; set; } = new List<EzSubState>();
        public List<EzCommand> CommandBank { get; set; } = new List<EzCommand>();
        public List<EzCommandArg> CommandArgBank { get; set; } = new List<EzCommandArg>();

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            bin.AssertInt32(MAGIC_DS1_PTDE);

            var loader = new ESDLoader(bin);

            Header = new EzHeader();
            Header.Unk0 = bin.AssertInt32(1);
            Header.Unk1 = bin.AssertInt32(1);
            Header.Unk2 = bin.AssertInt32(1);
            Header.Unk3 = bin.AssertInt32(0x54);
            int someEofOffsetShit = bin.ReadInt32();
            Header.Unk4 = bin.AssertInt32(6);
            Header.Unk5 = bin.AssertInt32(0x2C);
            Header.Unk6 = bin.AssertInt32(1);
            Header.Unk7 = bin.AssertInt32(0x10);
            Header.Unk8 = bin.AssertInt32(2);
            Header.Unk9 = bin.AssertInt32(0x24);
            int statesCount = bin.ReadInt32();
            Header.Unk11 = bin.AssertInt32(0x1C);
            int subStateCount = bin.ReadInt32();
            Header.Unk13 = bin.AssertInt32(0x10);
            int funcEntryCount = bin.ReadInt32();
            Header.Unk15 = bin.AssertInt32(8);
            int funcParamEntryCount = bin.ReadInt32();

            int transEntryOffset = bin.ReadInt32();
            int transEntryCount = bin.ReadInt32();

            if (transEntryOffset != -1)
            {
                bin.StepIn(FILE_OFFSET + transEntryOffset);
                {
                    for (int i = 0; i < transEntryCount; i++)
                    {
                        int substateOffset = bin.ReadInt32();
                        SubStateBank.Add(loader.GetEzSubState(FILE_OFFSET + substateOffset));
                    }
                }
                bin.StepOut();
            }

            int endOfBS0Offset = bin.ReadInt32();
            bin.AssertInt32(0);
            int endOfBS1Offset = bin.AssertInt32(endOfBS0Offset);
            bin.AssertInt32(0);
            int endOfBS2Offset = bin.AssertInt32(endOfBS0Offset);
            bin.AssertInt32(0);
            Header.Unk18 = bin.AssertInt32(1);

            //State Table Header

            Header.UnkBytes = bin.ReadBytes(EzHeader.UNK_BYTES_COUNT);

            int someOffsetA = bin.AssertInt32(0x2C);
            int someCountA = bin.ReadInt32();
            int someOffsetB = bin.AssertInt32(-1);
            int someCountB = bin.ReadInt32();

            bin.AssertRepeatedBytes(0, 12);

            int statesAOffset = bin.AssertInt32(0x4C);
            int statesACount = bin.ReadInt32();

            int statesAOffsetAGAIN = bin.AssertInt32(0x4C);
            int statesACountAGAIN = bin.ReadInt32();

            int statesBOffset = bin.ReadInt32();
            int statesBCount = bin.ReadInt32();

            int statesBOffsetAGAIN = bin.ReadInt32();

            int realStatesAOffset = (int)bin.Position;

            for (int i = 0; i < statesCount; i++)
            {
                StatesA.Add(loader.GetEzState(realStatesAOffset + EzState.SIZE * i));
            }

            int realStatesBOffset = (int)bin.Position;

            for (int i = 0; i < statesBCount; i++)
            {
                StatesB.Add(loader.GetEzState(realStatesBOffset + EzState.SIZE * i));
            }

            CommandBank = loader.DictEzCommand.Values.ToList();
            CommandArgBank = loader.DictEzCommandArg.Values.ToList();
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            throw new NotImplementedException();
        }
    }
}
