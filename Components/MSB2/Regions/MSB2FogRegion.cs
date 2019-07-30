using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/Fog")]
public class MSB2FogRegion : MSB2Region
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT04;

    public override void SetRegion(MSB2.Region bregion)
    {
        var region = (MSB2.Region.Fog)bregion;
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        UnkT04 = region.UnkT04;
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var region = new MSB2.Region.Fog(parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.UnkT04 = UnkT04;
        return region;
    }
}
