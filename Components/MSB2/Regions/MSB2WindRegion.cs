using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/Wind SFX")]
public class MSB2WindRegion : MSB2Region
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT04;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT08;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT0C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT14;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT18;

    public override void SetRegion(MSB2.Region bregion)
    {
        var region = (MSB2.Region.Wind)bregion;
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        UnkT04 = region.UnkT04;
        UnkT08 = region.UnkT08;
        UnkT0C = region.UnkT0C;
        UnkT10 = region.UnkT10;
        UnkT14 = region.UnkT14;
        UnkT18 = region.UnkT18;
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var region = new MSB2.Region.Wind(parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.UnkT04 = UnkT04;
        region.UnkT08 = UnkT08;
        region.UnkT0C = UnkT0C;
        region.UnkT10 = UnkT10;
        region.UnkT14 = UnkT14;
        region.UnkT18 = UnkT18;
        return region;
    }
}
