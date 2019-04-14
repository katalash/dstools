using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Collision")]
public class MSBSConnectCollisionPart : MSBSPart
{

    public MSBSUnkStruct2Part Unk2;

    /// <summary>
    /// The name of the associated collision part.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// A map ID in format mXX_XX_XX_XX.
    /// </summary>
    public byte MapID1, MapID2, MapID3, MapID4;

    public void SetPart(MSBS.Part.ConnectCollision part)
    {
        setBasePart(part);
        Unk2 = gameObject.AddComponent<MSBSUnkStruct2Part>();
        Unk2.setStruct(part.Unk2);
        CollisionName = part.CollisionName;
        MapID1 = part.MapID[0];
        MapID2 = part.MapID[1];
        MapID3 = part.MapID[2];
        MapID4 = part.MapID[3];
    }

    public MSBS.Part.ConnectCollision Serialize(GameObject parent)
    {
        var part = new MSBS.Part.ConnectCollision();
        _Serialize(part, parent);
        part.Unk2 = Unk2.Serialize();
        part.CollisionName = CollisionName;
        part.MapID[0] = MapID1;
        part.MapID[1] = MapID2;
        part.MapID[2] = MapID3;
        part.MapID[3] = MapID4;
        return part;
    }
}
