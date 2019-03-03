using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/SFX")]
public class MSB1SFXEvent : MSB1Event
{
    /// <summary>
    /// The ID of the .fxr file to play in this region.
    /// </summary>
    public int FFXID;

    public void SetEvent(MsbEventSFX evt)
    {
        setBaseEvent(evt);
        FFXID = evt.SfxID;
    }

    public MsbEventSFX Serialize(GameObject parent)
    {
        var evt = new MsbEventSFX();
        _Serialize(evt, parent);
        evt.SfxID = FFXID;
        return evt;
    }
}
