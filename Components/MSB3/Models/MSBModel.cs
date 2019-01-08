using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for a part
public class MSB3Model : MonoBehaviour
{
    /// <summary>
    /// The placeholder used for this model in MapStudio.
    /// </summary>
    public string Placeholder;

    /// <summary>
    /// The ID of this model.
    /// </summary>
    public int ID;

    public void setBaseModel(MSB3.Model model)
    {
        Placeholder = model.Placeholder;
        ID = model.ID;
    }

    internal void _Serialize(MSB3.Model model, GameObject parent)
    {
        model.Name = parent.name;
        model.Placeholder = Placeholder;
        model.ID = ID;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
