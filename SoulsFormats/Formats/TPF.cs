using System.Collections.Generic;
using System;

namespace SoulsFormats
{
    /// <summary>
    /// A multi-file DDS container used in DS1, DSR, DS2, DS3, DeS, BB, and NB.
    /// </summary>
    public partial class TPF : SoulsFile<TPF>
    {
        /// <summary>
        /// The textures contained within this TPF.
        /// </summary>
        public List<Texture> Textures;

        /// <summary>
        /// The platform this TPF will be used on.
        /// </summary>
        public TPFPlatform Platform;

        /// <summary>
        /// Indicates encoding used for texture names.
        /// </summary>
        public byte Encoding;

        /// <summary>
        /// Unknown.
        /// </summary>
        public byte Flag2;

        /// <summary>
        /// Creates an empty TPF configured for DS3.
        /// </summary>
        public TPF()
        {
            Textures = new List<Texture>();
            Platform = TPFPlatform.PC;
            Encoding = 1;
            Flag2 = 3;
        }

        /// <summary>
        /// Returns true if the data appears to be a TPF.
        /// </summary>
        internal override bool Is(BinaryReaderEx br)
        {
            string magic = br.GetASCII(0, 4);
            return magic == "TPF\0";
        }

        /// <summary>
        /// Reads TPF data from a BinaryReaderEx.
        /// </summary>
        internal override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("TPF\0");
            br.BigEndian = br.GetByte(0xC) == 2;

            int totalFileSize = br.ReadInt32();
            int fileCount = br.ReadInt32();

            Platform = br.ReadEnum8<TPFPlatform>();
            Flag2 = br.AssertByte(1, 2, 3);
            Encoding = br.AssertByte(0, 1, 2);
            br.AssertByte(0);

            Textures = new List<Texture>();
            for (int i = 0; i < fileCount; i++)
            {
                Textures.Add(new Texture(br, Platform, Encoding));
            }
        }

        /// <summary>
        /// Writes TPF data to a BinaryWriterEx.
        /// </summary>
        internal override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;
            bw.WriteASCII("TPF\0");
            bw.ReserveInt32("DataSize");
            bw.WriteInt32(Textures.Count);
            bw.WriteByte((byte)Platform);
            bw.WriteByte(Flag2);
            bw.WriteByte(Encoding);
            bw.WriteByte(0);

            for (int i = 0; i < Textures.Count; i++)
            {
                Textures[i].Write(bw, i, Platform);
            }
            bw.Pad(0x10);

            for (int i = 0; i < Textures.Count; i++)
            {
                Texture texture = Textures[i];
                bw.FillInt32($"FileName{i}", (int)bw.Position);
                if (Encoding == 1)
                    bw.WriteUTF16(texture.Name, true);
                else if (Encoding == 0 || Encoding == 2)
                    bw.WriteShiftJIS(texture.Name, true);
            }

            int dataStart = (int)bw.Position;
            for (int i = 0; i < Textures.Count; i++)
            {
                Texture texture = Textures[i];
                if (texture.Bytes.Length > 0)
                    bw.Pad(0x10);

                bw.FillInt32($"FileData{i}", (int)bw.Position);

                byte[] bytes = texture.Bytes;
                if (texture.Flags1 == 2 || texture.Flags2 == 3)
                    bytes = DCX.Compress(bytes, DCX.Type.ACEREDGE);
                bw.FillInt32($"FileSize{i}", bytes.Length);
                bw.WriteBytes(bytes);
            }
            bw.FillInt32("DataSize", (int)bw.Position - dataStart);
        }

        public void ConvertPS4ToPC()
        {
            if (Platform != TPFPlatform.PS4)
            {
                return;
            }
            Platform = TPFPlatform.PC;
            foreach (Texture tex in Textures)
            {
                tex.ConsoleToPC(TPFPlatform.PS4);
            }
        }

        public void ConvertPS3ToPC()
        {
            if (Platform != TPFPlatform.PS3)
            {
                return;
            }
            Platform = TPFPlatform.PC;
            foreach (Texture tex in Textures)
            {
                tex.ConsoleToPC(TPFPlatform.PS3);
            }
        }

        /// <summary>
        /// A DDS texture in a TPF container.
        /// </summary>
        public class Texture
        {
            /// <summary>
            /// The name of the texture; should not include a path or extension.
            /// </summary>
            public string Name;

            /// <summary>
            /// Indicates format of the texture.
            /// </summary>
            public byte Format;

            /// <summary>
            /// Whether this texture is a cubemap.
            /// </summary>
            public bool Cubemap;

            /// <summary>
            /// Number of mipmap levels in this texture.
            /// </summary>
            public byte Mipmaps;

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Flags1;

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Flags2;

            /// <summary>
            /// The raw data of the texture.
            /// </summary>
            public byte[] Bytes;

            /// <summary>
            /// Extended metadata present in headerless console TPF textures.
            /// </summary>
            public TexHeader Header;

            /// <summary>
            /// Create a new PC Texture with the specified information; Cubemap and Mipmaps are determined based on bytes.
            /// </summary>
            public Texture(string name, byte format, byte flags1, int flags2, byte[] bytes)
            {
                Name = name;
                Format = format;
                Flags1 = flags1;
                Flags2 = flags2;
                Bytes = bytes;
                Header = null;

                DDS dds = new DDS(bytes);
                // DDSCAPS2_CUBEMAP
                Cubemap = (dds.dwCaps2 & 0x200) != 0;
                Mipmaps = (byte)dds.dwMipMapCount;
            }

            internal Texture(BinaryReaderEx br, TPFPlatform platform, byte encoding)
            {
                int fileOffset = br.ReadInt32();
                int fileSize = br.ReadInt32();

                Format = br.ReadByte();
                Cubemap = br.ReadBoolean();
                Mipmaps = br.ReadByte();
                Flags1 = br.AssertByte(0, 1, 2, 3);

                int nameOffset = 0;
                if (platform == TPFPlatform.PC)
                {
                    Header = null;
                    nameOffset = br.ReadInt32();
                    Flags2 = br.AssertInt32(0, 1);
                }
                else if (platform == TPFPlatform.PS3)
                {
                    Header = new TexHeader();
                    Header.Width = br.ReadInt16();
                    Header.Height = br.ReadInt16();
                    Header.Unk1 = br.ReadInt32();
                    Header.Unk2 = br.AssertInt32(0, 0xAAE4);
                    nameOffset = br.ReadInt32();
                    Flags2 = br.AssertInt32(0, 1);
                }
                else if (platform == TPFPlatform.PS4 || platform == TPFPlatform.Xbone)
                {
                    Header = new TexHeader();
                    Header.Width = br.ReadInt16();
                    Header.Height = br.ReadInt16();

                    Header.TextureCount = br.AssertByte(1, 6);
                    br.AssertByte(0);
                    br.AssertByte(0);
                    br.AssertByte(0);

                    Header.Unk2 = br.AssertInt32(0xD);
                    nameOffset = br.ReadInt32();
                    Flags2 = br.AssertInt32(0, 1);
                    Header.DXGIFormat = br.ReadInt32();
                }

                Bytes = br.GetBytes(fileOffset, fileSize);
                if (Flags1 == 2 || Flags1 == 3)
                    Bytes = DCX.Decompress(Bytes);

                if (encoding == 1)
                    Name = br.GetUTF16(nameOffset);
                else if (encoding == 0 || encoding == 2)
                    Name = br.GetShiftJIS(nameOffset);
            }

            internal void Write(BinaryWriterEx bw, int index, TPFPlatform platform)
            {
                if (platform == TPFPlatform.PC)
                {
                    DDS dds = new DDS(Bytes);
                    // DDSCAPS2_CUBEMAP
                    Cubemap = (dds.dwCaps2 & 0x200) != 0;
                    Mipmaps = (byte)dds.dwMipMapCount;
                }

                bw.ReserveInt32($"FileData{index}");
                bw.ReserveInt32($"FileSize{index}");

                bw.WriteByte(Format);
                bw.WriteBoolean(Cubemap);
                bw.WriteByte(Mipmaps);
                bw.WriteByte(Flags1);

                if (platform == TPFPlatform.PC)
                {
                    bw.ReserveInt32($"FileName{index}");
                    bw.WriteInt32(Flags2);
                }
                else if (platform == TPFPlatform.PS3)
                {
                    bw.WriteInt16(Header.Width);
                    bw.WriteInt16(Header.Height);
                    bw.WriteInt32(Header.Unk1);
                    bw.WriteInt32(Header.Unk2);
                    bw.ReserveInt32($"FileName{index}");
                    bw.WriteInt32(Flags2);
                }
                else if (platform == TPFPlatform.PS4 || platform == TPFPlatform.Xbone)
                {
                    bw.WriteInt16(Header.Width);
                    bw.WriteInt16(Header.Height);

                    bw.WriteByte(Header.TextureCount);
                    bw.WriteByte(0);
                    bw.WriteByte(0);
                    bw.WriteByte(0);

                    bw.WriteInt32(Header.Unk2);
                    bw.ReserveInt32($"FileName{index}");
                    bw.WriteInt32(Flags2);
                    bw.WriteInt32(Header.DXGIFormat);
                }
            }

            private static void MapBlockPosition(int x, int y, int w, int bx, out int mx, out int my)
            {
                int num1 = bx / 2;
                int num2 = x / bx;
                int num3 = y / num1;
                int num4 = x % bx;
                int num5 = y % num1;
                int num6 = w / bx;
                int num7 = 2 * num6;
                int num8 = num2 + num3 * num6;
                int num9 = num8 % num7;
                int num10 = num9 / 2 + num9 % 2 * num6;
                int num11 = num8 / num7 * num7 + num10;
                int num12 = num11 % num6;
                int num13 = num11 / num6;

                mx = num12 * bx + num4;
                my = num13 * num1 + num5;
            }

            // <summary>
            // Convert the internal format to PC tpf format
            // </summary>
            public void ConsoleToPC(TPFPlatform source)
            {
                // Need to create a DDS Header
                BinaryWriterEx bw = new BinaryWriterEx(false);
                bw.WriteASCII("DDS ");
                bw.WriteInt32(124);
                bw.WriteUInt32(659463); // Flags
                bw.WriteUInt32((uint)Header.Height);
                bw.WriteUInt32((uint)Header.Width);
                bw.WriteUInt32(((uint)Header.Width * (uint)Header.Height) / 2); // Dummy pitch size
                bw.WriteUInt32(1); // Depth
                if (source == TPFPlatform.PS3)
                {
                    // DeS sometimes has mipmap count set to 0 :trashcat:
                    if (Mipmaps == 0)
                    {
                        var dim = Math.Max(Header.Width, Header.Height);
                        while (dim >= 1)
                        {
                            Mipmaps++;
                            dim >>= 1;
                        }
                    }
                }
                bw.WriteUInt32(Mipmaps);
                for (int i = 0; i < 11; i++)
                {
                    bw.WriteInt32(0);
                }
                // Pixel format
                bw.WriteInt32(32);
                bw.WriteInt32(4); // Flags (compressed)
                bool writeExtendedHeader = false;
                if (Header.DXGIFormat == 71 || Header.DXGIFormat == 72 || (source == TPFPlatform.PS3 && (Format == 0 || Format == 1)))
                {
                    bw.WriteASCII("DXT1");
                }
                else if (Header.DXGIFormat == 73 || Header.DXGIFormat == 74 || Header.DXGIFormat == 75 || (source == TPFPlatform.PS3 && (Format == 2 || Format == 3)))
                {
                    bw.WriteASCII("DXT3");
                }
                else if (Header.DXGIFormat == 76 || Header.DXGIFormat == 77 || Header.DXGIFormat == 78 || (source == TPFPlatform.PS3 && (Format == 4 || Format == 5)))
                {
                    bw.WriteASCII("DXT5");
                }
                else if (Header.DXGIFormat == 79 || Header.DXGIFormat == 80 || Header.DXGIFormat == 81)
                {
                    bw.WriteASCII("ATI1");
                }
                else if (Header.DXGIFormat == 82 || Header.DXGIFormat == 83 || Header.DXGIFormat == 84)
                {
                    bw.WriteASCII("ATI2");
                }
                else if (source == TPFPlatform.PS3 && (Format == 9 || Format == 10))
                {
                    bw.WriteASCII("DXT1");
                    Console.WriteLine("ARGB");
                }
                else
                {
                    bw.WriteASCII("DX10");
                    writeExtendedHeader = true;
                }
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);

                bw.WriteUInt32(0x401008); // Caps
                if (Cubemap)
                {
                    bw.WriteUInt32(0xFE00);
                }
                else
                {
                    bw.WriteUInt32(0);
                }
                bw.WriteUInt32(0);
                bw.WriteUInt32(0);
                bw.WriteUInt32(0);

                // DX10 extended header
                if (writeExtendedHeader)
                {
                    bw.WriteInt32(Header.DXGIFormat);
                    bw.WriteUInt32(3); // 2D texture
                    if (Cubemap)
                    {
                        bw.WriteUInt32(0x4);
                    }
                    else
                    {
                        bw.WriteUInt32(0);
                    }
                    bw.WriteUInt32(1); // Array Size
                    bw.WriteUInt32(0); // Misc
                }

                // Next attempt to unswizzle the texture
                byte[] unswizzled = new byte[Bytes.Length];
                for (int i = 0; i < unswizzled.Length; i++)
                {
                    unswizzled[i] = Bytes[i];
                }

                if (source == TPFPlatform.PS4)
                {
                    uint blockSize = 16;
                    if (Header.DXGIFormat == 71 || Header.DXGIFormat == 72 || Header.DXGIFormat == 79 || Header.DXGIFormat == 80 || Header.DXGIFormat == 81)
                    {
                        blockSize = 8;
                    }

                    int mipBase = 0;
                    int mipBaseSrc = 0;
                    for (int miplevel = 0; miplevel < Mipmaps; miplevel++)
                    {
                        uint bytesPerLine = Math.Max((uint)Header.Width >> miplevel, 1) * blockSize / 4;
                        int heightBlock = Math.Max((Header.Height / 4) >> miplevel, 1);
                        int widthBlock = Math.Max((Header.Width / 4) >> miplevel, 1);
                        // Convert swizzled to linear strided

                        int index = 0;
                        for (int y = 0; y < heightBlock; y++)
                        {
                            for (int x = 0; x < widthBlock; x++)
                            {
                                int mx = x;
                                int my = y;
                                if (widthBlock > 1 && heightBlock > 1)
                                {
                                    MapBlockPosition(x, y, widthBlock, 2, out mx, out my);
                                }

                                if (widthBlock > 2 && heightBlock > 2)
                                {
                                    MapBlockPosition(mx, my, widthBlock, 4, out mx, out my);
                                }

                                if (widthBlock > 4 && heightBlock > 4)
                                {
                                    MapBlockPosition(mx, my, widthBlock, 8, out mx, out my);
                                }

                                int destinationIndex = (int)blockSize * (my * widthBlock + mx);
                                for (int i = 0; i < blockSize; i++)
                                {
                                    unswizzled[mipBase + destinationIndex + i] = Bytes[mipBaseSrc + index];
                                    index += 1;
                                }
                            }
                        }

                        mipBase += index;
                        if (index < 512)
                        {
                            mipBaseSrc += 512;
                        }
                        else
                        {
                            mipBaseSrc += index;
                        }
                    }
                }

                // Append the rest of the original texture and update
                bw.WriteBytes(unswizzled);
                Bytes = bw.FinishBytes();
            }

            /// <summary>
            /// Returns the name of this texture.
            /// </summary>
            public override string ToString()
            {
                return Name;
            }

            /// <summary>
            /// Metadata for headerless textures used in console versions.
            /// </summary>
            public class TexHeader
            {
                /// <summary>
                /// Width of the texture, in pixels.
                /// </summary>
                public short Width;

                /// <summary>
                /// Height of the texture, in pixels.
                /// </summary>
                public short Height;

                /// <summary>
                /// Number of textures in the array, either 1 for normal textures or 6 for cubemaps.
                /// </summary>
                public byte TextureCount;

                /// <summary>
                /// Unknown; PS3 only.
                /// </summary>
                public int Unk1;

                /// <summary>
                /// Unknown; 0x0 or 0xAAE4 in DeS, 0xD in DS3.
                /// </summary>
                public int Unk2;

                /// <summary>
                /// Microsoft DXGI_FORMAT.
                /// </summary>
                public int DXGIFormat;
            }
        }

        /// <summary>
        /// The platform of the game a TPF is for.
        /// </summary>
        public enum TPFPlatform : byte
        {
            /// <summary>
            /// Headered DDS with minimal metadata.
            /// </summary>
            PC = 0,

            /// <summary>
            /// Headerless DDS with pre-DX10 metadata.
            /// </summary>
            PS3 = 2,

            /// <summary>
            /// Headerless DDS with DX10 metadata.
            /// </summary>
            PS4 = 4,

            /// <summary>
            /// Headerless DDS with DX10 metadata.
            /// </summary>
            Xbone = 5,
        }
    }
}
