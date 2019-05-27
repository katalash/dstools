using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Other")]
public class MSBSOtherRegion : MSBSRegion
{
    public void SetRegion(MSBS.Region.Other region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.Other Serialize(GameObject parent)
    {
        var region = new MSBS.Region.Other();
        _Serialize(region, parent);
        return region;
    }
}
