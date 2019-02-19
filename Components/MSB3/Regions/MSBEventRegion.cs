using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3EventRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.Event region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.Event Serialize(GameObject parent)
    {
        var region = new MSB3.Region.Event(ID, parent.name);
        _Serialize(region, parent);
        return region;
    }
}
