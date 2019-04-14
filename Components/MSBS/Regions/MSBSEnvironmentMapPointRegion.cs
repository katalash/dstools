using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Environment Map Point")]
public class MSBSEnvironmentMapPointRegion : MSBSRegion
{

    public float UnkT00;
    public int UnkT04;
    public int UnkT0C;
    public float UnkT10;
    public float UnkT14;
    public int UnkT18;
    public int UnkT1C;
    public int UnkT20;
    public int UnkT24;
    public int UnkT28;

    public void SetRegion(MSBS.Region.EnvironmentMapPoint region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        UnkT04 = region.UnkT04;
        UnkT0C = region.UnkT0C;
        UnkT10 = region.UnkT10;
        UnkT14 = region.UnkT14;
        UnkT18 = region.UnkT18;
        UnkT1C = region.UnkT1C;
        UnkT20 = region.UnkT20;
        UnkT24 = region.UnkT24;
        UnkT28 = region.UnkT28;
    }

    public MSBS.Region.EnvironmentMapPoint Serialize(GameObject parent)
    {
        var region = new MSBS.Region.EnvironmentMapPoint();
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.UnkT04 = UnkT04;
        region.UnkT0C = UnkT0C;
        region.UnkT10 = UnkT10;
        region.UnkT14 = UnkT14;
        region.UnkT18 = UnkT18;
        region.UnkT1C = UnkT1C;
        region.UnkT20 = UnkT20;
        region.UnkT24 = UnkT24;
        region.UnkT28 = UnkT28;
        return region;
    }
}
