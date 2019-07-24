using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Model Declarations/Collision")]
public class MSB3CollisionModel : MSB3Model
{
    public override void SetModel(MSB3.Model bmodel)
    {
        var model = (MSB3.Model.Collision)bmodel;
        setBaseModel(model);
    }

    public override MSB3.Model Serialize(GameObject parent)
    {
        var model = new MSB3.Model.Collision(parent.name);
        _Serialize(model, parent);
        return model;
    }
}
