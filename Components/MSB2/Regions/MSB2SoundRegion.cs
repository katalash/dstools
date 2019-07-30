using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/Sound")]
public class MSB2SoundRegion : MSB2Region
{
    /// <summary>
    /// Unknown; possibly sound type.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// ID of the sound to play in this region, or 0 for child regions.
    /// </summary>
    public int SoundID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT08;

    public override void SetRegion(MSB2.Region bregion)
    {
        var region = (MSB2.Region.Sound)bregion;
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        SoundID = region.SoundID;
        UnkT08 = region.UnkT08;
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var region = new MSB2.Region.Sound(parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.SoundID = SoundID;
        region.UnkT08 = UnkT08;
        return region;
    }
}
