using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB.POINT_PARAM_ST
{
    public class MsbRegionBox : MsbRegionBase
    {
        internal override void DebugPushUnknownFieldReport_Subtype(out string subtypeName, Dictionary<string, object> dict)
        {
            subtypeName = "Box";

            dict.Add(nameof(SUB_CONST_1), SUB_CONST_1);
            dict.Add(nameof(SUB_CONST_2), SUB_CONST_2);
        }

        internal int SUB_CONST_1 { get; set; } = 0;
        internal int SUB_CONST_2 { get; set; } = 0;
        public float WidthX { get; set; } = 1;
        public float DepthZ { get; set; } = 1;
        public float HeightY { get; set; } = 1;
        public int EntityID { get; set; } = -1;

        public MsbRegionBox(MsbRegionList parentList)
        {
            this.Index = parentList.Count;
        }

        internal override (int, int, int) GetOffsetDeltas()
        {
            return (4, 8, 20);
        }

        internal override PointParamSubtype GetSubtypeValue()
        {
            return PointParamSubtype.Boxes;
        }

        protected override void SubtypeRead(DSBinaryReader bin)
        {
            SUB_CONST_1 = bin.ReadInt32();
            SUB_CONST_2 = bin.ReadInt32();
            WidthX = bin.ReadSingle();
            DepthZ = bin.ReadSingle();
            HeightY = bin.ReadSingle();
            EntityID = bin.ReadInt32();
        }

        protected override void SubtypeWrite(DSBinaryWriter bin)
        {
            bin.Write(SUB_CONST_1);
            bin.Write(SUB_CONST_2);
            bin.Write(WidthX);
            bin.Write(DepthZ);
            bin.Write(HeightY);
            bin.Write(EntityID);
        }
    }
}
