using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

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

    public void SetEvent(MsbEventWindSFX evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.SubUnk1;
        UnkT04 = evt.SubUnk2;
        UnkT08 = evt.SubUnk3;
        UnkT0C = evt.SubUnk4;
        UnkT10 = evt.SubUnk5;
        UnkT14 = evt.SubUnk6;
        UnkT18 = evt.SubUnk7;
        UnkT1C = evt.SubUnk8;
        UnkT20 = evt.SubUnk9;
        UnkT24 = evt.SubUnk10;
        UnkT28 = evt.SubUnk11;
        UnkT2C = evt.SubUnk12;
        UnkT30 = evt.SubUnk13;
        UnkT34 = evt.SubUnk14;
        UnkT38 = evt.SubUnk15;
        UnkT3C = evt.SubUnk16;
    }

    public MsbEventWindSFX Serialize(GameObject parent)
    {
        var evt = new MsbEventWindSFX();
        _Serialize(evt, parent);
        evt.SubUnk1 = UnkT00;
        evt.SubUnk2 = UnkT04;
        evt.SubUnk3 = UnkT08;
        evt.SubUnk4 = UnkT0C;
        evt.SubUnk5 = UnkT10;
        evt.SubUnk6 = UnkT14;
        evt.SubUnk7 = UnkT18;
        evt.SubUnk8 = UnkT1C;
        evt.SubUnk9 = UnkT20;
        evt.SubUnk10 = UnkT24;
        evt.SubUnk11 = UnkT28;
        evt.SubUnk12 = UnkT2C;
        evt.SubUnk13 = UnkT30;
        evt.SubUnk14 = UnkT34;
        evt.SubUnk15 = UnkT38;
        evt.SubUnk16 = UnkT3C;
        return evt;
    }
}
