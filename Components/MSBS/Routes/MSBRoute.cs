using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

// Stores all the MSB specific fields for a part
public abstract class MSBSRoute : MonoBehaviour
{


    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk08;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk0C;

    public void setBaseRoute(MSBS.Route route)
    {
        Unk08 = route.Unk08;
        Unk0C = route.Unk0C;
    }

    internal void _Serialize(MSBS.Route route, GameObject parent)
    {
        route.Name = parent.name;
        route.Unk08 = Unk08;
        route.Unk0C = Unk0C;
    }

}
