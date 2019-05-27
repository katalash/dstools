using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Activation Area")]
public class MSB3ActivationAreaRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.ActivationArea region)
    {
        setBaseRegion(region);
    }

    public MSB3.Region.ActivationArea Serialize(GameObject parent)
    {
        var part = new MSB3.Region.ActivationArea(parent.name);
        _Serialize(part, parent);
        return part;
    }
}
