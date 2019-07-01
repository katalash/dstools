using System;

namespace PropertyHook
{
    /// <summary>
    /// A dynamic pointer starting from a relative address found after an array of bytes scanned for in the target process.
    /// </summary>
    public class PHPointerAOBRelative : PHPointerAOB
    {
        /// <summary>
        /// The offset of the relative address from the beginning of the AOB.
        /// </summary>
        public int AddressOffset { get; set; }

        /// <summary>
        /// The total size from the beginning of the AOB to the end of the instruction containing the relative address.
        /// </summary>
        public int InstructionSize { get; set; }

        /// <summary>
        /// Creates a new relative AOB pointer.
        /// </summary>
        public PHPointerAOBRelative(PHook parent, byte?[] aob, int addressOffset, int instructionSize, params int[] offsets) : base(parent, aob, offsets)
        {
            AddressOffset = addressOffset;
            InstructionSize = instructionSize;
        }

        internal override void ScanAOB(AOBScanner scanner)
        {
            IntPtr result = scanner.Scan(AOB);
            IntPtr temp = result + AddressOffset;
            uint address = Kernel32.ReadUInt32(Hook.Handle, temp);
            AOBResult = (IntPtr)((ulong)(result + InstructionSize) + address);
        }
    }
}
