using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Unk 00")]
public class MSB3Unk00Region : MSB3Region
{
    public void SetRegion(MSB3.Region.Unk00 region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.Unk00 Serialize(GameObject parent)
    {
        var part = new MSB3.Region.Unk00(parent.name);
        _Serialize(part, parent);
        return part;
    }
}
