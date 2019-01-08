using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public class LoadAbortedException : Exception
    {
        public LoadAbortedException(string fileName = null)
            : base(fileName != null ? $"Loading of file '{fileName}' aborted by user." : "Loading aborted by user.")
        {

        }
    }
}
