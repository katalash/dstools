using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.PARTS_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Parts/Navimesh")]
public class MSB1NavimeshPart : MSB1Part
{
    public int NavimeshGroup1;
    public int NavimeshGroup2;
    public int NavimeshGroup3;
    public int NavimeshGroup4;
    public void SetPart(MsbPartsNavimesh part)
    {
        setBasePart(part);
        NavimeshGroup1 = part.NaviMeshGroup1;
        NavimeshGroup2 = part.NaviMeshGroup2;
        NavimeshGroup3 = part.NaviMeshGroup3;
        NavimeshGroup4 = part.NaviMeshGroup4;
    }

    public MsbPartsNavimesh Serialize(GameObject parent)
    {
        var part = new MsbPartsNavimesh();
        _Serialize(part, parent);
        part.NaviMeshGroup1 = NavimeshGroup1;
        part.NaviMeshGroup2 = NavimeshGroup2;
        part.NaviMeshGroup3 = NavimeshGroup3;
        part.NaviMeshGroup4 = NavimeshGroup4;
        return part;
    }
}
