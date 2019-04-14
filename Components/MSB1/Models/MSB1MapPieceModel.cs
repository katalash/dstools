using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Model Declarations/Map Piece")]
public class MSB1MapPieceModel : MSB1Model
{
    public void SetModel(MsbModelMapPiece model)
    {
        setBaseModel(model);
    }

    public MsbModelMapPiece Serialize(GameObject parent)
    {
        var model = new MsbModelMapPiece();
        _Serialize(model, parent);
        return model;
    }
}
