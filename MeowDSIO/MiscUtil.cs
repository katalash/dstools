using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO
{
    public static class MiscUtil
    {
        public const string BAD_REF = "?MeowDSIO_BAD_REFERENCE?";

        //Yes that's a GameBoy Color reference
        public static void PrintDX(string txt, ConsoleColor? foreColor = null, ConsoleColor? backColor = null)
        {
            ConsoleColor? oldForeColor = null, oldBackColor = null;

            if (foreColor.HasValue)
            {
                oldForeColor = Console.ForegroundColor;
                Console.ForegroundColor = foreColor.Value;
            }
                
            if (backColor.HasValue)
            {
                oldBackColor = Console.BackgroundColor;
                Console.BackgroundColor = backColor.Value;
            }

            Console.Write(txt);

            if (foreColor.HasValue)
                Console.ForegroundColor = oldForeColor.Value;
                
            if (backColor.HasValue)
                Console.BackgroundColor = oldBackColor.Value;
        }

        public static void PrintlnDX(string txt, ConsoleColor? foreColor = null, ConsoleColor? backColor = null)
        {
            PrintDX(txt + Environment.NewLine, foreColor, backColor);
        }

        public static bool ConsolePrompYesNo(string question = null)
        {
            if (question != null)
            {
                PrintlnDX($"{question} (y/n)", ConsoleColor.Yellow);
            }

            bool? result = null;
            do
            {
                var k = Console.ReadKey(true).KeyChar;
                if ("Yy".Contains(k))
                {
                    result = true;
                }
                else if ("Nn".Contains(k))
                {
                    result = false;
                }
            }
            while (result == null);

            return result.Value;
        }

        public static int AddTolistAndReturnIndex<T>(List<T> list, T item)
        {
            if (list.Contains(item))
            {
                return list.IndexOf(item);
            }
            else
            {
                list.Add(item);
                return list.Count - 1;
            }
        }

        public static void IterateIndexList<T>(List<int> indexList, List<T> itemListDest, T sourceItem)
        {
            IterateIndexList(indexList, itemListDest, new T[] { sourceItem });
        }

        public static void IterateIndexList<T>(List<int> indexList, List<T> itemListDest, IEnumerable<T> sourceItems)
        {
            foreach (var item in sourceItems)
            {
                indexList.Add(AddTolistAndReturnIndex(itemListDest, item));
            }
        }

        private static readonly char[] _dirSep = new char[] { '\\', '/' };
        public static string GetFileNameWithoutDirectoryOrExtension(string fileName)
        {
            if (fileName.EndsWith("\\") || fileName.EndsWith("/"))
                fileName = fileName.TrimEnd(_dirSep);

            if (fileName.Contains("\\") || fileName.Contains("/"))
                fileName = fileName.Substring(fileName.LastIndexOfAny(_dirSep) + 1);

            if (fileName.Contains("."))
                fileName = fileName.Substring(0, fileName.LastIndexOf('.'));

            return fileName;
        }
    }
}
