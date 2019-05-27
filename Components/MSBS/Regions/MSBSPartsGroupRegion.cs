using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Parts Group Region")]
public class MSBSPartsGroupRegion : MSBSRegion
{

    public long UnkT00;

    public void SetRegion(MSBS.Region.PartsGroup region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public MSBS.Region.PartsGroup Serialize(GameObject parent)
    {
        var region = new MSBS.Region.PartsGroup();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
