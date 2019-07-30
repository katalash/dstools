using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using System.Numerics;
using System;

public abstract class MSB2Event : MonoBehaviour
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventID;

    public void setBaseEvent(MSB2.Event evt)
    {
        EventID = evt.EventID;
    }


    internal void _Serialize(MSB2.Event evt, GameObject parent)
    {
        evt.Name = parent.name;
        evt.EventID = EventID;
    }

    public abstract void SetEvent(MSB2.Event evt);
    public abstract MSB2.Event Serialize(GameObject obj);
}
