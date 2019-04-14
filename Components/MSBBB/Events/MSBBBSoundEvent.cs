using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Sound")]
public class MSBBBSoundEvent : MSBBBEvent
{
    /// <summary>
    /// Type of sound in this region; determines mixing behavior like muffling.
    /// </summary>
    public MSBBB.Event.Sound.SndType SoundType;

    /// <summary>
    /// ID of the sound to play in this region, or 0 for child regions.
    /// </summary>
    public int SoundID;

    public void SetEvent(MSBBB.Event.Sound evt)
    {
        setBaseEvent(evt);
        SoundType = evt.SoundType;
        SoundID = evt.SoundID;
    }

    public MSBBB.Event.Sound Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Sound(ID, parent.name);
        _Serialize(evt, parent);
        evt.SoundType = SoundType;
        evt.SoundID = SoundID;
        return evt;
    }
}
