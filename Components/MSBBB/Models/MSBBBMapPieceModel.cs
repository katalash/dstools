using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Model Declarations/Map Piece")]
public class MSBBBMapPieceModel : MSBBBModel
{
    public override void SetModel(MSBBB.Model model)
    {
        setBaseModel(model);
    }

    public MSBBB.Model.MapPiece Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.MapPiece(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
