using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Event 20")]
public class MSBSEvent20Event : MSBSEvent
{

    public int UnkT00;
    public short UnkT04;
    public short UnkT06;

    public void SetEvent(MSBS.Event.Event20 evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        UnkT06 = evt.UnkT06;
    }

    public MSBS.Event.Event20 Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Event20();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        evt.UnkT04 = UnkT04;
        evt.UnkT06 = UnkT06;
        return evt;
    }
}
