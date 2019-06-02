using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Environment")]
public class MSBBBEnvironmentEvent : MSBBBEvent
{
    public int UnkT00;
    public float UnkT04;
    public float UnkT08;
    public float UnkT0C;
    public float UnkT10;
    public float UnkT14;

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.Environment)bevt;
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        UnkT08 = evt.UnkT08;
        UnkT0C = evt.UnkT0C;
        UnkT10 = evt.UnkT10;
        UnkT14 = evt.UnkT14;
    }

    public MSBBB.Event.Environment Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Environment(parent.name);
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
