using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.LUAINFO
{
    public class ScriptRef
    {
        public string Name { get; set; } = null;
        public byte[] Bytecode { get; set; } = new byte[0];

        public ScriptRef(string Name, byte[] Bytecode)
        {
            this.Name = Name;
            this.Bytecode = Bytecode;
        }
    }
}
