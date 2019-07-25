using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Muffling Portal")]
public class MSB3MufflingPortal : MSB3Region
{
    public override void SetRegion(MSB3.Region bregion)
    {
        var region = (MSB3.Region.MufflingPortal)bregion;
        setBaseRegion(region);
    }

    public override MSB3.Region Serialize(GameObject parent)
    {
        var region = new MSB3.Region.MufflingPortal(parent.name);
        _Serialize(region, parent);
        return region;
    }
}
