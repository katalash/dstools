using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Connect Collision")]
public class MSB3ConnectCollisionPart : MSB3Part
{
    /// <summary>
    /// The name of the associated collision part.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// A map ID in format mXX_XX_XX_XX.
    /// </summary>
    public byte MapID1, MapID2, MapID3, MapID4;

    public void SetPart(MSB3.Part.ConnectCollision part)
    {
        setBasePart(part);
        CollisionName = part.CollisionName;
        MapID1 = part.MapID1;
        MapID2 = part.MapID2;
        MapID3 = part.MapID3;
        MapID4 = part.MapID4;
    }

    public MSB3.Part.ConnectCollision Serialize(GameObject parent)
    {
        var part = new MSB3.Part.ConnectCollision(parent.name);
        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.MapID1 = MapID1;
        part.MapID2 = MapID2;
        part.MapID3 = MapID3;
        part.MapID4 = MapID4;
        return part;
    }
}
