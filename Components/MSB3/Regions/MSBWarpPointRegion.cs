using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3WarpPointRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.WarpPoint region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.WarpPoint Serialize(GameObject parent)
    {
        var part = new MSB3.Region.WarpPoint(ID, parent.name);
        _Serialize(part, parent);
        return part;
    }
}
