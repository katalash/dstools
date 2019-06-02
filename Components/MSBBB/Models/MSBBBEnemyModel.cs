using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Model Declarations/Enemy")]
public class MSBBBEnemyModel : MSBBBModel
{
    public override void SetModel(MSBBB.Model model)
    {
        setBaseModel(model);
    }

    public override MSBBB.Model Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Enemy(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
