using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Other")]
public class MSBBBOtherEvent : MSBBBEvent
{
    public override void SetEvent(MSBBB.Event evt)
    {
        setBaseEvent(evt);
    }

    public MSBBB.Event.Other Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Other(parent.name);
        _Serialize(evt, parent);
        return evt;
    }
}
