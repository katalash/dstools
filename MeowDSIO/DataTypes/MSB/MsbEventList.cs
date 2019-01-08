using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public class MsbEventList : IList<MsbEventBase>
    {
        public List<MsbEventLight> Lights { get; set; }
            = new List<MsbEventLight>();
        public List<MsbEventSound> Sounds { get; set; }
            = new List<MsbEventSound>();
        public List<MsbEventSFX> SFXs { get; set; }
            = new List<MsbEventSFX>();
        public List<MsbEventWindSFX> WindSFXs { get; set; }
            = new List<MsbEventWindSFX>();
        public List<MsbEventTreasure> Treasures { get; set; }
            = new List<MsbEventTreasure>();
        public List<MsbEventGenerator> Generators { get; set; }
            = new List<MsbEventGenerator>();
        public List<MsbEventBloodMsg> BloodMessages { get; set; }
            = new List<MsbEventBloodMsg>();
        public List<MsbEventObjAct> ObjActs { get; set; }
            = new List<MsbEventObjAct>();
        public List<MsbEventSpawnPoint> SpawnPoints { get; set; }
            = new List<MsbEventSpawnPoint>();
        public List<MsbEventMapOffset> MapOffsets { get; set; }
            = new List<MsbEventMapOffset>();
        public List<MsbEventNavimesh> Navimeshes { get; set; }
            = new List<MsbEventNavimesh>();
        public List<MsbEventEnvironment> EnvLightMapSpot { get; set; }
            = new List<MsbEventEnvironment>();
        public List<MsbEventNpcWorldInvitation> NpcWorldInvitations { get; set; } 
            = new List<MsbEventNpcWorldInvitation>();

        private void CheckIndexDictRegister(Dictionary<int, MsbEventBase> indexDict, MsbEventBase thing)
        {
            if (indexDict.ContainsKey(thing.EventIndex))
                throw new InvalidDataException($"Two events found with {nameof(thing.EventIndex)} == {thing.EventIndex} in this MSB!");
            else
                indexDict.Add(thing.EventIndex, thing);
        }

        public IList<MsbEventBase> GlobalList => Lights.Cast<MsbEventBase>()
            .Concat(Sounds)
            .Concat(SFXs)
            .Concat(WindSFXs)
            .Concat(Treasures)
            .Concat(Generators)
            .Concat(BloodMessages)
            .Concat(ObjActs)
            .Concat(SpawnPoints)
            .Concat(MapOffsets)
            .Concat(Navimeshes)
            .Concat(EnvLightMapSpot)
            .Concat(NpcWorldInvitations)
            .ToList();

        public int Count => GlobalList.Count;

        public bool IsReadOnly => GlobalList.IsReadOnly;

        public MsbEventBase this[int index] { get => GlobalList[index]; set => GlobalList[index] = value; }

        public string NameOf(int index)
        {
            if (index == -1)
                return "";
            else if (index >= GlobalList.Count)
                return $"[INVALID GLOBAL EVENT INDEX: {index}]";

            return GlobalList[index].Name;
        }

        public string EnvLightMapSpotNameOf(int index)
        {
            if (index == -1)
                return "";
            else if (index >= EnvLightMapSpot.Count)
                return $"[INVALID LOCAL ENVIRONMENT EVENT INDEX: {index}]";

            return EnvLightMapSpot[index].Name;
        }

        public int GetNextIndex()
        {
            if (GlobalList.Count == 0)
            {
                return 0;
            }
            return GlobalList.OrderBy(x => x.EventIndex).Last().EventIndex + 1;
        }

        public int GetNextIndex(EventParamSubtype type)
        {
            var eventOfType = GlobalList.Where(x => x.Type == type).OrderBy(x => x.Index);
            if (!eventOfType.Any())
            {
                return 0;
            }
            else
            {
                return eventOfType.Last().Index + 1;
            }
        }

        public int EnvLightMapSpotIndexOf(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return -1;
            }
            var matches = EnvLightMapSpot.Where(x => x.Name == name);
            var matchCount = matches.Count();
            if (matchCount == 0)
            {
                throw new Exception($"MSB LightMapSpot Event \"{name}\" does not exist!");
            }
            else if (matchCount > 1)
            {
                throw new Exception($"More than one MSB LightMapSpot Event found named \"{name}\"!");
            }
            return EnvLightMapSpot.IndexOf(matches.First());
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
                throw new Exception($"MSB Event \"{name}\" does not exist!");
            }
            else if (matchCount > 1)
            {
                throw new Exception($"More than one MSB Event found named \"{name}\"!");
            }
            return GlobalList.IndexOf(matches.First());
        }

        public int IndexOf(MsbEventBase item)
        {
            return GlobalList.IndexOf(item);
        }

        public void Insert(int index, MsbEventBase item)
        {
            GlobalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            GlobalList.RemoveAt(index);
        }

        public void Add(MsbEventBase item)
        {
            GlobalList.Add(item);
        }

        public void Clear()
        {
            GlobalList.Clear();
        }

        public bool Contains(MsbEventBase item)
        {
            return GlobalList.Contains(item);
        }

        public void CopyTo(MsbEventBase[] array, int arrayIndex)
        {
            GlobalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(MsbEventBase item)
        {
            return GlobalList.Remove(item);
        }

        public IEnumerator<MsbEventBase> GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GlobalList.GetEnumerator();
        }
    }
}
