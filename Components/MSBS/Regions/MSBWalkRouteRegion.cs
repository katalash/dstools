using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Walk Route")]
public class MSBSWalkRouteRegion : MSBSRegion
{
    public void SetRegion(MSBS.Region.WalkRoute region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.WalkRoute Serialize(GameObject parent)
    {
        var region = new MSBS.Region.WalkRoute();
        _Serialize(region, parent);
        return region;
    }
}
