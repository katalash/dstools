using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSBBBMapPiecePart : MSBBBPart
{
    /// <summary>
    /// Controls which value from LightSet in the gparam is used.
    /// </summary>
    public int LightParamID;

    /// <summary>
    /// Controls which value from FogParam in the gparam is used.
    /// </summary>
    public int FogParamID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT10, UnkT14;

    public void SetPart(MSBBB.Part.MapPiece part)
    {
        setBasePart(part);
        LightParamID = part.LightParamID;
        FogParamID = part.FogParamID;
        UnkT10 = part.UnkT10;
        UnkT14 = part.UnkT14;
    }

    public MSBBB.Part.MapPiece Serialize(GameObject parent)
    {
        var part = new MSBBB.Part.MapPiece(ID, parent.name);
        _Serialize(part, parent);
        part.LightParamID = LightParamID;
        part.FogParamID = FogParamID;
        part.UnkT10 = UnkT10;
        part.UnkT14 = UnkT14;
        return part;
    }
}
