using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public class MsbModelList : IList<MsbModelBase>
    {
        public List<MsbModelMapPiece> MapPieces { get; set; }
            = new List<MsbModelMapPiece>();

        public List<MsbModelObject> Objects { get; set; }
            = new List<MsbModelObject>();

        public List<MsbModelCharacter> Characters { get; set; }
            = new List<MsbModelCharacter>();

        public List<MsbModelPlayer> Players { get; set; }
            = new List<MsbModelPlayer>();

        public List<MsbModelCollision> Collisions { get; set; }
            = new List<MsbModelCollision>();

        public List<MsbModelNavimesh> Navimeshes { get; set; }
            = new List<MsbModelNavimesh>();

        public IList<MsbModelBase> GlobalList =>
            MapPieces.Cast<MsbModelBase>()
            .Concat(Objects)
            .Concat(Characters)
            .Concat(Players)
            .Concat(Collisions)
            .Concat(Navimeshes)
            .ToList();

        public int Count => GlobalList.Count;

        public bool IsReadOnly => GlobalList.IsReadOnly;

        public MsbModelBase this[int index] { get => GlobalList[index]; set => GlobalList[index] = value; }

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
                throw new Exception($"MSB Region \"{name}\" does not exist!");
            }
            else if (matchCount > 1)
            {
                throw new Exception($"More than one MSB Region found named \"{name}\"!");
            }
            return GlobalList.IndexOf(matches.First());
        }

        public string NameOf(int index)
        {
            if (index == -1)
                return "";
            else if (index >= GlobalList.Count)
                return $"[INVALID MODEL INDEX: {index}]";

            return GlobalList[index].Name;
        }

        public int GetNextIndex(ModelParamSubtype type)
        {
            var modelsOfType = GlobalList.Where(x => x.ModelType == type).OrderBy(x => x.Index);
            if (!modelsOfType.Any())
            {
                return 0;
            }
            else
            {
                return modelsOfType.Last().Index + 1;
            }
        }

        public int IndexOf(MsbModelBase item)
        {
            return GlobalList.IndexOf(item);
        }

        public void Insert(int index, MsbModelBase item)
        {
            GlobalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            GlobalList.RemoveAt(index);
        }

        public void Add(MsbModelBase item)
        {
            GlobalList.Add(item);
        }

        public void Clear()
        {
            GlobalList.Clear();
        }

        public bool Contains(MsbModelBase item)
        {
            return GlobalList.Contains(item);
        }

        public void CopyTo(MsbModelBase[] array, int arrayIndex)
        {
            GlobalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(MsbModelBase item)
        {
            return GlobalList.Remove(item);
        }

        public IEnumerator<MsbModelBase> GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }
    }
}
