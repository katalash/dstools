using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3MufflingPortal : MSB3Region
{
    public void SetRegion(MSB3.Region.MufflingPortal region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.MufflingPortal Serialize(GameObject parent)
    {
        var region = new MSB3.Region.MufflingPortal(ID, parent.name);
        _Serialize(region, parent);
        return region;
    }
}
