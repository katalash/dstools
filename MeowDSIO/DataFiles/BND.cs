using MeowDSIO.DataTypes.BND;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class BND : DataFile, IDisposable, IList<BNDEntry>
    {
        public List<BNDEntry> Entries = new List<BNDEntry>();
        public BNDHeader Header { get; set; } = new BNDHeader();

        public void AddEntry(int id, string name, byte[] data)
        {
            Entries.Add(new BNDEntry(id, name, data));
        }

        public BNDEntry GetFirstEntryWithIDAndName(int id, string name, bool ignoreCase = false)
        {
            try
            {
                return Entries.First(x => x.ID == id && 
                    (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public IEnumerable<BNDEntry> GetAllEntriesWithIDAndName(int id, string name, bool ignoreCase = false)
        {
            return Entries.Where(x => x.ID == id && 
                (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
        }

        public BNDEntry GetFirstEntryWithName(string name, bool ignoreCase = false)
        {
            try
            {
                return Entries.First(x => (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public IEnumerable<BNDEntry> GetAllEntriesWithName(string name, bool ignoreCase = false)
        {
            return Entries.Where(x => (ignoreCase ? x.Name.ToUpper() : x.Name) == (ignoreCase ? name.ToUpper() : name));
        }

        public BNDEntry GetFirstEntryWithID(int id)
        {
            try
            {
                return Entries.First(x => x.ID == id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        
        public IEnumerable<BNDEntry> GetAllEntriesWithID(int id)
        {
            return Entries.Where(x => x.ID == id);
        }

        public IEnumerable<BNDEntry> GetAllEntriesWithinIDRange(int? minID, int? maxID)
        {
            return Entries.Where(x => x.ID >= (minID ?? int.MinValue) && x.ID <= (maxID ?? int.MaxValue));
        }


        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            string bndVersionString = bin.ReadStringAscii(BNDHeader.BndVersion_ByteLength);
            if (Enum.TryParse(bndVersionString, out BndVersion parsedBndVersion))
            {
                Header.BndVersion = parsedBndVersion;
            }
            else
            {
                throw new Exception($"Unable to parse BND version: '{bndVersionString}'.");
            }

            if (Header.BndVersion == BndVersion.BND3)
            {
                Header.Signature = bin.ReadStringAscii(BNDHeader.Signature_ByteLength);

                bin.BigEndian = false;

                Header.Format = bin.ReadByte();
                Header.IsBigEndian_Maybe = bin.ReadByte($"{nameof(Header)}.{nameof(Header.IsBigEndian_Maybe)}", 0, 1) == 1;
                Header.IsPS3_Maybe = bin.ReadByte($"{nameof(Header)}.{nameof(Header.IsPS3_Maybe)}", 0, 1) == 1;
                Header.UnkFlag01 = bin.ReadByte($"{nameof(Header)}.{nameof(Header.UnkFlag01)}", 0);

                bin.BigEndian = Header.IsBigEndian_Maybe;

                int fileCount = bin.ReadInt32();

                bin.ReadInt32(); //Names end offset

                Header.UnknownBytes01 = bin.ReadBytes(BNDHeader.UnknownBytes01_Length);

                var e = new BNDEntryHeaderBuffer();

                Entries.Clear();

                int prog_currentbyte = 0;
                int prog_numbytes = (int)bin.Length;

                for (int i = 0; i < fileCount; i++)
                {
                    e.Reset();

                    e.UnkFlag1 = bin.ReadByte();

                    if (e.UnkFlag1 == 0xC0)
                    {
                        e.IsCompressed = true;
                    }

                    //Blank bytes
                    bin.ReadByte();
                    bin.ReadByte();
                    bin.ReadByte();

                    e.CompressedFileSize = bin.ReadInt32();
                    e.FileOffset = bin.ReadInt32();
                    e.FileID = bin.ReadInt32();
                    e.FileNameOffset = bin.ReadInt32();

                    if (Header.Format == 0x74 || 
                        Header.Format == 0x54 || 
                        Header.Format == 0x2E || 
                        Header.Format == 0x64)
                    {
                        e.UncompressedFileSize = bin.ReadInt32();
                    }

                    Entries.Add(e.GetEntry(bin));

                    prog_currentbyte += e.CompressedFileSize;
                    prog?.Report((prog_currentbyte, prog_numbytes));
                }
            }
            else if (Header.BndVersion == BndVersion.BND4)
            {
                //throw new NotImplementedException("BND4 not finished yet.");

                Header.BND4_Unknown1 = bin.ReadBytes(BNDHeader.BND4_Unknown1_Length);
                int fileCount = bin.ReadInt32();
                Header.BND4_Unknown2 = bin.ReadBytes(BNDHeader.BND4_Unknown2_Length);
                Header.Signature = bin.ReadStringAscii(BNDHeader.Signature_ByteLength);
                long entrySize = bin.ReadInt64();
                //Header.BND4_Unknown3 = bin.ReadBytes(BNDHeader.BND4_Unknown3_Length);
                long dataOffset = bin.ReadInt64();
                //Header.BND4_Unknown4 = bin.ReadBytes(BNDHeader.BND4_Unknown4_Length);
                Header.BND4_IsUnicode = bin.ReadBoolean();
                Header.BND4_Unknown5 = bin.ReadBytes(BNDHeader.BND4_Unknown5_Length);

                for (int i = 0; i < fileCount; i++)
                {
                    Header.BND4_Padding = bin.ReadUInt64();

                    long? entryUnknown1 = null;

                    long entryID = i;
                    long entryDataOffset = -1;

                    long entryDataSize = bin.ReadInt64();

                    if (entrySize == 36)
                    {
                        entryUnknown1 = bin.ReadInt64();
                        entryDataOffset = bin.ReadInt32();
                        entryID = bin.ReadInt32();
                    }
                    else
                    {
                        entryDataOffset = bin.ReadInt32();
                    }

                    int entryNameOffset = bin.ReadInt32();

                    string entryName = null;
                    byte[] entryData = null;

                    bin.StepIn(entryDataOffset);
                    {
                        entryData = bin.ReadBytes((int)entryDataSize);
                    }
                    bin.StepOut();

                    bin.StepIn(entryNameOffset);
                    {
                        if (Header.BND4_IsUnicode)
                        {
                            entryName = bin.ReadStringUnicode();
                        }
                        else
                        {
                            entryName = bin.ReadStringShiftJIS();
                        }
                    }
                    bin.StepOut();

                    var newEntry = new BNDEntry((int)entryID, entryName, entryData);

                    if (entryUnknown1.HasValue)
                    {
                        newEntry.BND4_Unknown1 = entryUnknown1.Value;
                    }

                    Entries.Add(newEntry);
                }
            }

            
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            bin.WriteStringAscii(Header.BndVersion.ToString(), BNDHeader.BndVersion_ByteLength);

            if (Header.BndVersion == BndVersion.BND3)
            {
                bin.WriteStringAscii(Header.Signature, BNDHeader.Signature_ByteLength);

                bin.BigEndian = false;

                bin.Write(Header.Format);
                bin.Write(Header.IsBigEndian_Maybe);
                bin.Write(Header.IsPS3_Maybe);
                bin.Write(Header.UnkFlag01);

                bin.BigEndian = Header.IsBigEndian_Maybe;

                bin.Write(Entries.Count);

                var OFF_NameEndOffset = bin.Position;
                bin.Placeholder(); //Placeholder for name end offset

                bin.Write(Header.UnknownBytes01, BNDHeader.UnknownBytes01_Length);

                var OFF_EntryHeaders = bin.Position;

                const int ProgEst_Name = 0x20;
                const int ProgEst_Header = 0x14;

                int prog_cur = 0;
                //Rough estimation of the size of the headers (for both passes)
                int prog_max = Entries.Count * (ProgEst_Header * 2);

                //Rough estimation of name size (only for formats with names)
                if (Header.Format != 0x00)
                {
                    prog_max += (Entries.Count + ProgEst_Name);
                }

                //Add the actual data size
                foreach (var e in Entries)
                    prog_max += e.Size;

                for (int i = 0; i < Entries.Count; i++)
                {
                    if (Entries[i].UnkFlag1.HasValue)
                    {
                        bin.Write(Entries[i].UnkFlag1.Value);
                    }
                    else
                    {
                        if (Header.Format == 0x74 || Header.Format == 0x64)
                        {
                            if (Entries[i].IsCompressed)
                                bin.Write((byte)0xC0);
                            else
                                bin.Write((byte)0x40);
                        }
                        else if (Header.Format == 0x54 ||
                            Header.Format == 0x60 ||
                            Header.Format == 0x70 ||
                            Header.Format == 0xE0 ||
                            Header.Format == 0xF0)
                        {
                            bin.Write((byte)0x40);
                        }
                        else if (Header.Format == 0x0E ||
                            Header.Format == 0x2E)
                        {
                            bin.Write((byte)0x20);
                        }
                        else
                        {
                            //TODO: Think of a good way to go about this...?
                            bin.Write((byte)0x40);
                        }
                    }

                    bin.Write((byte)0);
                    bin.Write((byte)0);
                    bin.Write((byte)0);

                    //Write compressed size
                    bin.Write(Entries[i].Size);
                    bin.Placeholder(); //Placeholder for data offset

                    bin.Write(Entries[i].ID);
                    bin.Placeholder(); //Placeholder for name offset

                    if (Header.Format == 0x74 ||
                        Header.Format == 0x54 ||
                        Header.Format == 0x2E ||
                        Header.Format == 0x64)
                    {
                        //Write actual size
                        bin.Write(Entries[i].Size);
                    }

                    prog_cur += ProgEst_Header;
                    prog?.Report((prog_cur, prog_max));
                }

                var OFF_Names = bin.Position;

                var nameOffsets = new List<int>();
                if (Header.Format != 0x00)
                {
                    for (int i = 0; i < Entries.Count; i++)
                    {
                        nameOffsets.Add((int)bin.Position);
                        bin.WriteStringShiftJIS(Entries[i].Name, true);

                        prog_cur += ProgEst_Name;
                        prog?.Report((prog_cur, prog_max));
                    }
                }

                var OFF_AfterNames = bin.Position;

                bin.Pad(0x10);

                var fileOffsets = new List<int>();
                for (int i = 0; i < Entries.Count; i++)
                {
                    fileOffsets.Add((int)bin.Position);
                    bin.Write(Entries[i].GetBytes());
                    if (i < Entries.Count - 1) //Do not include padding after very last entry.
                    {
                        bin.Pad(0x10);
                    }
                    prog_cur += Entries[i].Size;
                    prog?.Report((prog_cur, prog_max));
                }

                bin.Position = OFF_EntryHeaders;
                for (int i = 0; i < Entries.Count; i++)
                {
                    bin.Position += 4; //UnkFlag1 and 3 empty bytes

                    bin.Position += 4; //Compressed Size
                    bin.Write(fileOffsets[i]);

                    if (Header.Format != 0x00)
                    {
                        bin.Position += 4; //ID
                        bin.Write(nameOffsets[i]);
                    }

                    if (Header.Format == 0x74 ||
                        Header.Format == 0x54 ||
                        Header.Format == 0x2E ||
                        Header.Format == 0x64)
                    {
                        bin.Position += 4; //Uncompressed Size
                    }

                    prog_cur += ProgEst_Header;
                    prog?.Report((prog_cur, prog_max));
                }

                bin.Position = OFF_NameEndOffset;
                bin.Write((int)OFF_AfterNames);
            }
            else if (Header.BndVersion == BndVersion.BND4)
            {
                throw new NotImplementedException("BND4 not finished yet.");
            }

            bin.Position = bin.Length;
        }

        #region IList

        public int Count => ((IList<BNDEntry>)Entries).Count;
        public bool IsReadOnly => ((IList<BNDEntry>)Entries).IsReadOnly;
        public BNDEntry this[int index] { get => ((IList<BNDEntry>)Entries)[index]; set => ((IList<BNDEntry>)Entries)[index] = value; }

        public int IndexOf(BNDEntry item)
        {
            return ((IList<BNDEntry>)Entries).IndexOf(item);
        }

        public void Insert(int index, BNDEntry item)
        {
            ((IList<BNDEntry>)Entries).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<BNDEntry>)Entries).RemoveAt(index);
        }

        public void Add(BNDEntry item)
        {
            ((IList<BNDEntry>)Entries).Add(item);
        }

        public void Clear()
        {
            ((IList<BNDEntry>)Entries).Clear();
        }

        public bool Contains(BNDEntry item)
        {
            return ((IList<BNDEntry>)Entries).Contains(item);
        }

        public void CopyTo(BNDEntry[] array, int arrayIndex)
        {
            ((IList<BNDEntry>)Entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(BNDEntry item)
        {
            return ((IList<BNDEntry>)Entries).Remove(item);
        }

        public IEnumerator<BNDEntry> GetEnumerator()
        {
            return ((IList<BNDEntry>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<BNDEntry>)Entries).GetEnumerator();
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            foreach (var e in Entries)
            {
                e?.Dispose();
            }
        }

        
        #endregion
    }
}
