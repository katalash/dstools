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

    public void SetRegion(MSB3.Region.InvasionPoint region)
    {
        setBaseRegion(region);
        Priority = region.Priority;
    }

    public MSB3.Region.InvasionPoint Serialize(GameObject parent)
    {
        var region = new MSB3.Region.InvasionPoint(parent.name);
        _Serialize(region, parent);
        region.Priority = Priority;
        return region;
    }
}
