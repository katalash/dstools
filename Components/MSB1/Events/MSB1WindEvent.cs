using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Wind")]
public class MSB1WindEvent : MSB1Event
{
    public float UnkT00;
    public float UnkT04;
    public float UnkT08;
    public float UnkT0C;
    public float UnkT10;
    public float UnkT14;
    public float UnkT18;
    public float UnkT1C;
    public float UnkT20;
    public float UnkT24;
    public float UnkT28;
    public float UnkT2C;
    public float UnkT30;
    public float UnkT34;
    public float UnkT38;
    public float UnkT3C;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.WindSFX)bevt;
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        UnkT08 = evt.UnkT08;
        UnkT0C = evt.UnkT0C;
        UnkT10 = evt.UnkT10;
        UnkT14 = evt.UnkT14;
        UnkT18 = evt.UnkT18;
        UnkT1C = evt.UnkT1C;
        UnkT20 = evt.UnkT20;
        UnkT24 = evt.UnkT24;
        UnkT28 = evt.UnkT28;
        UnkT2C = evt.UnkT2C;
        UnkT30 = evt.UnkT30;
        UnkT34 = evt.UnkT34;
        UnkT38 = evt.UnkT38;
        UnkT3C = evt.UnkT3C;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.WindSFX();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        evt.UnkT04 = UnkT04;
        evt.UnkT08 = UnkT08;
        evt.UnkT0C = UnkT0C;
        evt.UnkT10 = UnkT10;
        evt.UnkT14 = UnkT14;
        evt.UnkT18 = UnkT18;
        evt.UnkT1C = UnkT1C;
        evt.UnkT20 = UnkT20;
        evt.UnkT24 = UnkT24;
        evt.UnkT28 = UnkT28;
        evt.UnkT2C = UnkT2C;
        evt.UnkT30 = UnkT30;
        evt.UnkT34 = UnkT34;
        evt.UnkT38 = UnkT38;
        evt.UnkT3C = UnkT3C;
        return evt;
    }
}
