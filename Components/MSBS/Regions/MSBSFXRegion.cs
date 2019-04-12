using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/SFX")]
public class MSBSSFXRegion : MSBSRegion
{

    /// <summary>
    /// The ID of the .fxr file to play in this region.
    /// </summary>
    public int FFXID;

    /// <summary>
    /// If true, the effect is off by default until enabled by event scripts.
    /// </summary>
    public int StartDisabled;

    public int UnkT04 { get; set; }


    public void SetRegion(MSBS.Region.SFX region)
    {
        setBaseRegion(region);
        FFXID = region.FFXID;
        StartDisabled = region.StartDisabled;
        UnkT04 = region.UnkT04;
    }

    public MSBS.Region.SFX Serialize(GameObject parent)
    {
        var region = new MSBS.Region.SFX();
        _Serialize(region, parent);
        region.FFXID = FFXID;
        region.StartDisabled = StartDisabled;
        region.UnkT04 = UnkT04;
        return region;
    }
}
