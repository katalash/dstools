using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Object Action")]
public class MSB1ObjActEvent : MSB1Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int ObjActEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string ObjActPartName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short ObjActParamID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT0A;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventFlagID;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.ObjAct)bevt;
        setBaseEvent(evt);
        ObjActEntityID = evt.ObjActEntityID;
        ObjActPartName = evt.ObjActPartName;
        ObjActParamID = evt.ObjActParamID;
        UnkT0A = evt.UnkT0A;
        EventFlagID = evt.EventFlagID;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.ObjAct();
        _Serialize(evt, parent);
        evt.ObjActEntityID = ObjActEntityID;
        evt.ObjActPartName = (ObjActPartName == "") ? null : ObjActPartName;
        evt.ObjActParamID = ObjActParamID;
        evt.UnkT0A = UnkT0A;
        evt.EventFlagID = EventFlagID;
        return evt;
    }
}
