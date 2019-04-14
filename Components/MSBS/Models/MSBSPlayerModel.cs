using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Model Declarations/Player")]
public class MSBSPlayerModel : MSBSModel
{

    public void SetModel(MSBS.Model.Player model)
    {
        setBaseModel(model);
    }

    public MSBS.Model.Player Serialize(GameObject parent)
    {
        var model = new MSBS.Model.Player();
        _Serialize(model, parent);
        return model;
    }
}
