using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3MufflingBoxRegion : MSB3Region
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    public void SetRegion(MSB3.Region.MufflingBox region)
    {
        setBaseRegion(region);
        UnkT00 = region.UnkT00;
    }
}
