using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Environment Effect")]
public class MSB1EnvironmentEvent : MSB1Event
{
    public int UnkT00;
    public float UnkT04;
    public float UnkT08;
    public float UnkT0C;
    public float UnkT10;
    public float UnkT14;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.Environment)bevt;
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        UnkT08 = evt.UnkT08;
        UnkT0C = evt.UnkT0C;
        UnkT10 = evt.UnkT10;
        UnkT14 = evt.UnkT14;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.Environment();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        evt.UnkT04 = UnkT04;
        evt.UnkT08 = UnkT08;
        evt.UnkT0C = UnkT0C;
        evt.UnkT10 = UnkT10;
        evt.UnkT14 = UnkT14;
        return evt;
    }
}
