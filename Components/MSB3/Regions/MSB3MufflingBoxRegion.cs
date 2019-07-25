using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Muffling Box")]
public class MSB3MufflingBoxRegion : MSB3Region
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    public override void SetRegion(MSB3.Region bregion)
    {
        var region = (MSB3.Region.MufflingBox)bregion;
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public override MSB3.Region Serialize(GameObject parent)
    {
        var region = new MSB3.Region.MufflingBox(parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
