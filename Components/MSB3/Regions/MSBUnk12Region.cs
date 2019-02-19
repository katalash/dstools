using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3Unk12Region : MSB3Region
{
    public void SetRegion(MSB3.Region.Unk12 region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.Unk12 Serialize(GameObject parent)
    {
        var part = new MSB3.Region.Unk12(ID, parent.name);
        _Serialize(part, parent);
        return part;
    }
}
