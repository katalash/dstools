using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Environment Map Effect Box")]
public class MSBSEnvironmentEffectBoxRegion : MSBSRegion
{

    public float UnkT00;
    public float Compare;
    public byte UnkT08;
    public byte UnkT09;
    public short UnkT0A;
    public int UnkT24;
    public float UnkT28;
    public float UnkT2C;

    public void SetRegion(MSBS.Region.EnvironmentMapEffectBox region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        Compare = region.Compare;
        UnkT08 = region.UnkT08;
        UnkT09 = region.UnkT09;
        UnkT0A = region.UnkT0A;
        UnkT24 = region.UnkT24;
        UnkT28 = region.UnkT28;
        UnkT2C = region.UnkT2C;
    }

    public MSBS.Region.EnvironmentMapEffectBox Serialize(GameObject parent)
    {
        var region = new MSBS.Region.EnvironmentMapEffectBox();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.Compare = Compare;
        region.UnkT08 = UnkT08;
        region.UnkT09 = UnkT09;
        region.UnkT0A = UnkT0A;
        region.UnkT24 = UnkT24;
        region.UnkT28 = UnkT28;
        region.UnkT2C = UnkT2C;
        return region;
    }
}
