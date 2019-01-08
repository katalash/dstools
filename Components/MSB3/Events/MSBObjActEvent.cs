using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3ObjActEvent : MSB3Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int ObjActEntityID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string PartName2;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int ParameterID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int EventFlagID;

    public void SetEvent(MSB3.Event.ObjAct evt)
    {
        setBaseEvent(evt);
        ObjActEntityID = evt.ObjActEntityID;
        PartName2 = evt.PartName2;
        ParameterID = evt.ParameterID;
        UnkT10 = evt.UnkT10;
        EventFlagID = evt.EventFlagID;
    }
}
