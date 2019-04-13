using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Activation Area")]
public class MSBSActivationAreaRegion : MSBSRegion
{
    public void SetRegion(MSBS.Region.ActivationArea region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.ActivationArea Serialize(GameObject parent)
    {
        var region = new MSBS.Region.ActivationArea();
        _Serialize(region, parent);
        return region;
    }
}
