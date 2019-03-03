using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Environment Effect")]
public class MSB1EnvironmentEvent : MSB1Event
{
    public int UnkT00;
    public float UnkT04;
    public float UnkT08;
    public float UnkT0C;
    public float UnkT10;
    public float UnkT14;

    public void SetEvent(MsbEventEnvironment evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.SubUnk1;
        UnkT04 = evt.SubUnk2;
        UnkT08 = evt.SubUnk3;
        UnkT0C = evt.SubUnk4;
        UnkT10 = evt.SubUnk5;
        UnkT14 = evt.SubUnk6;
    }

    public MsbEventEnvironment Serialize(GameObject parent)
    {
        var evt = new MsbEventEnvironment();
        _Serialize(evt, parent);
        evt.SubUnk1 = UnkT00;
        evt.SubUnk2 = UnkT04;
        evt.SubUnk3 = UnkT08;
        evt.SubUnk4 = UnkT0C;
        evt.SubUnk5 = UnkT10;
        evt.SubUnk6 = UnkT14;
        return evt;
    }
}
