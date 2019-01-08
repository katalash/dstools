using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

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
}
