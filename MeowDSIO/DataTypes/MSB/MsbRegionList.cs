using MeowDSIO.DataTypes.MSB.POINT_PARAM_ST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public class MsbRegionList : IList<MsbRegionBase>
    {
        public ObservableCollection<MsbRegionPoint> Points { get; set; }
        = new ObservableCollection<MsbRegionPoint>();

        public ObservableCollection<MsbRegionSphere> Spheres { get; set; }
        = new ObservableCollection<MsbRegionSphere>();

        public ObservableCollection<MsbRegionCylinder> Cylinders { get; set; }
        = new ObservableCollection<MsbRegionCylinder>();

        public ObservableCollection<MsbRegionBox> Boxes { get; set; }
        = new ObservableCollection<MsbRegionBox>();

        private void CheckIndexDictRegister(Dictionary<int, MsbRegionBase> indexDict, MsbRegionBase thing)
        {
            if (indexDict.ContainsKey(thing.SolvedIndex))
                throw new InvalidDataException($"Two regions found with {nameof(thing.SolvedIndex)} == {thing.SolvedIndex} in this MSB!");
            else
                indexDict.Add(thing.SolvedIndex, thing);
        }

        public string NameOf(int index)
        {
            if (index == -1)
                return "";
            else if (index >= GlobalList.Count)
                return $"[INVALID REGION INDEX: {index}]";

            return GlobalList[index].Name;
        }

        public IList<MsbRegionBase> GlobalList => Points.Cast<MsbRegionBase>()
            .Concat(Spheres)
            .Concat(Cylinders)
            .Concat(Boxes)
            .OrderBy(x => x.SolvedIndex)
            .ToList();

        public int Count => GlobalList.Count;

        public bool IsReadOnly => GlobalList.IsReadOnly;

        public MsbRegionBase this[int index] { get => GlobalList[index]; set => GlobalList[index] = value; }

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

        public int GetNextIndex()
        {
            var orderedRegions = GlobalList.OrderBy(x => x.SolvedIndex);
            if (!orderedRegions.Any())
            {
                return 0;
            }
            return orderedRegions.Last().SolvedIndex + 1;
        }

        //public int GetNextIndex(PointParamSubtype type)
        //{
        //    var pointOfType = GlobalList.Where(x => x.Type == type).OrderBy(x => x.Index);
        //    if (!pointOfType.Any())
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return pointOfType.Last().Index + 1;
        //    }
        //}

        public int IndexOf(MsbRegionBase item)
        {
            return GlobalList.IndexOf(item);
        }

        public void Insert(int index, MsbRegionBase item)
        {
            GlobalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            GlobalList.RemoveAt(index);
        }

        public void Add(MsbRegionBase item)
        {
            GlobalList.Add(item);
        }

        public void Clear()
        {
            GlobalList.Clear();
        }

        public bool Contains(MsbRegionBase item)
        {
            return GlobalList.Contains(item);
        }

        public void CopyTo(MsbRegionBase[] array, int arrayIndex)
        {
            GlobalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(MsbRegionBase item)
        {
            return GlobalList.Remove(item);
        }

        public IEnumerator<MsbRegionBase> GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }
    }
}
