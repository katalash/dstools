using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Auto Draw Group")]
public class MSBSAutoDrawGroupEvent : MSBSEvent
{

    public int UnkT00;
    public int UnkT04;

    public void SetEvent(MSBS.Event.AutoDrawGroup evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
    }

    public MSBS.Event.AutoDrawGroup Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.AutoDrawGroup();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        evt.UnkT04 = UnkT04;
        return evt;
    }
}
