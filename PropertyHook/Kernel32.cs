using System;
using System.Runtime.InteropServices;

namespace PropertyHook
{
    /// <summary>
    /// Provides wrappers for process manipulation via kernel32.dll.
    /// </summary>
    public static class Kernel32
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RESERVE = 0x2000;
        public const uint MEM_RESET = 0x8000;
        public const uint MEM_PHYSICAL = 0x400000;
        public const uint MEM_RESET_UNDO = 0x1000000;
        public const uint MEM_LARGE_PAGES = 0x20000000;

        public const uint PAGE_NOACCESS = 0x1;
        public const uint PAGE_READONLY = 0x2;
        public const uint PAGE_READWRITE = 0x4;
        public const uint PAGE_WRITECOPY = 0x8;
        public const uint PAGE_EXECUTE = 0x10;
        public const uint PAGE_EXECUTE_READ = 0x20;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        public const uint PAGE_GUARD = 0x100;
        public const uint PAGE_NOCACHE = 0x200;
        public const uint PAGE_WRITECOMBINE = 0x400;
        public const uint PAGE_TARGETS_INVALID = 0x40000000;
        public const uint PAGE_TARGETS_NO_UPDATE = 0x4000000;
        public const uint PAGE_EXECUTE_ANY = PAGE_EXECUTE | PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE | PAGE_EXECUTE_WRITECOPY;

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public ulong RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern int WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);


        public static byte[] ReadBytes(IntPtr handle, IntPtr address, uint length)
        {
            byte[] bytes = new byte[length];
            ReadProcessMemory(handle, address, bytes, length, 0);
            return bytes;
        }

        public static bool WriteBytes(IntPtr handle, IntPtr address, byte[] bytes)
        {
            return WriteProcessMemory(handle, address, bytes, (uint)bytes.Length, 0);
        }

        public static IntPtr ReadIntPtr(IntPtr handle, IntPtr address, bool is64bit)
        {
            if (is64bit)
                return (IntPtr)ReadInt64(handle, address);
            else
                return (IntPtr)ReadInt32(handle, address);
        }

        public static bool ReadFlag32(IntPtr handle, IntPtr address, uint mask)
        {
            uint flags = ReadUInt32(handle, address);
            return (flags & mask) != 0;
        }

        public static bool WriteFlag32(IntPtr handle, IntPtr address, uint mask, bool state)
        {
            uint flags = ReadUInt32(handle, address);
            if (state)
                flags |= mask;
            else
                flags &= ~mask;
            return WriteUInt32(handle, address, flags);
        }


        public static sbyte ReadSByte(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 1);
            return (sbyte)bytes[0];
        }

        public static bool WriteSByte(IntPtr handle, IntPtr address, sbyte value)
        {
            // Note: do not BitConverter.GetBytes this, stupid
            return WriteBytes(handle, address, new byte[] { (byte)value });
        }


        public static byte ReadByte(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 1);
            return bytes[0];
        }

        public static bool WriteByte(IntPtr handle, IntPtr address, byte value)
        {
            // Note: do not BitConverter.GetBytes this, stupid
            return WriteBytes(handle, address, new byte[] { value });
        }


        public static bool ReadBoolean(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 1);
            return BitConverter.ToBoolean(bytes, 0);
        }

        public static bool WriteBoolean(IntPtr handle, IntPtr address, bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static short ReadInt16(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 2);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static bool WriteInt16(IntPtr handle, IntPtr address, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static ushort ReadUInt16(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 2);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static bool WriteUInt16(IntPtr handle, IntPtr address, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static int ReadInt32(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static bool WriteInt32(IntPtr handle, IntPtr address, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static uint ReadUInt32(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static bool WriteUInt32(IntPtr handle, IntPtr address, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static long ReadInt64(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static bool WriteInt64(IntPtr handle, IntPtr address, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static ulong ReadUInt64(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 8);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static bool WriteUInt64(IntPtr handle, IntPtr address, ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static float ReadSingle(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static bool WriteSingle(IntPtr handle, IntPtr address, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }


        public static double ReadDouble(IntPtr handle, IntPtr address)
        {
            byte[] bytes = ReadBytes(handle, address, 8);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static bool WriteDouble(IntPtr handle, IntPtr address, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
