using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.LUAGNL
{
    public class StringRef
    {
        public string Value { get; set; } = null;

        public static implicit operator string(StringRef s) => s.Value;
        public static implicit operator StringRef(string s) => new StringRef() { Value = s };
    }
}
