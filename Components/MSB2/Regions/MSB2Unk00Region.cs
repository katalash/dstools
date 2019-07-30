using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/Unk 00")]
public class MSB2Unk00Region : MSB2Region
{
    public override void SetRegion(MSB2.Region region)
    {
        setBaseRegion(region);
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var part = new MSB2.Region.Region0(parent.name);
        _Serialize(part, parent);
        return part;
    }
}
