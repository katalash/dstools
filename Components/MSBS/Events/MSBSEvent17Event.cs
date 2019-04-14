using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Event 17")]
public class MSBSEvent17Event : MSBSEvent
{

    public int UnkT00;

    public void SetEvent(MSBS.Event.Event17 evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
    }

    public MSBS.Event.Event17 Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Event17();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        return evt;
    }
}
