using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Auto Draw Group")]
public class MSBSAutoDrawGroupRegion : MSBSRegion
{

    public long UnkT00;

    public void SetRegion(MSBS.Region.AutoDrawGroup region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public MSBS.Region.AutoDrawGroup Serialize(GameObject parent)
    {
        var region = new MSBS.Region.AutoDrawGroup();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
