using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    public abstract class MsbStruct
    {
        public void Write(DSBinaryWriter bin)
        {
            bin.StartMsbStruct();
            {
                InternalWrite(bin);
            }
            bin.EndMsbStruct();
        }

        public void Read(DSBinaryReader bin)
        {
            bin.StartMsbStruct();
            {
                InternalRead(bin);
            }
            bin.EndMsbStruct();
        }

        //RUN bin.StartMsbStruct() FIRST
        protected abstract void InternalWrite(DSBinaryWriter bin);

        //RUN bin.StartMsbStruct() FIRST
        protected abstract void InternalRead(DSBinaryReader bin);

        public MsbStruct Clone()
        {
            return (MsbStruct)this.MemberwiseClone();
        }
    }
}
