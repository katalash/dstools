using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Model Declarations/Player")]
public class MSBBBPlayerModel : MSBBBModel
{
    public void SetModel(MSBBB.Model.Player model)
    {
        setBaseModel(model);
    }

    public MSBBB.Model.Player Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Player(ID, parent.name);
        _Serialize(model, parent);
        return model;
    }
}
