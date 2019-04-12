using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Muffling Box")]
public class MSBSMufflingBoxRegion : MSBSRegion
{

    public int UnkT00;

    public void SetRegion(MSBS.Region.MufflingBox region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public MSBS.Region.MufflingBox Serialize(GameObject parent)
    {
        var region = new MSBS.Region.MufflingBox();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
