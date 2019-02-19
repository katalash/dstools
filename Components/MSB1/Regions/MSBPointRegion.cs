using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.POINT_PARAM_ST;

public class MSB1PointRegion : MSB1Region
{
    public int EventEntityID;
    public void SetRegion(MsbRegionPoint region)
    {
        EventEntityID = region.EntityID;
        setBaseRegion(region);
    }

    public MsbRegionPoint Serialize(GameObject parent)
    {
        var region = new MsbRegionPoint(null);
        _Serialize(region, parent);
        region.EntityID = EventEntityID;
        if (parent.GetComponent<SphereCollider>() != null)
        {
            var col = parent.GetComponent<SphereCollider>();
        }
        else
        {
            throw new Exception($@"Point region {parent.name} has no shape. Attach a sphere collider.");
        }
        return region;
    }
}
