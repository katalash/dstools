using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Parts/Connect Collision")]
public class MSB2ConnectCollisionPart : MSB2Part
{
    public string CollisionName;
    public byte MapID1;
    public byte MapID2;
    public byte MapID3;
    public byte MapID4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT08;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT0C;

    public override void SetPart(MSB2.Part bpart)
    {
        var part = (MSB2.Part.ConnectCollision)bpart;
        setBasePart(part);
        CollisionName = part.CollisionName;
        MapID1 = part.MapID1;
        MapID2 = part.MapID2;
        MapID3 = part.MapID3;
        MapID4 = part.MapID4;
        UnkT08 = part.UnkT08;
        UnkT0C = part.UnkT0C;
    }

    public override MSB2.Part Serialize(GameObject parent)
    {
        var part = new MSB2.Part.ConnectCollision();
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.MapID1 = MapID1;
        part.MapID2 = MapID2;
        part.MapID3 = MapID3;
        part.MapID4 = MapID4;
        part.UnkT08 = UnkT08;
        part.UnkT0C = UnkT0C;
        return part;
    }
}
