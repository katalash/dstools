using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.FMG
{
    public struct FMGChunk
    {
        public readonly int StartIndex;
        public readonly int StartID;
        public readonly int EndID;

        public FMGChunk(int StartIndex, int StartID, int EndID)
        {
            this.StartIndex = StartIndex;
            this.StartID = StartID;
            this.EndID = EndID;
        }
    }
}
