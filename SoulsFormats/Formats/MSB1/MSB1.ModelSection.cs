using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    public partial class MSB1
    {
        /// <summary>
        /// A section containing all the models available to parts in this map.
        /// </summary>
        public class ModelSection : Section<Model>
        {
            internal override string Type => "MODEL_PARAM_ST";

            /// <summary>
            /// Map piece models in this section.
            /// </summary>
            public List<Model> MapPieces;

            /// <summary>
            /// Object models in this section.
            /// </summary>
            public List<Model> Objects;

            /// <summary>
            /// Enemy models in this section.
            /// </summary>
            public List<Model> Enemies;

            /// <summary>
            /// Player models in this section.
            /// </summary>
            public List<Model> Players;

            /// <summary>
            /// Collision models in this section.
            /// </summary>
            public List<Model> Collisions;

            /// <summary>
            /// Navmeshes in this section.
            /// </summary>
            public List<Model> Navmeshes;

            internal ModelSection(BinaryReaderEx br, int unk1) : base(br, unk1)
            {
                MapPieces = new List<Model>();
                Objects = new List<Model>();
                Enemies = new List<Model>();
                Players = new List<Model>();
                Collisions = new List<Model>();
                Navmeshes = new List<Model>();
            }

            /// <summary>
            /// Returns every model in the order they will be written.
            /// </summary>
            public override List<Model> GetEntries()
            {
                return Util.ConcatAll<Model>(
                    MapPieces, Objects, Enemies, Players, Collisions, Navmeshes);
            }

            internal override Model ReadEntry(BinaryReaderEx br)
            {
                ModelType type = br.GetEnum32<ModelType>(br.Position + 4);

                switch (type)
                {
                    case ModelType.MapPiece:
                        var mapPiece = new Model(br);
                        MapPieces.Add(mapPiece);
                        return mapPiece;

                    case ModelType.Object:
                        var obj = new Model(br);
                        Objects.Add(obj);
                        return obj;

                    case ModelType.Enemy:
                        var enemy = new Model(br);
                        Enemies.Add(enemy);
                        return enemy;

                    case ModelType.Player:
                        var player = new Model(br);
                        Players.Add(player);
                        return player;

                    case ModelType.Collision:
                        var collision = new Model(br);
                        Collisions.Add(collision);
                        return collision;

                    case ModelType.Navmesh:
                        var navmesh = new Model(br);
                        Navmeshes.Add(navmesh);
                        return navmesh;

                    default:
                        throw new NotImplementedException($"Unsupported model type: {type}");
                }
            }

            internal override void WriteEntries(BinaryWriterEx bw, List<Model> entries)
            {
                throw new NotImplementedException();
            }
        }

        internal enum ModelType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Player = 4,
            Collision = 5,
            Navmesh = 6
        }

        /// <summary>
        /// A model available for use by parts in this map.
        /// </summary>
        public class Model : Entry
        {
            internal ModelType Type { get; private set; }

            /// <summary>
            /// The name of this model.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// The placeholder used for this model in MapStudio.
            /// </summary>
            public string Placeholder;

            /// <summary>
            /// The ID of this model.
            /// </summary>
            public int ID;

            /// <summary>
            /// The number of parts using this model; recalculated whenever the MSB is written.
            /// </summary>
            public int InstanceCount { get; internal set; }

            internal Model(BinaryReaderEx br)
            {
                long start = br.Position;

                int nameOffset = br.ReadInt32();
                br.AssertUInt32((uint)Type);
                ID = br.ReadInt32();
                int placeholderOffset = br.ReadInt32();
                InstanceCount = br.ReadInt32();
                br.AssertInt32(0);

                Name = br.GetShiftJIS(start + nameOffset);
                Placeholder = br.GetShiftJIS(start + placeholderOffset);
            }

            /// <summary>
            /// Returns the model type and name of this model.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} : {Name}";
            }
        }
    }
}
