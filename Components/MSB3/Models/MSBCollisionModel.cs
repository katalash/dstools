using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3CollisionModel : MSB3Model
{
    public void SetModel(MSB3.Model.Collision model)
    {
        setBaseModel(model);
    }

    public MSB3.Model.Collision Serialize(GameObject parent)
    {
        var model = new MSB3.Model.Collision(ID, parent.name);
        _Serialize(model, parent);
        return model;
    }
}
