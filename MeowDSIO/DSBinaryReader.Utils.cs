using MeowDSIO.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public partial class DSBinaryReader : BinaryReader
    {
        private static Encoding ShiftJISEncoding = Encoding.GetEncoding("shift_jis");

        // Now with 100% less 0DD0ADDE
        public static readonly byte[] PLACEHOLDER_32BIT = new byte[] { 0xDE, 0xAD, 0xD0, 0x0D };

        public string FileName { get; private set; }

        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public long Length => BaseStream.Length;
        public void Goto(long absoluteOffset) => BaseStream.Seek(absoluteOffset, SeekOrigin.Begin);
        public void Jump(long relativeOffset) => BaseStream.Seek(relativeOffset, SeekOrigin.Current);
        private Stack<long> StepStack = new Stack<long>();
        private Stack<PaddedRegion> PaddedRegionStack = new Stack<PaddedRegion>();

        public bool BigEndian = false;

        public void StepInMSB(int offset)
        {
            if (currentMsbStructOffset >= 0)
            {
                StepIn(currentMsbStructOffset + offset);
            }
            else
            {
                throw new DSReadException(this, $"Attempted to use .{nameof(StepInMSB)}() without running .{nameof(StartMsbStruct)}() first.");
            }
        }

        public void StepIn(long offset)
        {
            StepStack.Push(Position);
            Goto(offset);
        }

        public void StepOut()
        {
            if (StepStack.Count == 0)
                throw new InvalidOperationException("You cannot step out unless StepIn() was previously called on an offset.");

            Goto(StepStack.Pop());
        }

        public void StepIntoPaddedRegion(long length, byte? padding)
        {
            PaddedRegionStack.Push(new PaddedRegion(Position, length, padding));
        }

        public void StepOutOfPaddedRegion()
        {
            if (PaddedRegionStack.Count == 0)
                throw new InvalidOperationException("You cannot step out of padded region unless inside of one.");

            var deepestPaddedRegion = PaddedRegionStack.Pop();
            deepestPaddedRegion.AdvanceReaderToEnd(this);
        }

        public void StepOutOfPaddedRegion(out byte foundPadding)
        {
            if (PaddedRegionStack.Count == 0)
                throw new InvalidOperationException("You cannot step out of padded region unless inside of one.");

            var deepestPaddedRegion = PaddedRegionStack.Pop();
            deepestPaddedRegion.AdvanceReaderToEnd(this, out foundPadding);
        }

        public void DoAt(long offset, Action doAction)
        {
            StepIn(offset);
            doAction();
            StepOut();
        }

        /// <summary>
        /// Reads a value using the given function, throwing an exception if it does not match any options specified.
        /// </summary>
        /// <param name="readValue">A function which reads one value.</param>
        /// <param name="typeName">The human-readable name of the type, to be included in the exception message.</param>
        /// <param name="valueFormat">A format to be applied to the read value and options, to be included in the exception message.</param>
        /// <param name="options">A list of possible values.</param>
        private T AssertValue<T>(Func<T> readValue, string typeName, string valueFormat, T[] options) where T : IEquatable<T>
        {
            T value = readValue();
            bool valid = false;
            foreach (T option in options)
                if (value.Equals(option))
                    valid = true;

            if (!valid)
            {
                string strValue = string.Format(valueFormat, value);

                List<string> strOptions = new List<string>();
                foreach (T option in options)
                    strOptions.Add(string.Format(valueFormat, option));

                throw new InvalidDataException(string.Format(
                    "Read {0}: {1} | Expected {0}: {2}", typeName, strValue, string.Join(", ", strOptions)));
            }

            return value;
        }

        /// <summary>
        /// Reads a one-byte boolean value and throws an exception if it does not match the specified option.
        /// </summary>
        public bool AssertBoolean(bool option)
        {
            return AssertValue(ReadBoolean, "Boolean", "{0}", new bool[] { option });
        }

        /// <summary>
        /// Reads a one-byte signed integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public sbyte AssertSByte(params sbyte[] options)
        {
            return AssertValue(ReadSByte, "SByte", "0x{0:X}", options);
        }

        /// <summary>
        /// Reads a one-byte unsigned integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public byte AssertByte(params byte[] options)
        {
            return AssertValue(ReadByte, "Byte", "0x{0:X}", options);
        }

        /// <summary>
        /// Reads a two-byte signed integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public short AssertInt16(params short[] options)
        {
            return AssertValue(ReadInt16, "Int16", "0x{0:X}", options);
        }

        /// <summary>
        /// Reads a two-byte unsigned integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public ushort AssertUInt16(params ushort[] options)
        {
            return AssertValue(ReadUInt16, "UInt16", "0x{0:X}", options);
        }

        /// <summary>
        /// Reads a four-byte signed integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public int AssertInt32(params int[] options)
        {
            return AssertValue(ReadInt32, "Int32", "0x{0:X}", options);
        }

        /// <summary>
        /// Reads a four-byte unsigned integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public uint AssertUInt32(params uint[] options)
        {
            return AssertValue(ReadUInt32, "UInt32", "0x{0:X}", options);
        }


        /// <summary>
        /// Reads an eight-byte signed integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public long AssertInt64(params long[] options)
        {
            return AssertValue(ReadInt64, "Int64", "0x{0:X}", options);
        }

        /// <summary>
        /// Reads an eight-byte unsigned integer and throws an exception if it does not match any of the specified options.
        /// </summary>
        public ulong AssertUInt64(params ulong[] options)
        {
            return AssertValue(ReadUInt64, "UInt64", "0x{0:X}", options);
        }

        /// <summary>
        /// Reads a four-byte floating point number and throws an exception if it does not match any of the specified options.
        /// </summary>
        public float AssertSingle(params float[] options)
        {
            return AssertValue(ReadSingle, "Single", "{0}", options);
        }

        /// <summary>
        /// Reads an eight-byte floating point number and throws an exception if it does not match any of the specified options.
        /// </summary>
        public double AssertDouble(params double[] options)
        {
            return AssertValue(ReadDouble, "Double", "{0}", options);
        }
    }
}
