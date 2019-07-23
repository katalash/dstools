using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

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


    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.Message)bevt;
        setBaseEvent(evt);
        MessageID = evt.MessageID;
        UnkT02 = evt.UnkT02;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.Message();
        _Serialize(evt, parent);
        evt.MessageID = MessageID;
        evt.UnkT02 = UnkT02;
        return evt;
    }
}
