using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.Exceptions
{
    public class DSReadException : DSIOException
    {
        public DSReadException(DSBinaryReader bin, string message)
            : base(DSIOExceptionType.Read, message, bin.FileName, bin.Position)
        {

        }
    }
}
