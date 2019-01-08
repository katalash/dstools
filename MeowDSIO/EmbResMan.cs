using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public static class EmbResMan
    {
        public const string ResourceNamePrefix = "MeowDSIO.EmbeddedResource.";

        public static Assembly ThisAssembly;
        public static readonly string[] ResourceNames;

        static EmbResMan()
        {
            ThisAssembly = typeof(EmbResMan).Assembly;

            ResourceNames = ThisAssembly.GetManifestResourceNames()
                .Select(x => x.Substring(ResourceNamePrefix.Length))
                .ToArray();
        }

        public static string GetAbsoluteResourceName(string resourceName)
            => ResourceNamePrefix + resourceName;

        public static Stream GetStream(string resourceName)
            => ThisAssembly.GetManifestResourceStream(GetAbsoluteResourceName(resourceName));

        public static TStreamReader GetStreamReader<TStreamReader>(string resourceName,
            Func<Stream, TStreamReader> funcGetStreamReader)
            where TStreamReader : class, IDisposable
        {
            using (var stream = GetStream(resourceName))
            {
                if (stream != null)
                    return funcGetStreamReader(stream);
                else
                    return null;
            }
        }

        public static TData GetData<TStreamReader, TData>(string resourceName,
            Func<Stream, TStreamReader> funcGetStreamReader,
            Func<TStreamReader, TData> funcReadData)
            where TStreamReader : class, IDisposable
            where TData : class
        {
            using (var streamReader = GetStreamReader(resourceName, funcGetStreamReader))
            {
                if (streamReader != null)
                    return funcReadData(streamReader);
                else
                    return null;
            }
        }
    }
}
