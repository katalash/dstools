using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.Exceptions
{
    public class DSWriteException : DSIOException
    {
        public DSWriteException(DSBinaryWriter bin, string message)
            : base(DSIOExceptionType.Write, message, bin.FileName, bin.Position)
        {

        }
    }
}
