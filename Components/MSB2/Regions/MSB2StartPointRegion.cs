using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/Start Point")]
public class MSB2StartPointRegion : MSB2Region
{
    public override void SetRegion(MSB2.Region region)
    {
        setBaseRegion(region);
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var region = new MSB2.Region.StartPoint(parent.name);
        _Serialize(region, parent);
        return region;
    }
}
