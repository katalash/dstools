using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3EnvironmentMapPointRegion : MSB3Region
{
    /// <summary>
    /// Unknown. Only ever 1 bit set, so probably flags.
    /// </summary>
    public int UnkFlags;

    public void SetRegion(MSB3.Region.EnvironmentMapPoint region)
    {
        setBaseRegion(region);
        UnkFlags = region.UnkFlags;
    }
}
