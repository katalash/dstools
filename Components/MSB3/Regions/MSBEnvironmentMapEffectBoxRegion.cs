using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3EnvironmentEffectBoxRegion : MSB3Region
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT04;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT08, UnkT0A;

    public void SetRegion(MSB3.Region.EnvironmentMapEffectBox region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
        UnkT04 = region.UnkT04;
        UnkT08 = region.UnkT08;
        UnkT0A = region.UnkT0A;
    }
}
