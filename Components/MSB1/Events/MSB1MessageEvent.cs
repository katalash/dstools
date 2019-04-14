using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Message")]
public class MSB1MessageEvent : MSB1Event
{
    /// <summary>
    /// ID of the message's text in the FMGs.
    /// </summary>
    public short MessageID;

    /// <summary>
    /// Unknown. Always 0 or 2.
    /// </summary>
    public short UnkT02;


    public void SetEvent(MsbEventBloodMsg evt)
    {
        setBaseEvent(evt);
        MessageID = evt.MsgID;
        UnkT02 = evt.SubUnk1;
    }

    public MsbEventBloodMsg Serialize(GameObject parent)
    {
        var evt = new MsbEventBloodMsg();
        _Serialize(evt, parent);
        evt.MsgID = MessageID;
        evt.SubUnk1 = UnkT02;
        return evt;
    }
}
