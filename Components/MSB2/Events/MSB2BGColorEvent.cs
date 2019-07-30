using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Events/BG Color")]
public class MSB2BGColorEvent : MSB2Event
{
    public Color32 Color;

    public override void SetEvent(MSB2.Event bevt)
    {
        var evt = (MSB2.Event.BGColor)bevt;
        setBaseEvent(evt);
        Color = new Color32(evt.Color.R, evt.Color.G, evt.Color.B, evt.Color.A);
    }

    public override MSB2.Event Serialize(GameObject parent)
    {
        var evt = new MSB2.Event.BGColor();
        _Serialize(evt, parent);
        evt.Color = System.Drawing.Color.FromArgb(Color.a, Color.r, Color.g, Color.b);
        return evt;
    }
}
