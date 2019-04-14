using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Wind SFX")]
public class MSBSWindSFXRegion : MSBSRegion
{

    /// <summary>
    /// ID of an .fxr file.
    /// </summary>
    public int FFXID;

    /// <summary>
    /// Name of a corresponding WindArea region.
    /// </summary>
    public string WindAreaName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT18;


    public void SetRegion(MSBS.Region.WindSFX region)
    {
        setBaseRegion(region);
        FFXID = region.FFXID;
        WindAreaName = region.WindAreaName;
        UnkT18 = region.UnkT18;
    }

    public MSBS.Region.WindSFX Serialize(GameObject parent)
    {
        var region = new MSBS.Region.WindSFX();
        _Serialize(region, parent);
        region.FFXID = FFXID;
        region.WindAreaName = WindAreaName;
        region.UnkT18 = UnkT18;
        return region;
    }
}
