using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Model Declarations/Enemy")]
public class MSBBBEnemyModel : MSBBBModel
{
    public void SetModel(MSBBB.Model.Enemy model)
    {
        setBaseModel(model);
    }

    public MSBBB.Model.Enemy Serialize(GameObject parent)
    {
        var model = new MSBBB.Model.Enemy(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
