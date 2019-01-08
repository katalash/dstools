using MeowDSIO.DataTypes.FMG;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeowDSIO.DataFiles
{
    public class FMG : DataFile, IDictionary<int, string>
    {
        public const string NullString = "<null>";
        public const string EmptyString = "<empty>";

        private FMGHeader _header = new FMGHeader();
        public FMGHeader Header
        {
            get => _header;
            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }

        
        private Dictionary<int, string> entries = new Dictionary<int, string>();

        private List<FMGChunk> CalculateChunks()
        {
            var chunks = new List<FMGChunk>();

            int startIndex = -1;
            int startID = -1;

            var entryList = entries.ToList();

            for (int i = 0; i < entryList.Count; i++)
            {
                if (startIndex < 0)
                {
                    startIndex = i;
                    startID = entryList[i].Key;
                    continue;
                }
                else if ((entryList[i].Key - entryList[i - 1].Key) > 1)
                {
                    chunks.Add(new FMGChunk(startIndex, startID, entryList[i - 1].Key));
                    startIndex = i;
                    startID = entryList[i].Key;
                }

            }

            // If there's an unfinished chunk, finish it
            if (chunks.Count > 0 && startIndex > chunks[chunks.Count - 1].StartIndex)
            {
                chunks.Add(new FMGChunk(startIndex, startID, entryList[entryList.Count - 1].Key));
            }

            return chunks;
        }

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            //UniEscapeChar
            bin.ReadUInt16();

            Header.UnkFlag01 = bin.ReadByte();
            Header.UnkFlag02 = bin.ReadByte();

            //FileSize
            bin.ReadInt32();

            Header.UnkFlag03 = bin.ReadByte();
            Header.IsBigEndian = (bin.ReadByte() == FMGHeader.ENDIAN_FLAG_BIG);
            Header.UnkFlag04 = bin.ReadByte();
            Header.UnkFlag05 = bin.ReadByte();

            int chunkCount = bin.ReadInt32();
            int stringCount = bin.ReadInt32();
            int stringOffsetsBegin = bin.ReadInt32();

            //Pad
            bin.ReadUInt32();

            entries.Clear();
            FMGChunkHeaderBuffer chunk = new FMGChunkHeaderBuffer(stringOffsetsBegin);
            for (int i = 0; i < chunkCount; i++)
            {
                chunk.FirstStringIndex = bin.ReadInt32();
                chunk.FirstStringID = bin.ReadInt32();
                chunk.LastStringID = bin.ReadInt32();

                chunk.ReadEntries(bin, entries);
            }

            IsModified = false;
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            var Chunks = CalculateChunks();

            bin.BigEndian = Header.IsBigEndian;

            bin.Write((ushort)0);
            bin.Write(Header.UnkFlag01);
            bin.Write(Header.UnkFlag02);

            bin.Placeholder("FileSize");

            bin.Write(Header.UnkFlag03);

            if (Header.IsBigEndian)
                bin.Write(FMGHeader.ENDIAN_FLAG_BIG);
            else
                bin.Write(FMGHeader.ENDIAN_FLAG_LITTLE);

            bin.Write(Header.UnkFlag04);
            bin.Write(Header.UnkFlag05);

            bin.Write(Chunks.Count);

            bin.Write(entries.Count);

            bin.Placeholder("StringsBeginPointer");

            bin.Write(0); //PAD

            bin.Label("ChunksBeginOffset");

            foreach (var chunk in Chunks)
            {
                bin.Write(chunk.StartIndex);
                bin.Write(chunk.StartID);
                bin.Write(chunk.EndID);
            }

            bin.PointToHere("StringsBeginPointer");

            bin.Label("StringsBeginOffset");

            bin.Position += (entries.Count * 4);

            var stringOffsetList = new List<int>();

            foreach (var kvp in entries)
            {
                string entryStringCheck = kvp.Value.Trim();

                if (entryStringCheck == NullString)
                {
                    stringOffsetList.Add(0);
                }
                else
                {
                    stringOffsetList.Add((int)bin.Position);

                    if (entryStringCheck == EmptyString)
                        bin.WriteStringUnicode(string.Empty, terminate: true);
                    else
                        bin.WriteStringUnicode(kvp.Value, terminate: true);

                }
            }

            //At the very end of all the strings, place the file end padding:
            bin.Write((ushort)0); //PAD

            //Since we reached max length, might as well go fill in the file size:
            bin.Replace("FileSize", (int)bin.Length);

            bin.Goto("StringsBeginOffset");

            for (int i = 0; i < stringOffsetList.Count; i++)
            {
                bin.Write(stringOffsetList[i]);
            }

            bin.Position = bin.Length;
        }

        #region IDictionary
        public ICollection<int> Keys => ((IDictionary<int, string>)entries).Keys;

        public ICollection<string> Values => ((IDictionary<int, string>)entries).Values;

        public int Count => ((IDictionary<int, string>)entries).Count;

        public bool IsReadOnly => ((IDictionary<int, string>)entries).IsReadOnly;

        public string this[int key] { get => ((IDictionary<int, string>)entries)[key]; set => ((IDictionary<int, string>)entries)[key] = value; }


        public bool ContainsKey(int key)
        {
            return ((IDictionary<int, string>)entries).ContainsKey(key);
        }

        public void Add(int key, string value)
        {
            ((IDictionary<int, string>)entries).Add(key, value);
        }

        public bool Remove(int key)
        {
            return ((IDictionary<int, string>)entries).Remove(key);
        }

        public bool TryGetValue(int key, out string value)
        {
            return ((IDictionary<int, string>)entries).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<int, string> item)
        {
            ((IDictionary<int, string>)entries).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<int, string>)entries).Clear();
        }

        public bool Contains(KeyValuePair<int, string> item)
        {
            return ((IDictionary<int, string>)entries).Contains(item);
        }

        public void CopyTo(KeyValuePair<int, string>[] array, int arrayIndex)
        {
            ((IDictionary<int, string>)entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<int, string> item)
        {
            return ((IDictionary<int, string>)entries).Remove(item);
        }

        public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
        {
            return ((IDictionary<int, string>)entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<int, string>)entries).GetEnumerator();
        }
        #endregion
    }
}
