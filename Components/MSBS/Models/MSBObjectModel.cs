using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Model Declarations/Object")]
public class MSBSObjectModel : MSBSModel
{

    public void SetModel(MSBS.Model.Object model)
    {
        setBaseModel(model);
    }

    public MSBS.Model.Object Serialize(GameObject parent)
    {
        var model = new MSBS.Model.Object();
        _Serialize(model, parent);
        return model;
    }
}
