using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3GroupTourEvent : MSB3Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00, UnkT04;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string[] GroupPartsNames;

    public void SetEvent(MSB3.Event.GroupTour evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        GroupPartsNames = evt.GroupPartsNames;
    }
}
