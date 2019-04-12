using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Wind Area")]
public class MSBSWindAreaRegion : MSBSRegion
{
    public void SetRegion(MSBS.Region.WindArea region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.WindArea Serialize(GameObject parent)
    {
        var region = new MSBS.Region.WindArea();
        _Serialize(region, parent);
        return region;
    }

}
