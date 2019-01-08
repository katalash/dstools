using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3WarpPointRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.WarpPoint region)
    {
        setBaseRegion(region);
    }
}
