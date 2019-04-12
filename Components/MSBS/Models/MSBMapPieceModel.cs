using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Model Declarations/Map Piece")]
public class MSBSMapPieceModel : MSBSModel
{

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT04, UnkT08, UnkT0C, UnkT10, UnkT14, UnkT18;

    public void SetModel(MSBS.Model.MapPiece model)
    {
        setBaseModel(model);
        UnkT00 = model.UnkT00;
        UnkT04 = model.UnkT04;
        UnkT08 = model.UnkT08;
        UnkT0C = model.UnkT0C;
        UnkT10 = model.UnkT10;
        UnkT14 = model.UnkT14;
        UnkT18 = model.UnkT18;
    }

    public MSBS.Model.MapPiece Serialize(GameObject parent)
    {
        var model = new MSBS.Model.MapPiece();
        _Serialize(model, parent);
        model.UnkT00 = UnkT00;
        model.UnkT04 = UnkT04;
        model.UnkT08 = UnkT08;
        model.UnkT0C = UnkT0C;
        model.UnkT10 = UnkT10;
        model.UnkT14 = UnkT14;
        model.UnkT18 = UnkT18;
        return model;
    }
}
