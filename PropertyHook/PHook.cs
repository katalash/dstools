using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace PropertyHook
{
    /// <summary>
    /// An interface to another application's memory that automatically handles hooking, unhooking, and pointer management.
    /// </summary>
    public abstract class PHook
    {
        /// <summary>
        /// Whether the hook is currently attached to a process.
        /// </summary>
        public bool Hooked { get; private set; }

        /// <summary>
        /// The currently attached process, or null if none.
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// Whether the attached process is 32-bit or 64-bit.
        /// </summary>
        public bool Is64Bit { get; private set; }

        /// <summary>
        /// The handle to the attached process, or zero if none.
        /// </summary>
        public IntPtr Handle => Process?.Handle ?? IntPtr.Zero;

        /// <summary>
        /// How often the automatic hooking thread should check for new processes, in milliseconds.
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// The minimum time a process must have been running before hooking is attempted, in milliseconds.
        /// </summary>
        public int MinLifetime { get; set; }

        /// <summary>
        /// Fires immediately after attaching to a new process.
        /// </summary>
        public event EventHandler<PHEventArgs> OnHooked;

        /// <summary>
        /// Fires immediately after detaching from a process.
        /// </summary>
        public event EventHandler<PHEventArgs> OnUnhooked;

        private Func<Process, bool> Selector;
        private List<PHPointerAOB> AOBPointers;
        private Thread RefreshThread;
        private CancellationTokenSource RefreshCancellationSource;

        /// <summary>
        /// Creates a new PHook.
        /// </summary>
        /// <param name="refreshInterval">How often the automatic hooking thread should check for new processes, in milliseconds.</param>
        /// <param name="minLifetime">The minimum time a process must have been running before hooking is attempted, in milliseconds.</param>
        /// <param name="processSelector">A function that determines if a process should be attempted to be hooked.</param>
        public PHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector)
        {
            Selector = processSelector;
            RefreshInterval = refreshInterval;
            MinLifetime = minLifetime;
            AOBPointers = new List<PHPointerAOB>();
            RefreshThread = null;
            RefreshCancellationSource = null;
        }

        /// <summary>
        /// Starts a thread that automatically checks for new processes to hook.
        /// </summary>
        public void Start()
        {
            if (RefreshThread == null)
            {
                RefreshCancellationSource = new CancellationTokenSource();
                var threadStart = new ThreadStart(() => AutoRefresh(RefreshCancellationSource.Token));
                RefreshThread = new Thread(threadStart);
                RefreshThread.IsBackground = true;
                RefreshThread.Start();
            }
        }

        /// <summary>
        /// Stops the automatic hooking thread.
        /// </summary>
        public void Stop()
        {
            if (RefreshThread != null)
            {
                RefreshCancellationSource.Cancel();
                RefreshThread = null;
                RefreshCancellationSource = null;
            }
        }

        private void AutoRefresh(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                Refresh();
                Thread.Sleep(RefreshInterval);
            }
        }

        /// <summary>
        /// Checks for new processes to attach to; either call this manually, or call Start to do it automatically.
        /// </summary>
        public void Refresh()
        {
            if (!Hooked)
            {
                // After hooking, close all the remaining Processes immediately
                bool cleanup = false;
                foreach (Process process in Process.GetProcesses())
                {
                    bool close = false;
                    bool is64Bit = false;
                    try
                    {
                        close = cleanup || !Selector(process) || process.HasExited || (DateTime.Now - process.StartTime).TotalMilliseconds < MinLifetime;
                        if (!close && Environment.Is64BitOperatingSystem)
                        {
                            // This is actually really slow, so only do it if everything else passed
                            if (Kernel32.IsWow64Process(process.Handle, out bool result))
                            {
                                is64Bit = !result;
                                close |= is64Bit && !Environment.Is64BitProcess;
                            }
                            else
                            {
                                close = true;
                            }
                        }
                    }
                    catch (Win32Exception)
                    {
                        close = true;
                    }

                    if (close)
                    {
                        process.Close();
                    }
                    else
                    {
                        cleanup = true;
                        Is64Bit = is64Bit;
                        Process = process;
                        process.EnableRaisingEvents = true;
                        process.Exited += Unhook;

                        if (AOBPointers.Count > 0)
                        {
                            var scanner = new AOBScanner(process);
                            foreach (PHPointerAOB pointer in AOBPointers)
                            {
                                pointer.ScanAOB(scanner);
                            }
                        }

                        Hooked = true;
                        RaiseOnHooked();
                    }
                }
            }
        }

        private void Unhook(object sender, EventArgs e)
        {
            Hooked = false;
            foreach (PHPointerAOB pointer in AOBPointers)
            {
                pointer.DumpAOB();
            }
            Process = null;
            RaiseOnUnhooked();
        }

        /// <summary>
        /// Creates and registers a new relative AOB pointer.
        /// </summary>
        public PHPointer RegisterRelativeAOB(byte?[] aob, int addressOffset, int instructionSize, params int[] offsets)
        {
            var pointer = new PHPointerAOBRelative(this, aob, addressOffset, instructionSize, offsets);
            AOBPointers.Add(pointer);
            return pointer;
        }

        /// <summary>
        /// Create and register a new relative AOB pointer with a CE-style AOB string.
        /// </summary>
        public PHPointer RegisterRelativeAOB(string aob, int addressOffset, int instructionSize, params int[] offsets)
        {
            return RegisterRelativeAOB(AOBScanner.StringToAOB(aob), addressOffset, instructionSize, offsets);
        }

        /// <summary>
        /// Creates and registers a new absolute AOB pointer.
        /// </summary>
        public PHPointer RegisterAbsoluteAOB(byte?[] aob, params int[] offsets)
        {
            var pointer = new PHPointerAOBAbsolute(this, aob, offsets);
            AOBPointers.Add(pointer);
            return pointer;
        }

        /// <summary>
        /// Creates and registers a new absolute AOB pointer with a CE-style AOB string.
        /// </summary>
        public PHPointer RegisterAbsoluteAOB(string aob, params int[] offsets)
        {
            return RegisterAbsoluteAOB(AOBScanner.StringToAOB(aob), offsets);
        }

        /// <summary>
        /// Creates a new base address pointer.
        /// </summary>
        public PHPointer CreateBasePointer(IntPtr baseAddress, params int[] offsets)
        {
            var pointer = new PHPointerBase(this, baseAddress, offsets);
            return pointer;
        }

        /// <summary>
        /// Creates a new child pointer.
        /// </summary>
        public PHPointer CreateChildPointer(PHPointer basePointer, params int[] offsets)
        {
            var pointer = new PHPointerChild(this, basePointer, offsets);
            return pointer;
        }

        /// <summary>
        /// Unregisters an AOB pointer. Returns null.
        /// </summary>
        public PHPointer UnregisterAOBPointer(PHPointerAOB pointer)
        {
            AOBPointers.Remove(pointer);
            return null;
        }

        /// <summary>
        /// Manually rescans all AOB pointers.
        /// </summary>
        public void RescanAOB()
        {
            if (Hooked && AOBPointers.Count > 0)
            {
                var scanner = new AOBScanner(Process);
                foreach (PHPointerAOB pointer in AOBPointers)
                {
                    pointer.ScanAOB(scanner);
                }
            }
        }

        /// <summary>
        /// Allocates a memory region with the given size and permissions.
        /// </summary>
        public IntPtr Allocate(uint size, uint flProtect = Kernel32.PAGE_READWRITE)
        {
            return Kernel32.VirtualAllocEx(Handle, IntPtr.Zero, size, Kernel32.MEM_COMMIT, flProtect);
        }

        /// <summary>
        /// Frees a memory region at the given address. Returns true if successful.
        /// </summary>
        public bool Free(IntPtr address)
        {
            return Kernel32.VirtualFreeEx(Handle, address, 0, Kernel32.MEM_RESET);
        }

        /// <summary>
        /// Starts a thread at the given address and waits for it to complete. Returns execution result.
        /// </summary>
        public int Execute(IntPtr address, uint timeout = 0xFFFFFFFF)
        {
            IntPtr thread = Kernel32.CreateRemoteThread(Handle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);
            int result = Kernel32.WaitForSingleObject(thread, timeout);
            Kernel32.CloseHandle(thread);
            return result;
        }

        /// <summary>
        /// Allocates memory for the given bytes, starts a thread at their address, waits for it to complete, and frees the memory. Returns execution result.
        /// </summary>
        public int Execute(byte[] bytes, uint timeout = 0xFFFFFFFF)
        {
            IntPtr address = Allocate((uint)bytes.Length, Kernel32.PAGE_EXECUTE_READWRITE);
            Kernel32.WriteBytes(Handle, address, bytes);
            int result = Execute(address, timeout);
            Free(address);
            return result;
        }

        private void RaiseOnHooked()
        {
            OnHooked?.Invoke(this, new PHEventArgs(this));
        }

        private void RaiseOnUnhooked()
        {
            OnUnhooked?.Invoke(this, new PHEventArgs(this));
        }
    }
}
