using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SoulsFormats;

class BTLSekiroLight : BTLDS3Light
{
    /// <summary>
    /// Unknown; only in Sekiro.
    /// </summary>
    public float UnkC8;

    /// <summary>
    /// Unknown; only in Sekiro.
    /// </summary>
    public float UnkCC;

    /// <summary>
    /// Unknown; only in Sekiro.
    /// </summary>
    public float UnkD0;

    /// <summary>
    /// Unknown; only in Sekiro.
    /// </summary>
    public float UnkD4;

    /// <summary>
    /// Unknown; only in Sekiro.
    /// </summary>
    public float UnkD8;

    /// <summary>
    /// Unknown; only in Sekiro.
    /// </summary>
    public int UnkDC;

    /// <summary>
    /// Unknown; only in Sekiro.
    /// </summary>
    public float UnkE0;

    public override void SetFromLight(BTL.Light l)
    {
        base.SetFromLight(l);
        UnkC8 = l.UnkC8;
        UnkCC = l.UnkCC;
        UnkD0 = l.UnkD0;
        UnkD4 = l.UnkD4;
        UnkD8 = l.UnkD8;
        UnkDC = l.UnkDC;
        UnkE0 = l.UnkE0;
    }

    public override BTL.Light Serialize(GameObject parent, BTL.Light light=null)
    {
        var l = new BTL.Light();
        base.Serialize(parent, l);
        l.UnkC8 = UnkC8;
        l.UnkCC = UnkCC;
        l.UnkD0 = UnkD0;
        l.UnkD4 = UnkD4;
        l.UnkD8 = UnkD8;
        l.UnkDC = UnkDC;
        l.UnkE0 = UnkE0;
        return l;
    }
}
