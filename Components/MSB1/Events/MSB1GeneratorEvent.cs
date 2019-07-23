using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Generator")]
public class MSB1GeneratorEvent : MSB1Event
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

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.Generator)bevt;
        setBaseEvent(evt);
        MaxNum = evt.MaxNum;
        LimitNum = evt.LimitNum;
        MinGenNum = evt.MinGenNum;
        MaxGenNum = evt.MaxGenNum;
        MinInterval = evt.MinInterval;
        MaxInterval = evt.MaxInterval;
        SpawnPointNames = evt.SpawnPointNames;
        SpawnPartNames = evt.SpawnPartNames;
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.Generator();
        _Serialize(evt, parent);
        evt.MaxNum = MaxNum;
        evt.LimitNum = LimitNum;
        evt.MinGenNum = MinGenNum;
        evt.MaxGenNum = MaxGenNum;
        evt.MinInterval = MinInterval;
        evt.MaxInterval = MaxInterval;
        for (int i = 0; i < 4; i++)
        {
            if (i >= SpawnPointNames.Length)
                break;
            evt.SpawnPointNames[i] = (SpawnPointNames[i] == "") ? null : SpawnPointNames[i];
        }
        for (int i = 0; i < 32; i++)
        {
            if (i >= SpawnPartNames.Length)
                break;
            evt.SpawnPartNames[i] = (SpawnPartNames[i] == "") ? null : SpawnPartNames[i];
        }
        return evt;
    }
}
