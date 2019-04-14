using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Invasion Point")]
public class MSBSInvasionPointRegion : MSBSRegion
{
    public int UnkT00;

    public void SetRegion(MSBS.Region.InvasionPoint region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public MSBS.Region.InvasionPoint Serialize(GameObject parent)
    {
        var region = new MSBS.Region.InvasionPoint();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
