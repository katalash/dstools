using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Parts Group")]
public class MSBSPartsGroupEvent : MSBSEvent
{

    public void SetEvent(MSBS.Event.PartsGroup evt)
    {
        setBaseEvent(evt);
    }

    public MSBS.Event.PartsGroup Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.PartsGroup();
        _Serialize(evt, parent);
        return evt;
    }
}
