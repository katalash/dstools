using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Other")]
public class MSBBBOtherEvent : MSBBBEvent
{
    public void SetEvent(MSBBB.Event.Other evt)
    {
        setBaseEvent(evt);
    }

    public MSBBB.Event.Other Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Other(ID, parent.name);
        _Serialize(evt, parent);
        return evt;
    }
}
