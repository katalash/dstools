using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3WalkRouteRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.WalkRoute region)
    {
        setBaseRegion(region);
    }
}
