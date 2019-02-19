using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBBB
    {
        /// <summary>
        /// A section containing points and volumes for various purposes.
        /// </summary>
        public class PointSection : Section<Region>
        {
            internal override string Type => "POINT_PARAM_ST";

            public List<Region> Regions;

            /// <summary>
            /// The points in this section.
            /// </summary>
            public List<Region.Point> Points;

            /// <summary>
            /// The circles in this section.
            /// </summary>
            public List<Region.Circle> Circles;

            /// <summary>
            /// The spheres in this section.
            /// </summary>
            public List<Region.Sphere> Spheres;

            /// <summary>
            /// The cylinders in this section.
            /// </summary>
            public List<Region.Cylinder> Cylinders;

            /// <summary>
            /// The boxes in this section.
            /// </summary>
            public List<Region.Box> Boxes;

            /// <summary>
            /// Creates a new PointSection with no regions.
            /// </summary>
            public PointSection(int unk1 = 3) : base(unk1)
            {
                Regions = new List<Region>();
                /*Points = new List<Region.Point>();
                Circles = new List<Region.Circle>();
                Spheres = new List<Region.Sphere>();
                Cylinders = new List<Region.Cylinder>();
                Boxes = new List<Region.Box>();*/
            }

            /// <summary>
            /// Returns every region in the order they will be written.
            /// </summary>
            public override List<Region> GetEntries()
            {
                //return SFUtil.ConcatAll<Region>(
                //    Points, Circles, Spheres, Cylinders, Boxes);
                return Regions;
            }

            internal override Region ReadEntry(BinaryReaderEx br)
            {
                RegionType type = br.GetEnum32<RegionType>(br.Position + 0x10);

                switch (type)
                {
                    case RegionType.Point:
                        var point = new Region.Point(br);
                        //Points.Add(point);
                        Regions.Add(point);
                        return point;

                    case RegionType.Circle:
                        var circle = new Region.Circle(br);
                        //Circles.Add(circle);
                        Regions.Add(circle);
                        return circle;

                    case RegionType.Sphere:
                        var sphere = new Region.Sphere(br);
                        //Spheres.Add(sphere);
                        Regions.Add(sphere);
                        return sphere;

                    case RegionType.Cylinder:
                        var cylinder = new Region.Cylinder(br);
                        //Cylinders.Add(cylinder);
                        Regions.Add(cylinder);
                        return cylinder;

                    case RegionType.Box:
                        var box = new Region.Box(br);
                        //Boxes.Add(box);
                        Regions.Add(box);
                        return box;

                    default:
                        throw new NotImplementedException($"Unsupported region type: {type}");
                }
            }

            internal override void WriteEntries(BinaryWriterEx bw, List<Region> entries)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    bw.FillInt64($"Offset{i}", bw.Position);
                    entries[i].Write(bw);
                }
            }

            internal void GetNames(MSBBB msb, Entries entries)
            {
                foreach (Region region in entries.Regions)
                    region.GetNames(msb, entries);
            }

            internal void GetIndices(MSBBB msb, Entries entries)
            {
                foreach (Region region in entries.Regions)
                    region.GetIndices(msb, entries);
            }
        }

        internal enum RegionType : uint
        {
            Point = 0,
            Circle = 1,
            Sphere = 2,
            Cylinder = 3,
            Square = 4,
            Box = 5,
        }

        /// <summary>
        /// A point or volumetric area used for a variety of purposes.
        /// </summary>
        public abstract class Region : Entry
        {
            internal abstract RegionType Type { get; }

            /// <summary>
            /// The name of this region.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// Whether this region has additional type data. The only region type where this actually varies is Sound.
            /// </summary>
            public bool HasTypeData;

            /// <summary>
            /// The ID of this region.
            /// </summary>
            public int ID;

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk2, Unk3, Unk4;

            /// <summary>
            /// Not sure if this is exactly a drawgroup, but it's what makes messages not appear in dark Firelink.
            /// </summary>
            public uint DrawGroup;

            /// <summary>
            /// Center of the region.
            /// </summary>
            public Vector3 Position;

            /// <summary>
            /// Rotation of the region, in degrees.
            /// </summary>
            public Vector3 Rotation;

            private int ActivationPartIndex;
            /// <summary>
            /// Region is inactive unless this part is drawn; null for always active.
            /// </summary>
            public string ActivationPartName;

            /// <summary>
            /// An ID used to identify this region in event scripts.
            /// </summary>
            public int EventEntityID;

            internal Region(int id, string name, bool hasTypeData)
            {
                ID = id;
                Name = name;
                Position = Vector3.Zero;
                Rotation = Vector3.Zero;
                ActivationPartName = null;
                EventEntityID = -1;
                Unk2 = 0;
                Unk3 = 0;
                Unk4 = 0;
                DrawGroup = 0;
                HasTypeData = hasTypeData;
            }

            internal Region(Region clone)
            {
                Name = clone.Name;
                ID = clone.ID;
                Position = clone.Position;
                Rotation = clone.Rotation;
                ActivationPartName = clone.ActivationPartName;
                EventEntityID = clone.EventEntityID;
                Unk2 = clone.Unk2;
                Unk3 = clone.Unk3;
                Unk4 = clone.Unk4;
                DrawGroup = clone.DrawGroup;
                HasTypeData = clone.HasTypeData;
            }

            internal Region(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                br.AssertInt32(0);
                ID = br.ReadInt32();
                br.AssertUInt32((uint)Type);
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Unk2 = br.ReadInt32();

                Name = br.GetUTF16(start + nameOffset);

                long baseDataOffset1 = br.ReadInt64();
                br.StepIn(start + baseDataOffset1);
                Unk3 = br.ReadInt32();
                br.StepOut();

                long baseDataOffset2 = br.AssertInt64(baseDataOffset1 + 4);
                br.StepIn(start + baseDataOffset2);
                Unk4 = br.ReadInt32();
                br.StepOut();

                // This will be 0 for points, but that's fine
                long typeDataOffset = br.ReadInt64();
                if (typeDataOffset != 0)
                {
                    br.StepIn(start + typeDataOffset);
                    ReadSpecific(br);
                    br.StepOut();
                }

                long baseDataOffset3 = br.ReadInt64();

                br.StepIn(start + baseDataOffset3);
                //ActivationPartIndex = br.ReadInt32();
                EventEntityID = br.ReadInt32();

                if (br.Position % 8 != 0)
                    br.AssertInt32(0);

                br.StepOut();
            }

            internal abstract void ReadSpecific(BinaryReaderEx br);

            internal void Write(BinaryWriterEx bw)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(0);
                bw.WriteInt32(ID);
                bw.WriteUInt32((uint)Type);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteInt32(Unk2);

                bw.ReserveInt64("BaseDataOffset1");
                bw.ReserveInt64("BaseDataOffset2");


                bw.ReserveInt64("TypeDataOffset");
                bw.ReserveInt64("BaseDataOffset3");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(Name, true);
                bw.Pad(4);

                bw.FillInt64("BaseDataOffset1", bw.Position - start);
                bw.WriteInt32(Unk3);

                bw.FillInt64("BaseDataOffset2", bw.Position - start);
                bw.WriteInt32(Unk4);
                bw.Pad(8);

                WriteSpecific(bw, start);

                bw.FillInt64("BaseDataOffset3", bw.Position - start);
                bw.WriteInt32(EventEntityID);

                bw.Pad(8);
            }

            internal abstract void WriteSpecific(BinaryWriterEx bw, long start);

            internal virtual void GetNames(MSBBB msb, Entries entries)
            {
                ActivationPartName = GetName(entries.Parts, ActivationPartIndex);
            }

            internal virtual void GetIndices(MSBBB msb, Entries entries)
            {
                ActivationPartIndex = GetIndex(entries.Parts, ActivationPartName);
            }

            /// <summary>
            /// Returns the region type, ID, shape type, and name of this region.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} {ID} : {Name}";
            }

            public class Point : Region
            {
                internal override RegionType Type => RegionType.Point;

                internal Point(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br) { }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", 0);
                }
            }

            public class Circle : Region
            {
                internal override RegionType Type => RegionType.Circle;

                /// <summary>
                /// The radius of the circle.
                /// </summary>
                public float Radius;

                internal Circle(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteSingle(Radius);
                }
            }

            public class Sphere : Region
            {
                internal override RegionType Type => RegionType.Sphere;

                /// <summary>
                /// The radius of the sphere.
                /// </summary>
                public float Radius;

                internal Sphere(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteSingle(Radius);
                }
            }

            public class Cylinder : Region
            {
                internal override RegionType Type => RegionType.Cylinder;

                /// <summary>
                /// The radius of the cylinder.
                /// </summary>
                public float Radius;

                /// <summary>
                /// The height of the cylinder.
                /// </summary>
                public float Height;

                internal Cylinder(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                    Height = br.ReadSingle();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteSingle(Radius);
                    bw.WriteSingle(Height);
                }
            }

            public class Box : Region
            {
                internal override RegionType Type => RegionType.Box;

                /// <summary>
                /// The length of the box.
                /// </summary>
                public float Length;

                /// <summary>
                /// The width of the box.
                /// </summary>
                public float Width;

                /// <summary>
                /// The height of the box.
                /// </summary>
                public float Height;

                internal Box(BinaryReaderEx br) : base(br) { }

                internal override void ReadSpecific(BinaryReaderEx br)
                {
                    Length = br.ReadSingle();
                    Width = br.ReadSingle();
                    Height = br.ReadSingle();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteSingle(Length);
                    bw.WriteSingle(Width);
                    bw.WriteSingle(Height);
                }
            }
        }
    }
}
