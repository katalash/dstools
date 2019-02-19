using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3SoundRegion : MSB3Region
{
    /// <summary>
    /// Type of sound in this region; determines mixing behavior like muffling.
    /// </summary>
    public MSB3.Region.Sound.SndType SoundType;

    /// <summary>
    /// ID of the sound to play in this region, or 0 for child regions.
    /// </summary>
    public int SoundID;

    /// <summary>
    /// Names of other Sound regions which extend this one.
    /// </summary>
    public string[] ChildRegionNames;

    public void SetRegion(MSB3.Region.Sound region)
    {
        setBaseRegion(region);
        SoundType = region.SoundType;
        SoundID = region.SoundID;
        ChildRegionNames = region.ChildRegionNames;
    }

    public MSB3.Region.Sound Serialize(GameObject parent)
    {
        var region = new MSB3.Region.Sound(ID, parent.name);
        _Serialize(region, parent);
        region.SoundID = SoundID;
        for (int i = 0; i < 16; i++)
        {
            if (i >= ChildRegionNames.Length)
                break;
            region.ChildRegionNames[i] = (ChildRegionNames[i] == "") ? null : ChildRegionNames[i];
        }
        return region;
    }
}
