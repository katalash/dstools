using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Sound")]
public class MSB1SoundEvent : MSB1Event
{
    /// <summary>
    /// Type of sound in this region; determines mixing behavior like muffling.
    /// </summary>
    public int SoundType;

    /// <summary>
    /// ID of the sound to play in this region, or 0 for child regions.
    /// </summary>
    public int SoundID;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.Sound)bevt;
        setBaseEvent(evt);
        SoundType = evt.SoundType;
        SoundID = evt.SoundID;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.Sound();
        _Serialize(evt, parent);
        evt.SoundType = SoundType;
        evt.SoundID = SoundID;
        return evt;
    }
}
