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

    public void SetRegion(MSB3.Region.MufflingBox region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }

    public MSB3.Region.MufflingBox Serialize(GameObject parent)
    {
        var region = new MSB3.Region.MufflingBox(ID, parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        return region;
    }
}
