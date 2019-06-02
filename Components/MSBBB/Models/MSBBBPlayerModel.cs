using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Model Declarations/Player")]
public class MSBBBPlayerModel : MSBBBModel
{
    public override void SetModel(MSBBB.Model model)
    {
        setBaseModel(model);
    }

    public override MSBBB.Model Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Player(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
