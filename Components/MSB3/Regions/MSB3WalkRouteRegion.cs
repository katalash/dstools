using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Walk Route")]
public class MSB3WalkRouteRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.WalkRoute region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.WalkRoute Serialize(GameObject parent)
    {
        var part = new MSB3.Region.WalkRoute(ID, parent.name);
        _Serialize(part, parent);
        return part;
    }
}
