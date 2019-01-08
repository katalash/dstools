using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.MODEL_PARAM_ST;

// Stores all the MSB specific fields for a part
public class MSB1Model : MonoBehaviour
{
    /// <summary>
    /// The placeholder used for this model in MapStudio.
    /// </summary>
    public string Placeholder;

    /// <summary>
    /// The ID of this model.
    /// </summary>
    public int ID;

    public void setBaseModel(MsbModelBase model)
    {
        Placeholder = model.PlaceholderModel;
        ID = model.Index;
    }

    internal void _Serialize(MsbModelBase model, GameObject parent)
    {
        model.Name = parent.name;
        model.PlaceholderModel = Placeholder;
        model.Index = ID;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
