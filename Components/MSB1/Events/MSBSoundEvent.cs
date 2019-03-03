using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Sound")]
public class MSB1SoundEvent : MSB1Event
{
    /// <summary>
    /// Type of sound in this region; determines mixing behavior like muffling.
    /// </summary>
    public MsbSoundType SoundType;

    /// <summary>
    /// ID of the sound to play in this region, or 0 for child regions.
    /// </summary>
    public int SoundID;

    public void SetEvent(MsbEventSound evt)
    {
        setBaseEvent(evt);
        SoundType = evt.SoundType;
        SoundID = evt.SoundID;
    }

    public MsbEventSound Serialize(GameObject parent)
    {
        var evt = new MsbEventSound();
        _Serialize(evt, parent);
        evt.SoundType = SoundType;
        evt.SoundID = SoundID;
        return evt;
    }
}
