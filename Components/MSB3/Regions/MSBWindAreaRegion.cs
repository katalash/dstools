using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3WindAreaRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.WindArea region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.WindArea Serialize(GameObject parent)
    {
        var part = new MSB3.Region.WindArea(ID, parent.name);
        _Serialize(part, parent);
        return part;
    }
}
