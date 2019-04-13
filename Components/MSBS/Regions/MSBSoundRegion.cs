using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Regions/Sound")]
public class MSBSSoundRegion : MSBSRegion
{

    /// <summary>
    /// Type of sound in this region; determines mixing behavior like muffling.
    /// </summary>
    public int SoundType;

    /// <summary>
    /// ID of the sound to play in this region, or 0 for child regions.
    /// </summary>
    public int SoundID;

    /// <summary>
    /// Names of other Sound regions which extend this one.
    /// </summary>
    public string[] ChildRegionNames;

    /// <summary>
    /// Unknown (Something about enable/disable state?)
    /// </summary>
    public int UnkT48;

    public void SetRegion(MSBS.Region.Sound region)
    {
        setBaseRegion(region);
        SoundType = region.SoundType;
        SoundID = region.SoundID;
        ChildRegionNames = region.ChildRegionNames;
        UnkT48 = region.UnkT48;
    }

    public MSBS.Region.Sound Serialize(GameObject parent)
    {
        var region = new MSBS.Region.Sound();
        _Serialize(region, parent);
        region.SoundType = SoundType;
        region.SoundID = SoundID;
        for (int i = 0; i < 16; i++)
        {
            if (i >= ChildRegionNames.Length)
                break;
            region.ChildRegionNames[i] = (ChildRegionNames[i] == "") ? null : ChildRegionNames[i];
        }
        region.UnkT48 = UnkT48;
        return region;
    }
}
