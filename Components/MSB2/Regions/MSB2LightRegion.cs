using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Regions/Light")]
public class MSB2LightRegion : MSB2Region
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00 { get; set; }

    /// <summary>
    /// Unknown.
    /// </summary>
    public Color32 ColorT04 { get; set; }

    /// <summary>
    /// Unknown.
    /// </summary>
    public Color32 ColorT08 { get; set; }

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT0C { get; set; }

    public override void SetRegion(MSB2.Region bregion)
    {
        var region = (MSB2.Region.Light)bregion;
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        ColorT04 = new Color32(region.ColorT04.R, region.ColorT04.G, region.ColorT04.B, region.ColorT04.A);
        ColorT08 = new Color32(region.ColorT08.R, region.ColorT08.G, region.ColorT08.B, region.ColorT08.A);
        UnkT0C = region.UnkT0C;
    }

    public override MSB2.Region Serialize(GameObject parent)
    {
        var region = new MSB2.Region.Light(parent.name);
        _Serialize(region, parent);
        region.UnkT00 = UnkT00;
        region.ColorT04 = System.Drawing.Color.FromArgb(ColorT04.a, ColorT04.r, ColorT04.g, ColorT04.b);
        region.ColorT08 = System.Drawing.Color.FromArgb(ColorT08.a, ColorT08.r, ColorT08.g, ColorT08.b);
        region.UnkT0C = UnkT0C;
        return region;
    }
}
