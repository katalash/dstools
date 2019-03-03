using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Light")]
public class MSB1LightEvent : MSB1Event
{
    public int UnkT00;


    public void SetEvent(MsbEventLight evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.SubUnk1;
    }

    public MsbEventLight Serialize(GameObject parent)
    {
        var evt = new MsbEventLight();
        _Serialize(evt, parent);
        evt.SubUnk1 = UnkT00;
        return evt;
    }
}
