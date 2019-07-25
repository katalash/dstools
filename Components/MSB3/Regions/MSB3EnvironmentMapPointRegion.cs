using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Environment Map Point")]
public class MSB3EnvironmentMapPointRegion : MSB3Region
{
    /// <summary>
    /// Unknown. Only ever 1 bit set, so probably flags.
    /// </summary>
    public int UnkFlags;

    public override void SetRegion(MSB3.Region bregion)
    {
        var region = (MSB3.Region.EnvironmentMapPoint)bregion;
        setBaseRegion(region);
        UnkFlags = region.UnkFlags;
    }

    public override MSB3.Region Serialize(GameObject parent)
    {
        var region = new MSB3.Region.EnvironmentMapPoint(parent.name);
        _Serialize(region, parent);
        region.UnkFlags = UnkFlags;
        return region;
    }
}
