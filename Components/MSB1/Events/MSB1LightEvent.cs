using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Light")]
public class MSB1LightEvent : MSB1Event
{
    public int UnkT00;


    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.Light)bevt;
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.Light();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        return evt;
    }
}
