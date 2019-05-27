using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Wind SFX")]
public class MSB3WindSFXRegion : MSB3Region
{
    /// <summary>
    /// ID of an .fxr file.
    /// </summary>
    public int FFXID;

    /// <summary>
    /// Name of a corresponding WindArea region.
    /// </summary>
    public string WindAreaName;

    public void SetRegion(MSB3.Region.WindSFX region)
    {
        setBaseRegion(region);
        FFXID = region.FFXID;
        WindAreaName = region.WindAreaName;
    }

    public MSB3.Region.WindSFX Serialize(GameObject parent)
    {
        var region = new MSB3.Region.WindSFX(parent.name);
        _Serialize(region, parent);
        region.FFXID = FFXID;
        region.WindAreaName = (WindAreaName == "") ? null : WindAreaName;
        return region;
    }
}
