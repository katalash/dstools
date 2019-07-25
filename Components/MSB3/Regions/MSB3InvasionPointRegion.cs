using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Invasion Point")]
public class MSB3InvasionPointRegion : MSB3Region
{
    /// <summary>
    /// Not sure what this does.
    /// </summary>
    public int Priority;

    public override void SetRegion(MSB3.Region bregion)
    {
        var region = (MSB3.Region.InvasionPoint)bregion;
        setBaseRegion(region);
        Priority = region.Priority;
    }

    public override MSB3.Region Serialize(GameObject parent)
    {
        var region = new MSB3.Region.InvasionPoint(parent.name);
        _Serialize(region, parent);
        region.Priority = Priority;
        return region;
    }
}
