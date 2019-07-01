namespace PropertyHook
{
    /// <summary>
    /// A dynamic pointer starting from the base address of an array of bytes scanned for in the target process.
    /// </summary>
    public class PHPointerAOBAbsolute : PHPointerAOB
    {
        /// <summary>
        /// Creates a new absolute AOB pointer.
        /// </summary>
        public PHPointerAOBAbsolute(PHook parent, byte?[] aob, params int[] offsets) : base(parent, aob, offsets) { }

        internal override void ScanAOB(AOBScanner scanner)
        {
            AOBResult = scanner.Scan(AOB);
        }
    }
}
