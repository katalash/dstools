using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Muffling Portal")]
public class MSBSMufflingPortal : MSBSRegion
{
    public int UnkT00;

    public void SetRegion(MSBS.Region.MufflingPortal region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public MSBS.Region.MufflingPortal Serialize(GameObject parent)
    {
        var region = new MSBS.Region.MufflingPortal();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
