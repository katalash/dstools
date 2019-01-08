using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3EventRegion : MSB3Region
{
    public void SetRegion(MSB3.Region.Event region)
    {
        setBaseRegion(region);
    }
}
