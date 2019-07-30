using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/Env Light")]
public class MSB2EnvLightRegion : MSB2Region
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

    public override void SetRegion(MSB2.Region bregion)
    {
        var region = (MSB2.Region.EnvLight)bregion;
        UnkT00 = region.UnkT00;
        UnkT04 = region.UnkT04;
        UnkT08 = region.UnkT08;
        setBaseRegion(region);
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var region = new MSB2.Region.EnvLight(parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.UnkT04 = UnkT04;
        region.UnkT08 = UnkT08;
        return region;
    }
}
