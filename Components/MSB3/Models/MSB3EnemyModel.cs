using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Model Declarations/Enemy")]
public class MSB3EnemyModel : MSB3Model
{
    public void SetModel(MSB3.Model.Enemy model)
    {
        setBaseModel(model);
    }

    public MSB3.Model.Enemy Serialize(GameObject parent)
    {
        var model = new MSB3.Model.Enemy(ID, parent.name);
        _Serialize(model, parent);
        return model;
    }
}
