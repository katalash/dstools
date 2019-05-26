using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Parts/Connect Collision")]
public class MSB1ConnectCollisionPart : MSB1Part
{
    public byte UnkT00;
    public string MapName;

    public void SetPart(MsbPartsConnectHit part)
    {
        setBasePart(part);
        UnkT00 = part.SubUnk1;
        MapName = part.MapName;
    }

    public MsbPartsConnectHit Serialize(GameObject parent)
    {
        var part = new MsbPartsConnectHit();
        _Serialize(part, parent);
        part.SubUnk1 = UnkT00;
        part.MapName = (MapName == "") ? null : MapName;
        return part;
    }
}
