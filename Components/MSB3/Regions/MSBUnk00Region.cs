using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3Unk00Region : MSB3Region
{
    public void SetRegion(MSB3.Region.Unk00 region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.Unk00 Serialize(GameObject parent)
    {
        var part = new MSB3.Region.Unk00(ID, parent.name);
        _Serialize(part, parent);
        return part;
    }
}
