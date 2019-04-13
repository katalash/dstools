using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Model Declarations/Enemy")]
public class MSBSEnemyModel : MSBSModel
{

    public void SetModel(MSBS.Model.Enemy model)
    {
        setBaseModel(model);
    }

    public MSBS.Model.Enemy Serialize(GameObject parent)
    {
        var model = new MSBS.Model.Enemy();
        _Serialize(model, parent);
        return model;
    }
}
