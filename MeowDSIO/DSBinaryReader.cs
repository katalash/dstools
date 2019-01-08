using MeowDSIO.DataFiles;
using MeowDSIO.Exceptions.DSRead;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using MeowDSIO.Exceptions;

namespace MeowDSIO
{
    public partial class DSBinaryReader : BinaryReader
    {
        public DSBinaryReader(string fileName, Stream input)
            : base(input)
        {
            FileName = fileName;
        }

        public DSBinaryReader(string fileName, Stream input, Encoding encoding)
            : base(input, encoding)
        {
            FileName = fileName;
        }

        public DSBinaryReader(string fileName, Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
            FileName = fileName;
        }

        private long currentMsbStructOffset = -1;

        public int MsbOffset
        {
            get
            {
                if (currentMsbStructOffset >= 0)
                {
                    return (int)(Position - currentMsbStructOffset);
                }
                else
                {
                    throw new DSReadException(this, $"Attempted to read current MSB struct offset without running .{nameof(StartMsbStruct)}() first.");
                }
            }
            set
            {
                if (currentMsbStructOffset >= 0)
                {
                    Position = (currentMsbStructOffset + value);
                }
                else
                {
                    throw new DSReadException(this, $"Attempted to write current MSB struct offset without running .{nameof(StartMsbStruct)}() first.");
                }
            }
        }

        public void StartMsbStruct()
        {
            currentMsbStructOffset = Position;
        }

        public void EndMsbStruct()
        {
            currentMsbStructOffset = -1;
        }

        /// <summary>
        /// Reads an ASCII string.
        /// </summary>
        /// <param name="length">If non-null, reads the specified number of characters. 
        /// <para/>If null, reads characters until it reaches a control character of value 0 (and this 0-value is excluded from the returned string).</param>
        /// <returns>An ASCII string.</returns>
        public string ReadStringAscii(int? length = null)
        {
            if (length.HasValue)
            {
                return Encoding.ASCII.GetString(ReadBytes(length.Value));
            }
            else
            {
                var sb = new StringBuilder();

                byte[] nextByte = new byte[] { 0 };

                while (true)
                {
                    nextByte[0] = ReadByte();

                    if (nextByte[0] > 0)
                        sb.Append(Encoding.ASCII.GetChars(nextByte));
                    else
                        break;
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Reads an ASCII string.
        /// </summary>
        /// <param name="length">If non-null, reads the specified number of characters. 
        /// <para/>If null, reads characters until it reaches a control character of value 0 (and this 0-value is excluded from the returned string).</param>
        /// <returns>An ASCII string.</returns>
        public string ReadStringUnicode(int? length = null)
        {
            if (length.HasValue)
            {
                if (BigEndian)
                    return Encoding.BigEndianUnicode.GetString(ReadBytes(length.Value * 2));
                else
                    return Encoding.Unicode.GetString(ReadBytes(length.Value * 2));
            }
            else
            {
                var sb = new StringBuilder();

                byte[] nextBytes = new byte[] { 0, 0 };

                while (true)
                {
                    nextBytes = ReadBytes(2);

                    if (nextBytes[0] != 0 || nextBytes[1] != 0)
                    {
                        if (BigEndian)
                            sb.Append(Encoding.BigEndianUnicode.GetChars(nextBytes));
                        else
                            sb.Append(Encoding.Unicode.GetChars(nextBytes));
                    }
                    else
                    {
                        break;
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Reads a Shift-JIS string.
        /// </summary>
        /// <returns>A Shift-JIS string.</returns>
        public string ReadStringShiftJIS(int specificLength = -1, bool stopOnTerminator = true)
        {
            if (!stopOnTerminator)
                return ShiftJISEncoding.GetString(ReadBytes(specificLength).ToArray());

            List<byte> shiftJisData = new List<byte>();

            byte nextByte = 0;

            while (specificLength < 0 || shiftJisData.Count < specificLength)
            {
                nextByte = ReadByte();

                if (stopOnTerminator && nextByte == 0)
                    break;

                shiftJisData.Add(nextByte);
            }

            if (shiftJisData.Count == 0)
                return "";

            return ShiftJISEncoding.GetString(shiftJisData.ToArray());
        }

        public string ReadStringShiftJIS(int specificLength)
        {
            return ReadStringShiftJIS(specificLength, false);
        }


        public void Pad(int align)
        {
            var off = Position % align;
            if (off > 0)
            {
                ReadBytes((int)(align - off));
            }
        }

        public byte ReadMtdDelimiter()
        {
            byte result = ReadByte();
            Pad(4);
            return result;
        }

        public Vector2 ReadVector2()
        {
            float x = ReadSingle();
            float y = ReadSingle();
            return new Vector2(x, y);
        }

        public Vector3 ReadVector3()
        {
            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            return new Vector3(x, y, z);
        }

        

        public Vector3? ReadFlverNullableVector3(byte[] compare)
        {
            byte[] valueBytes = ReadBytes(compare.Length);

            bool isNull = true;
            for (int i = 0; i < valueBytes.Length; i++)
            {
                if (valueBytes[i] != compare[i])
                {
                    isNull = false;
                    break;
                }
            }

            if (isNull)
                return null;

            Position -= compare.Length;

            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            return new Vector3(x, y, z);
        }

        public Vector4 ReadVector4()
        {
            float w = ReadSingle();
            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            return new Vector4(w, x, y, z);
        }

        public string ReadMtdName(out byte delim)
        {
            int valLength = ReadInt32();
            string result = ReadStringShiftJIS(valLength);
            delim = ReadMtdDelimiter();
            return result;
        }

        public string ReadMtdName()
        {
            return ReadMtdName(out _);
        }

        public byte ReadByte(string valName, params byte[] checkValues)
        {
            byte val = ReadByte();
            if (!checkValues.Contains(val))
            {
                throw new Exception($"Unexpected value found for {valName}: {val}");
            }
            return val;
        }

        public TVal CheckConsumeValue<TVal>(
            string ValueNameStr,
            Func<TVal> ReadFunction, 
            TVal ExpectedValue)
            where TVal : IEquatable<TVal>
        {
            TVal ConsumedValue = ReadFunction();
            
            if (!((IEquatable<TVal>)ConsumedValue).Equals(ExpectedValue))
            {
                throw new ConsumeValueCheckFailedException<TVal>(this, ValueNameStr, ExpectedValue, ConsumedValue);
            }

            return ConsumedValue;
        }

        public string ReadPaddedStringShiftJIS(int paddedRegionLength, byte? padding)
        {
            byte[] data = ReadBytes(paddedRegionLength);
            int strEndIndex = -1;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    strEndIndex = i;
                    break;
                }
            }

            // String has null-terminator in it
            if (strEndIndex > 0)
            {
                return ShiftJISEncoding.GetString(data, 0, strEndIndex);
            }
            // String begins with null-terminator
            else if (strEndIndex == 0)
            {
                // If string ends on very first byte, there's no real point to 
                // running it through the encoding and all that.
                return string.Empty;
            }
            // String has no null-terminator
            else
            {
                return ShiftJISEncoding.GetString(data);
            }
        }

        public string ReadPaddedStringUnicode(int paddedRegionLength, byte? padding)
        {
            byte[] data = ReadBytes(paddedRegionLength);
            int strEndIndex = -1;
            int currentIndex = 0;
            while (currentIndex < data.Length)
            {
                if (data[currentIndex] != 0)
                {
                    currentIndex++;
                    continue;
                }
                strEndIndex = currentIndex;
                break;
            }
            if (strEndIndex > 0)
            {
                return Encoding.Unicode.GetString(data, 0, strEndIndex);
            }
            if (strEndIndex == 0)
            {
                return string.Empty;
            }
            return Encoding.Unicode.GetString(data);
        }

        public TData ReadAsDataFile<TData>(string virtualUri = null, int dataSizeInBytes = -1, bool forceNoDcx = false)
            where TData: DataFile, new()
        {
            byte[] data = ReadBytes((dataSizeInBytes < 0) ? (int)Length : dataSizeInBytes);
            return DataFile.LoadFromBytes<TData>(data, virtualUri ?? FileName, null, forceNoDcx);
        }

        public byte[] ReadAllBytes()
        {
            return ReadBytes((int)BaseStream.Length);
        }
        
        public void AssertByte(byte value)
        {
            byte b = ReadByte();
            if (b != value)
            {
                throw new InvalidDataException(string.Format(
                    "Read byte: 0x{0:X} | Expected byte: 0x{1:X}", b, value));
            }

        }

        public void AssertRepeatedBytes(byte value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                AssertByte(value);
            }
        }

        public void AssertBytes(params byte[] values)
        {
            foreach (byte value in values)
            {
                byte b = ReadByte();
                if (b != value)
                {
                    throw new InvalidDataException(string.Format(
                        "Read byte: 0x{0:X} | Expected byte: 0x{1:X}", b, value));
                }
            }
        }

        public int AssertInt32(int value)
        {
            int i = ReadInt32();
            if (i != value)
            {
                throw new InvalidDataException(string.Format(
                    "Read int: 0x{0:X} | Expected int: 0x{1:X}", i, value));
            }
            return i;
        }

        public void AssertStringAscii(string value, int length)
        {
            string s = ReadStringAscii(length);
            if (s != value)
            {
                throw new InvalidDataException(string.Format(
                    "Read string: {0} | Expected string: {1}", s, value));
            }
        }

        public string ReadMsbString()
        {
            string result = null;

            int msbStringOffset = ReadInt32();
            if (msbStringOffset > 0)
            {
                StepInMSB(msbStringOffset);
                {
                    result = ReadStringShiftJIS();
                }
                StepOut();
            }
            else
            {
                throw new DSReadException(this, "Read an MSB string offset of 0. I thought all MSB strings pointed somewhere, even if empty...?");
            }

            return result;
        }
    }
}
