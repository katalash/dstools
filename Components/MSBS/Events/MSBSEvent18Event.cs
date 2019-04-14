using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Event 18")]
public class MSBSEvent18Event : MSBSEvent
{

    public int UnkT00;

    public void SetEvent(MSBS.Event.Event18 evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
    }

    public MSBS.Event.Event18 Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Event18();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        return evt;
    }
}
