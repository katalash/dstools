﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Model Declarations/Object")]
public class MSB3ObjectModel : MSB3Model
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public byte UnkT00, UnkT01;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool UnkT02, UnkT03;

    public override void SetModel(MSB3.Model bmodel)
    {
        var model = (MSB3.Model.Object)bmodel;
        setBaseModel(model);
        UnkT00 = model.UnkT00;
        UnkT01 = model.UnkT01;
        UnkT02 = model.UnkT02;
        UnkT03 = model.UnkT03;
    }

    public override MSB3.Model Serialize(GameObject parent)
    {
        var model = new MSB3.Model.Object(parent.name);
        _Serialize(model, parent);
        model.UnkT00 = UnkT00;
        model.UnkT01 = UnkT01;
        model.UnkT02 = UnkT02;
        model.UnkT03 = UnkT03;
        return model;
    }
}
