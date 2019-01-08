using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.BND
{
    public class BNDEntry : IDisposable
    {
        public byte? UnkFlag1 { get; set; } = null;
        public bool IsCompressed { get; set; } = false;

        public int ID { get; set; }
        public string Name { get; set; }
        public long? BND4_Unknown1 = null;
        private byte[] Data;

        public BNDEntry(int ID, string Name, byte[] FileBytes)
        {
            this.ID = ID;
            this.Name = Name;
            Data = FileBytes;
        }

        public T ReadDataAs<T>()
            where T : DataFile, new()
        {
            return DataFile.LoadFromBytes<T>(Data, Name, null);
        }

        public T ReadDataAs<T>(IProgress<(int, int)> prog)
            where T : DataFile, new()
        {
            return DataFile.LoadFromBytes<T>(Data, Name, prog);
        }

        public void ReplaceData<T>(T data)
            where T : DataFile, new()
        {
            Data = DataFile.SaveAsBytes(data, Name, null);
        }

        public void ReplaceData<T>(T data, IProgress<(int, int)> prog)
            where T : DataFile, new()
        {
            Data = DataFile.SaveAsBytes(data, Name, prog);
        }

        public int Size => (Data?.Length ?? 0);

        public byte[] GetBytes()
        {
            return Data;
        }

        public void SetBytes(byte[] newBytes)
        {
            Data = newBytes;
        }

        public void Dispose()
        {
            Data = null;
        }
    }
}
