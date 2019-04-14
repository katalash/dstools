using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Object Action")]
public class MSBSObjActEvent : MSBSEvent
{


    public int ObjActEntityID;
    public string ObjActPartName;
    public int ObjActID;
    public byte StateType;
    public int EventFlagID;

    public void SetEvent(MSBS.Event.ObjAct evt)
    {
        setBaseEvent(evt);
        ObjActEntityID = evt.ObjActEntityID;
        ObjActPartName = evt.ObjActPartName;
        ObjActID = evt.ObjActID;
        StateType = evt.StateType;
        EventFlagID = evt.EventFlagID;
    }

    public MSBS.Event.ObjAct Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.ObjAct();
        _Serialize(evt, parent);
        evt.ObjActEntityID = ObjActEntityID;
        evt.ObjActPartName = ObjActPartName;
        evt.ObjActID = ObjActID;
        evt.StateType = StateType;
        evt.EventFlagID = EventFlagID;
        return evt;
    }
}
