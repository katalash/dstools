using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PropertyHook
{
    internal class AOBScanner
    {
        private List<Kernel32.MEMORY_BASIC_INFORMATION> MemRegions;

        private Dictionary<IntPtr, byte[]> ReadMemory;

        public AOBScanner(Process process)
        {
            MemRegions = new List<Kernel32.MEMORY_BASIC_INFORMATION>();
            IntPtr memRegionAddr = process.MainModule.BaseAddress;
            IntPtr mainModuleEnd = process.MainModule.BaseAddress + process.MainModule.ModuleMemorySize;
            uint queryResult;

            do
            {
                var memInfo = new Kernel32.MEMORY_BASIC_INFORMATION();
                queryResult = Kernel32.VirtualQueryEx(process.Handle, memRegionAddr, out memInfo, (uint)Marshal.SizeOf(memInfo));
                if (queryResult != 0)
                {
                    if ((memInfo.State & Kernel32.MEM_COMMIT) != 0 && (memInfo.Protect & Kernel32.PAGE_GUARD) == 0 && (memInfo.Protect & Kernel32.PAGE_EXECUTE_ANY) != 0)
                        MemRegions.Add(memInfo);
                    memRegionAddr = memInfo.BaseAddress + (int)memInfo.RegionSize;
                }
            } while (queryResult != 0 && (ulong)memRegionAddr < (ulong)mainModuleEnd);

            ReadMemory = new Dictionary<IntPtr, byte[]>();
            foreach (Kernel32.MEMORY_BASIC_INFORMATION memRegion in MemRegions)
                ReadMemory[memRegion.BaseAddress] = Kernel32.ReadBytes(process.Handle, memRegion.BaseAddress, (uint)memRegion.RegionSize);
        }

        public IntPtr Scan(byte?[] aob)
        {
            List<IntPtr> results = new List<IntPtr>();
            foreach (IntPtr baseAddress in ReadMemory.Keys)
            {
                byte[] bytes = ReadMemory[baseAddress];

                for (int i = 0; i < bytes.Length - aob.Length; i++)
                {
                    bool found = true;
                    for (int j = 0; j < aob.Length; j++)
                    {
                        if (aob[j] != null && aob[j] != bytes[i + j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        return baseAddress + i;
                    }
                }
            }

            return IntPtr.Zero;
        }

        public static byte?[] StringToAOB(string text)
        {
            string[] items = text.Split(' ');
            byte?[] aob = new byte?[items.Length];
            for (int i = 0; i < aob.Length; i++)
            {
                string item = items[i];
                if (item == "?")
                    aob[i] = null;
                else
                    aob[i] = byte.Parse(item, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return aob;
        }
    }
}
