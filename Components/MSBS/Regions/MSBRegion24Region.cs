using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Region 24")]
public class MSBSRegion24Region : MSBSRegion
{
    public void SetRegion(MSBS.Region.Region24 region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.Region24 Serialize(GameObject parent)
    {
        var region = new MSBS.Region.Region24();
        _Serialize(region, parent);
        return region;
    }
}
