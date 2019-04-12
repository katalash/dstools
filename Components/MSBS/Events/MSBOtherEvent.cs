using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Other")]
public class MSBSOtherEvent : MSBSEvent
{

    public void SetEvent(MSBS.Event.Other evt)
    {
        setBaseEvent(evt);
    }

    public MSBS.Event.Other Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Other();
        _Serialize(evt, parent);
        return evt;
    }
}
