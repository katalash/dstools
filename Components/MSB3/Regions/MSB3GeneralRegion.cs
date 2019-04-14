using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/General")]
public class MSB3GeneralRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.General region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.General Serialize(GameObject parent)
    {
        var region = new MSB3.Region.General(ID, parent.name);
        _Serialize(region, parent);
        return region;
    }
}
