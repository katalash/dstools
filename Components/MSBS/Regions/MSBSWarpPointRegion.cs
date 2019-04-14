using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Warp Point")]
public class MSBSWarpPointRegion : MSBSRegion
{
    public void SetRegion(MSBS.Region.WarpPoint region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.WarpPoint Serialize(GameObject parent)
    {
        var region = new MSBS.Region.WarpPoint();
        _Serialize(region, parent);
        return region;
    }
}
