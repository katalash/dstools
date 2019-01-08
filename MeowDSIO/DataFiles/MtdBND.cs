using MeowDSIO.DataTypes.BND;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class MtdBND : DataFile, IDictionary<string, MTD>
    {
        public BNDHeader Header { get; set; } = new BNDHeader();
        public Dictionary<string, MTD> Entries { get; set; } = new Dictionary<string, MTD>();

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            var bnd = bin.ReadAsDataFile<BND>(FilePath ?? VirtualUri);

            Header = bnd.Header;

            Entries = new Dictionary<string, MTD>();

            foreach (var entry in bnd)
            {
                if (!Entries.ContainsKey(entry.Name))
                    Entries.Add(entry.Name, entry.ReadDataAs<MTD>());
                else
                    Entries[entry.Name] = entry.ReadDataAs<MTD>();
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            var bnd = new BND()
            {
                Header = Header
            };

            int ID = 0;

            foreach (var kvp in Entries)
            {
                bnd.Entries.Add(new BNDEntry(ID++, kvp.Key, DataFile.SaveAsBytes(kvp.Value, kvp.Key)));
            }

            bin.WriteDataFile(bnd, FilePath ?? VirtualUri);
        }

        #region IDictionary
        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, MTD>)Entries).ContainsKey(key);
        }

        public void Add(string key, MTD value)
        {
            ((IDictionary<string, MTD>)Entries).Add(key, value);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, MTD>)Entries).Remove(key);
        }

        public bool TryGetValue(string key, out MTD value)
        {
            return ((IDictionary<string, MTD>)Entries).TryGetValue(key, out value);
        }

        public MTD this[string key] { get => ((IDictionary<string, MTD>)Entries)[key]; set => ((IDictionary<string, MTD>)Entries)[key] = value; }

        public ICollection<string> Keys => ((IDictionary<string, MTD>)Entries).Keys;

        public ICollection<MTD> Values => ((IDictionary<string, MTD>)Entries).Values;

        public void Add(KeyValuePair<string, MTD> item)
        {
            ((IDictionary<string, MTD>)Entries).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<string, MTD>)Entries).Clear();
        }

        public bool Contains(KeyValuePair<string, MTD> item)
        {
            return ((IDictionary<string, MTD>)Entries).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, MTD>[] array, int arrayIndex)
        {
            ((IDictionary<string, MTD>)Entries).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, MTD> item)
        {
            return ((IDictionary<string, MTD>)Entries).Remove(item);
        }

        public int Count => ((IDictionary<string, MTD>)Entries).Count;

        public bool IsReadOnly => ((IDictionary<string, MTD>)Entries).IsReadOnly;

        public IEnumerator<KeyValuePair<string, MTD>> GetEnumerator()
        {
            return ((IDictionary<string, MTD>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, MTD>)Entries).GetEnumerator();
        }
        #endregion
    }
}
