using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3SFXRegion : MSB3Region
{
    /// <summary>
    /// The ID of the .fxr file to play in this region.
    /// </summary>
    public int FFXID;

    /// <summary>
    /// If true, the effect is off by default until enabled by event scripts.
    /// </summary>
    public bool StartDisabled;

    public void SetRegion(MSB3.Region.SFX region)
    {
        setBaseRegion(region);
        FFXID = region.FFXID;
        StartDisabled = region.StartDisabled;
    }
}
