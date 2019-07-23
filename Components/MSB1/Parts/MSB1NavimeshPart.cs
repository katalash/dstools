using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Parts/Navimesh")]
public class MSB1NavimeshPart : MSB1Part
{
    public uint NavimeshGroup1;
    public uint NavimeshGroup2;
    public uint NavimeshGroup3;
    public uint NavimeshGroup4;
    public override void SetPart(MSB1.Part bpart)
    {
        var part = (MSB1.Part.Navmesh)bpart;
        setBasePart(part);
        NavimeshGroup1 = part.NvmGroups[0];
        NavimeshGroup2 = part.NvmGroups[1];
        NavimeshGroup3 = part.NvmGroups[2];
        NavimeshGroup4 = part.NvmGroups[3];
    }

    public override MSB1.Part Serialize(GameObject parent)
    {
        var part = new MSB1.Part.Navmesh();
        _Serialize(part, parent);
        part.NvmGroups[0] = NavimeshGroup1;
        part.NvmGroups[1] = NavimeshGroup2;
        part.NvmGroups[2] = NavimeshGroup3;
        part.NvmGroups[3] = NavimeshGroup4;
        return part;
    }
}
