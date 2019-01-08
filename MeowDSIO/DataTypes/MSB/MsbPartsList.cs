using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public class MsbPartsList : IList<MsbPartsBase>
    {
        public List<MsbPartsMapPiece> MapPieces { get; set; }
            = new List<MsbPartsMapPiece>();
        public List<MsbPartsObject> Objects { get; set; }
            = new List<MsbPartsObject>();
        public List<MsbPartsNPC> NPCs { get; set; }
            = new List<MsbPartsNPC>();
        public List<MsbPartsPlayer> Players { get; set; }
            = new List<MsbPartsPlayer>();
        public List<MsbPartsHit> Hits { get; set; }
            = new List<MsbPartsHit>();
        public List<MsbPartsNavimesh> Navimeshes { get; set; }
            = new List<MsbPartsNavimesh>();
        public List<MsbPartsObjectDummy> DummyObjects { get; set; }
            = new List<MsbPartsObjectDummy>();
        public List<MsbPartsNPCDummy> DummyNPCs { get; set; }
            = new List<MsbPartsNPCDummy>();
        public List<MsbPartsConnectHit> ConnectHits { get; set; }
            = new List<MsbPartsConnectHit>();

        private void CheckIndexDictRegister(List<MsbPartsBase> indexDict, MsbPartsBase thing)
        {
            indexDict.Add(thing);
        }

        public IList<MsbPartsBase> GlobalList => MapPieces.Cast<MsbPartsBase>()
            .Concat(Objects)
            .Concat(NPCs)
            .Concat(Players)
            .Concat(Hits)
            .Concat(Navimeshes)
            .Concat(DummyObjects)
            .Concat(DummyNPCs)
            .Concat(ConnectHits)
            .ToList();

        public int Count => GlobalList.Count;

        public bool IsReadOnly => GlobalList.IsReadOnly;

        public MsbPartsBase this[int index] { get => GlobalList[index]; set => GlobalList[index] = value; }

        public string NameOf(int index)
        {
            if (index == -1)
                return "";
            else if (index >= GlobalList.Count)
                return $"[INVALID PART INDEX: {index}]";

            return GlobalList[index].Name;
        }

        public int GetNextIndex(PartsParamSubtype type)
        {
            var partOfType = GlobalList.Where(x => x.Type == type).OrderBy(x => x.Index);
            if (!partOfType.Any())
            {
                return 0;
            }
            else
            {
                return partOfType.Last().Index + 1;
            }
        }

        public int IndexOf(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return -1;
            }
            var matches = GlobalList.Where(x => x.Name == name);
            var matchCount = matches.Count();
            if (matchCount == 0)
            {
                throw new Exception($"MSB Part \"{name}\" does not exist!");
            }
            else if (matchCount > 1)
            {
                throw new Exception($"More than one MSB Part found named \"{name}\"!");
            }
            return GlobalList.IndexOf(matches.First());
        }

        public int IndexOf(MsbPartsBase item)
        {
            return GlobalList.IndexOf(item);
        }

        public void Insert(int index, MsbPartsBase item)
        {
            GlobalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            GlobalList.RemoveAt(index);
        }

        public void Add(MsbPartsBase item)
        {
            GlobalList.Add(item);
        }

        public void Clear()
        {
            GlobalList.Clear();
        }

        public bool Contains(MsbPartsBase item)
        {
            return GlobalList.Contains(item);
        }

        public void CopyTo(MsbPartsBase[] array, int arrayIndex)
        {
            GlobalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(MsbPartsBase item)
        {
            return GlobalList.Remove(item);
        }

        public IEnumerator<MsbPartsBase> GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }
    }
}
