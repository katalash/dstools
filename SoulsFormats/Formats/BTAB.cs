using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    /// <summary>
    /// A DS2/DS3 and BB file that specifies coordinates into a lightmap atlas to apply certain light maps to map parts
    /// </summary>
    public class BTAB : SoulsFile<BTAB>
    {
        /// <summary>
        /// Entries in this BTAB.
        /// </summary>
        public List<Entry> Entries;

        internal override bool Is(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        internal override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;

            br.AssertInt32(1);
            br.AssertInt32(0);
            int entryCount = br.ReadInt32();
            int nameSize = br.ReadInt32();
            br.AssertInt32(0);
            // Entry size
            br.AssertInt32(0x28);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);

            long nameStart = br.Position;
            br.Position = nameStart + nameSize;
            Entries = new List<Entry>();
            for (int i = 0; i < entryCount; i++)
                Entries.Add(new Entry(br, nameStart));
        }

        internal override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;

            bw.WriteInt32(1);
            bw.WriteInt32(0);
            bw.WriteInt32(Entries.Count);
            bw.ReserveInt32("NameSize");
            bw.WriteInt32(0);
            bw.WriteInt32(0x28);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);

            long nameStart = bw.Position;
            var nameOffsets = new List<int>();
            foreach (Entry entry in Entries)
            {
                int nameOffset = (int)(bw.Position - nameStart);
                nameOffsets.Add(nameOffset);
                bw.WriteUTF16(entry.MSBPartName, true);
                if (nameOffset % 0x10 != 0)
                {
                    for (int i = 0; i < 0x10 - (nameOffset % 0x10); i++)
                        bw.WriteByte(0);
                }

                int nameOffset2 = (int)(bw.Position - nameStart);
                nameOffsets.Add(nameOffset2);
                bw.WriteUTF16(entry.FLVERMaterialName, true);
                if (nameOffset2 % 0x10 != 0)
                {
                    for (int i = 0; i < 0x10 - (nameOffset2 % 0x10); i++)
                        bw.WriteByte(0);
                }
            }

            bw.FillInt32("NameSize", (int)(bw.Position - nameStart));
            for (int i = 0; i < Entries.Count; i++)
                Entries[i].Write(bw, nameOffsets[i * 2], nameOffsets[i * 2 + 1]);
        }

        /// <summary>
        /// A BTAB entry.
        /// </summary>
        public class Entry
        {
            /// <summary>
            /// The name of the target part defined in the MSB file
            /// </summary>
            public string MSBPartName;

            /// <summary>
            /// The name of a material in the FLVER; not the name of the MTD file itself.
            /// </summary>
            public string FLVERMaterialName;

            /// <summary>
            /// Which atlas texture to use for lightmaps
            /// </summary>
            public int AtlasIndex;

            /// <summary>
            /// Used to scale and translate lightmap uvs to select a particular index for a map part
            /// </summary>
            public Vector2 AtlasOffset;
            public Vector2 AtlasScale;

            internal Entry(BinaryReaderEx br, long nameStart)
            {
                int nameOffset = br.ReadInt32();
                MSBPartName = br.GetUTF16(nameStart + nameOffset);
                br.AssertInt32(0);

                int nameOffset2 = br.ReadInt32();
                FLVERMaterialName = br.GetUTF16(nameStart + nameOffset2);
                br.AssertInt32(0);

                AtlasIndex = br.ReadInt32();
                AtlasOffset = br.ReadVector2();
                AtlasScale = br.ReadVector2();
                br.AssertInt32(0);
            }

            internal void Write(BinaryWriterEx bw, int nameOffset, int nameOffset2)
            {
                bw.WriteInt32(nameOffset);
                bw.WriteInt32(0);
                bw.WriteInt32(nameOffset2);
                bw.WriteInt32(0);
                bw.WriteInt32(AtlasIndex);
                bw.WriteVector2(AtlasOffset);
                bw.WriteVector2(AtlasScale);
                bw.WriteInt32(0);
            }

            /// <summary>
            /// Returns the MSB part name and FLVER material name of the entry.
            /// </summary>
            public override string ToString()
            {
                return $"{MSBPartName} : {FLVERMaterialName}";
            }
        }
    }
}
