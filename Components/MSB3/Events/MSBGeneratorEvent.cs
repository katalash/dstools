using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3GeneratorEvent : MSB3Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public short MaxNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short LimitNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short MinGenNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short MaxGenNum;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float MinInterval;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float MaxInterval;

    /// <summary>
    /// Regions that enemies can be spawned at.
    /// </summary>
    public string[] SpawnPointNames;

    /// <summary>
    /// Enemies spawned by this generator.
    /// </summary>
    public string[] SpawnPartNames;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT14, UnkT18;

    public void SetEvent(MSB3.Event.Generator evt)
    {
        setBaseEvent(evt);
        MaxNum = evt.MaxNum;
        LimitNum = evt.LimitNum;
        MinGenNum = evt.MinGenNum;
        MaxGenNum = evt.MaxGenNum;
        MinInterval = evt.MinInterval;
        MaxInterval = evt.MaxInterval;
        SpawnPointNames = evt.SpawnPointNames;
        SpawnPartNames = evt.SpawnPartNames;
        UnkT10 = evt.UnkT10;
        UnkT14 = evt.UnkT14;
        UnkT18 = evt.UnkT18;
    }
}
