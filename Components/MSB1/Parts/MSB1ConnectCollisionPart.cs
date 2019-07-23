using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Parts/Connect Collision")]
public class MSB1ConnectCollisionPart : MSB1Part
{
    public string CollisionName;
    public byte MapID1;
    public byte MapID2;
    public byte MapID3;
    public byte MapID4;

    public override void SetPart(MSB1.Part bpart)
    {
        var part = (MSB1.Part.ConnectCollision)bpart;
        setBasePart(part);
        CollisionName = part.CollisionName;
        MapID1 = part.MapID[0];
        MapID2 = part.MapID[1];
        MapID3 = part.MapID[2];
        MapID4 = part.MapID[3];
    }

    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.ConnectCollision();
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.MapID[0] = MapID1;
        part.MapID[1] = MapID2;
        part.MapID[2] = MapID3;
        part.MapID[3] = MapID4;
        return part;
    }
}
