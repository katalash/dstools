using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Parts/Navimesh")]
public class MSB2NavimeshPart : MSB2Part
{
    public uint NavimeshGroup1;
    public uint NavimeshGroup2;
    public uint NavimeshGroup3;
    public uint NavimeshGroup4;
    public override void SetPart(MSB2.Part bpart)
    {
        var part = (MSB2.Part.Navmesh)bpart;
        setBasePart(part);
        NavimeshGroup1 = part.UnkT00;
        NavimeshGroup2 = part.UnkT04;
        NavimeshGroup3 = part.UnkT08;
        NavimeshGroup4 = part.UnkT0C;
    }

    public override MSB2.Part Serialize(GameObject parent)
    {
        var part = new MSB2.Part.Navmesh();
        _Serialize(part, parent);
        part.UnkT00 = NavimeshGroup1;
        part.UnkT04 = NavimeshGroup2;
        part.UnkT08 = NavimeshGroup3;
        part.UnkT0C = NavimeshGroup4;
        return part;
    }
}
