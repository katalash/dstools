using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/SFX")]
public class MSB2SFXRegion : MSB2Region
{
    /// <summary>
    /// The effect to play at this region.
    /// </summary>
    public int EffectID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT04;

    public override void SetRegion(MSB2.Region bregion)
    {
        var region = (MSB2.Region.SFX)bregion;
        setBaseRegion(region);
        EffectID = region.EffectID;
        UnkT04 = region.UnkT04;
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var region = new MSB2.Region.SFX(parent.name);
        _Serialize(region, parent);
        region.EffectID = EffectID;
        region.UnkT04 = UnkT04;
        return region;
    }
}
