using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.Exceptions
{
    public abstract class DSIOException : Exception
    {
        private static string GetDSIOExceptionTypeActionString(DSIOExceptionType dsioExceptionType)
        {
            switch (dsioExceptionType)
            {
                case DSIOExceptionType.Read: return "read";
                case DSIOExceptionType.Write: return "write";
                default: return null;
            }
        }

        private static string GetDSIOExceptionText(
            DSIOExceptionType dsioExceptionType,
            string message,
            string filePath, 
            long fileOffset)
        {
            var sb = new StringBuilder();

            var actionTxt = GetDSIOExceptionTypeActionString(dsioExceptionType);

            sb.AppendLine($"Exception encountered during {actionTxt} operation at offset " + 
                $"0x{fileOffset:X} of file '{filePath}':");

            sb.AppendLine();
            sb.AppendLine(message ?? "[NULL]");


            return sb.ToString();
        }

        public DSIOException(DSIOExceptionType dsioExceptionType, string message, string filePath, long fileOffset)
            : base(GetDSIOExceptionText(dsioExceptionType, message, filePath, fileOffset))
        {
            
        }
    }
}
