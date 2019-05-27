using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Region 23")]
public class MSBSRegion23Region : MSBSRegion
{

    public long UnkT00;

    public void SetRegion(MSBS.Region.Region23 region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public MSBS.Region.Region23 Serialize(GameObject parent)
    {
        var region = new MSBS.Region.Region23();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
