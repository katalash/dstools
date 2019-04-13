using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Event")]
public class MSBSEventRegion : MSBSRegion
{
    public void SetRegion(MSBS.Region.Event region)
    {
        setBaseRegion(region);
    }

    public MSBS.Region.Event Serialize(GameObject parent)
    {
        var region = new MSBS.Region.Event();
        _Serialize(region, parent);
        return region;
    }
}
