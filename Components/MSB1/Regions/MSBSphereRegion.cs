using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.POINT_PARAM_ST;

public class MSB1SphereRegion : MSB1Region
{
    public int EventEntityID;
    public void SetRegion(MsbRegionSphere region)
    {
        EventEntityID = region.EntityID;
        setBaseRegion(region);
    }

    public MsbRegionSphere Serialize(GameObject parent)
    {
        var region = new MsbRegionSphere(null);
        _Serialize(region, parent);
        region.EntityID = EventEntityID;
        if (parent.GetComponent<SphereCollider>() != null)
        {
            var col = parent.GetComponent<SphereCollider>();
            region.Radius = col.radius;
        }
        else
        {
            throw new Exception($@"Sphere region {parent.name} has no shape. Attach a sphere collider.");
        }
        return region;
    }
}
