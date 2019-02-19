using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3ObjActEvent : MSB3Event
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

    public void SetEvent(MSB3.Event.ObjAct evt)
    {
        setBaseEvent(evt);
        ObjActEntityID = evt.ObjActEntityID;
        PartName2 = evt.PartName2;
        ParameterID = evt.ParameterID;
        UnkT10 = evt.UnkT10;
        EventFlagID = evt.EventFlagID;
    }

    public MSB3.Event.ObjAct Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.ObjAct(ID, parent.name);
        _Serialize(evt, parent);
        evt.ObjActEntityID = ObjActEntityID;
        evt.PartName2 = (PartName2 == "") ? null : PartName2;
        evt.ParameterID = ParameterID;
        evt.UnkT10 = UnkT10;
        evt.EventFlagID = EventFlagID;
        return evt;
    }
}
