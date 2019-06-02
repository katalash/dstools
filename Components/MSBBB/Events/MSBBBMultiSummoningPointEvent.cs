using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/MultiSummoningPoint")]
public class MSBBBMultiSummoningPointEvent : MSBBBEvent
{
    public int UnkT00;
    public short UnkT04;
    public short UnkT06;
    public short UnkT08;
    public short UnkT0A;

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.MultiSummoningPoint)bevt;
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        UnkT06 = evt.UnkT06;
        UnkT08 = evt.UnkT08;
        UnkT0A = evt.UnkT0A;
    }

    public MSBBB.Event.MultiSummoningPoint Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.MultiSummoningPoint(parent.name);
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        evt.UnkT04 = UnkT04;
        evt.UnkT06 = UnkT06;
        evt.UnkT08 = UnkT08;
        evt.UnkT0A = UnkT0A;
        return evt;
    }
}
