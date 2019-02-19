using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.POINT_PARAM_ST;

public class MSB1BoxRegion : MSB1Region
{
    public int EventEntityID;
    public void SetRegion(MsbRegionBox region)
    {
        EventEntityID = region.EntityID;
        setBaseRegion(region);
    }

    public MsbRegionBox Serialize(GameObject parent)
    {
        var region = new MsbRegionBox(null);
        _Serialize(region, parent);
        region.EntityID = EventEntityID;
        if (parent.GetComponent<BoxCollider>() != null)
        {
            var col = parent.GetComponent<BoxCollider>();
            region.WidthX = col.size.x;
            region.HeightY = col.size.y;
            region.DepthZ = col.size.z;
        }
        else
        {
            throw new Exception($@"Box region {parent.name} has no shape. Attach a box collider.");
        }
        return region;
    }
}
