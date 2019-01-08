using MeowDSIO.DataTypes.BND;
using MeowDSIO.DataTypes.FMGBND;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataFiles
{
    public class MSGBND : DataFile, IDictionary<FmgType, FMG>
    {
        public BNDHeader Header { get; set; } = new BNDHeader();
        public Dictionary<FmgType, FMG> FMGs { get; set; } = new Dictionary<FmgType, FMG>();

        protected override void Read(DSBinaryReader bin, IProgress<(int, int)> prog)
        {
            var bnd = bin.ReadAsDataFile<BND>(FilePath ?? VirtualUri);
            Header = bnd.Header;
            FMGs = new Dictionary<FmgType, FMG>();
            foreach (var entry in bnd)
            {
                var fmgType = (FmgType)entry.ID;
                FMGs[fmgType] = entry.ReadDataAs<FMG>();
            }
        }

        protected override void Write(DSBinaryWriter bin, IProgress<(int, int)> prog)
        {
            var bnd = new BND();
            bnd.Header = Header;
            foreach (var kvp in FMGs)
            {
                var entry = new BNDEntry((int)kvp.Key, kvp.Key.ToString(), DataFile.SaveAsBytes(kvp.Value, kvp.Key.ToString()));
            }
            bin.WriteDataFile(bnd, FilePath ?? VirtualUri);
        }

        #region IDictionary

        public FMG this[FmgType key] { get => ((IDictionary<FmgType, FMG>)FMGs)[key]; set => ((IDictionary<FmgType, FMG>)FMGs)[key] = value; }

        public ICollection<FmgType> Keys => ((IDictionary<FmgType, FMG>)FMGs).Keys;

        public ICollection<FMG> Values => ((IDictionary<FmgType, FMG>)FMGs).Values;

        public int Count => ((IDictionary<FmgType, FMG>)FMGs).Count;

        public bool IsReadOnly => ((IDictionary<FmgType, FMG>)FMGs).IsReadOnly;

        public void Add(FmgType key, FMG value)
        {
            ((IDictionary<FmgType, FMG>)FMGs).Add(key, value);
        }

        public void Add(KeyValuePair<FmgType, FMG> item)
        {
            ((IDictionary<FmgType, FMG>)FMGs).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<FmgType, FMG>)FMGs).Clear();
        }

        public bool Contains(KeyValuePair<FmgType, FMG> item)
        {
            return ((IDictionary<FmgType, FMG>)FMGs).Contains(item);
        }

        public bool ContainsKey(FmgType key)
        {
            return ((IDictionary<FmgType, FMG>)FMGs).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<FmgType, FMG>[] array, int arrayIndex)
        {
            ((IDictionary<FmgType, FMG>)FMGs).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<FmgType, FMG>> GetEnumerator()
        {
            return ((IDictionary<FmgType, FMG>)FMGs).GetEnumerator();
        }

        public bool Remove(FmgType key)
        {
            return ((IDictionary<FmgType, FMG>)FMGs).Remove(key);
        }

        public bool Remove(KeyValuePair<FmgType, FMG> item)
        {
            return ((IDictionary<FmgType, FMG>)FMGs).Remove(item);
        }

        public bool TryGetValue(FmgType key, out FMG value)
        {
            return ((IDictionary<FmgType, FMG>)FMGs).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<FmgType, FMG>)FMGs).GetEnumerator();
        }

        #endregion

    }
}
