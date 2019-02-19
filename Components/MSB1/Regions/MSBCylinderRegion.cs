using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.POINT_PARAM_ST;

public class MSB1CylinderRegion : MSB1Region
{
    public int EventEntityID;
    public void SetRegion(MsbRegionCylinder region)
    {
        EventEntityID = region.EntityID;
        setBaseRegion(region);
    }

    public MsbRegionCylinder Serialize(GameObject parent)
    {
        var region = new MsbRegionCylinder(null);
        _Serialize(region, parent);
        region.EntityID = EventEntityID;
        if (parent.GetComponent<CapsuleCollider>() != null)
        {
            var col = parent.GetComponent<CapsuleCollider>();
            region.Radius = col.radius;
            region.Height = col.height;
        }
        else
        {
            throw new Exception($@"Cylinder region {parent.name} has no shape. Attach a capsule collider.");
        }
        return region;
    }
}
