using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Unknown")]
public class MSBBBUnknownEvent : MSBBBEvent
{
    public void SetEvent(MSBBB.Event.Unknown evt)
    {
        setBaseEvent(evt);
    }

    public MSBBB.Event.Unknown Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Unknown(parent.name);
        _Serialize(evt, parent);
        return evt;
    }
}
