using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Object Action")]
public class MSBBBObjActEvent : MSBBBEvent
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int ObjActEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string PartName2;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int ParameterID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventFlagID;

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.ObjAct)bevt;
        ObjActEntityID = evt.ObjActEntityID;
        PartName2 = evt.PartName2;
        ParameterID = evt.ParameterID;
        UnkT10 = evt.UnkT10;
        EventFlagID = evt.EventFlagID;
    }

    public override MSBBB.Event Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.ObjAct(parent.name);
        _Serialize(evt, parent);
        evt.ObjActEntityID = ObjActEntityID;
        evt.PartName2 = (PartName2 == "") ? null : PartName2;
        evt.ParameterID = ParameterID;
        evt.UnkT10 = UnkT10;
        evt.EventFlagID = EventFlagID;
        return evt;
    }
}
