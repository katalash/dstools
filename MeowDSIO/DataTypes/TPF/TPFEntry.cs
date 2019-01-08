using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.TPF
{
    public class TPFEntry : IDisposable
    {
        public string Name { get; set; }
        public uint FlagsA { get; set; }
        public uint FlagsB { get; set; }
        public byte[] DDSBytes { get; set; }

        public TPFEntry()
        {

        }

        public TPFEntry(string Name, uint FlagsA, uint FlagsB, byte[] DDSBytes)
        {
            this.Name = Name;
            this.FlagsA = FlagsA;
            this.FlagsB = FlagsB;
            this.DDSBytes = DDSBytes;
        }

        public int Size => (DDSBytes?.Length ?? 0);

        public byte[] GetBytes()
        {
            return DDSBytes;
        }

        public void SetBytes(byte[] newBytes)
        {
            DDSBytes = newBytes;
        }

        public void Dispose()
        {
            DDSBytes = null;
        }
    }
}
