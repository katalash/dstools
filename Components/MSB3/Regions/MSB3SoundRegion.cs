﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Regions/Sound")]
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

    public override void SetRegion(MSB3.Region bregion)
    {
        var region = (MSB3.Region.Sound)bregion;
        setBaseRegion(region);
        SoundType = region.SoundType;
        SoundID = region.SoundID;
        ChildRegionNames = region.ChildRegionNames;
    }

    public override MSB3.Region Serialize(GameObject parent)
    {
        var region = new MSB3.Region.Sound(parent.name);
        _Serialize(region, parent);
        region.SoundID = SoundID;
        region.SoundType = SoundType;
        for (int i = 0; i < 16; i++)
        {
            if (i >= ChildRegionNames.Length)
            {
                region.ChildRegionNames[i] = null;
                continue;
            }
            region.ChildRegionNames[i] = (ChildRegionNames[i] == "") ? null : ChildRegionNames[i];
        }
        return region;
    }
}
