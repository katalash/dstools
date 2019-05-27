using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Muffling Portal")]
public class MSB3MufflingPortal : MSB3Region
{
    public void SetRegion(MSB3.Region.MufflingPortal region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.MufflingPortal Serialize(GameObject parent)
    {
        var region = new MSB3.Region.MufflingPortal(parent.name);
        _Serialize(region, parent);
        return region;
    }
}
