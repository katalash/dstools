using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Region0")]
public class MSBSRegion0Region : MSBSRegion
{
    public void SetRegion(MSBS.Region.Region0 region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.Region0 Serialize(GameObject parent)
    {
        var region = new MSBS.Region.Region0();
        _Serialize(region, parent);
        return region;
    }
}
