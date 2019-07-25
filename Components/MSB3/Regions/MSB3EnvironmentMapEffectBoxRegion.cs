using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Environment Effect Box")]
public class MSB3EnvironmentEffectBoxRegion : MSB3Region
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Compare;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool UnkT08;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte UnkT09;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT0A;

    public override void SetRegion(MSB3.Region bregion)
    {
        var region = (MSB3.Region.EnvironmentMapEffectBox)bregion;
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        Compare = region.Compare;
        UnkT08 = region.UnkT08;
        UnkT09 = region.UnkT09;
        UnkT0A = region.UnkT0A;
    }

    public override MSB3.Region Serialize(GameObject parent)
    {
        var region = new MSB3.Region.EnvironmentMapEffectBox(parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.Compare = Compare;
        region.UnkT08 = UnkT08;
        region.UnkT09 = UnkT09;
        region.UnkT0A = UnkT0A;
        return region;
    }
}
