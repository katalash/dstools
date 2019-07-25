using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Wind Area")]
public class MSB3WindAreaRegion : MSB3Region
{
    public override void SetRegion(MSB3.Region region)
    {
        setBaseRegion(region);
    }

    public override MSB3.Region Serialize(GameObject parent)
    {
        var part = new MSB3.Region.WindArea(parent.name);
        _Serialize(part, parent);
        return part;
    }
}
