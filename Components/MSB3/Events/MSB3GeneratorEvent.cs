using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Events/Generator")]
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
    public int SessionCondition;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT14, UnkT18;

    public override void SetEvent(MSB3.Event bevt)
    {
        var evt = (MSB3.Event.Generator)bevt;
        setBaseEvent(evt);
        MaxNum = evt.MaxNum;
        LimitNum = evt.LimitNum;
        MinGenNum = evt.MinGenNum;
        MaxGenNum = evt.MaxGenNum;
        MinInterval = evt.MinInterval;
        MaxInterval = evt.MaxInterval;
        SpawnPointNames = evt.SpawnPointNames;
        SpawnPartNames = evt.SpawnPartNames;
        SessionCondition = evt.SessionCondition;
        UnkT14 = evt.UnkT14;
        UnkT18 = evt.UnkT18;
    }

    public override MSB3.Event Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.Generator(parent.name);
        _Serialize(evt, parent);
        evt.MaxNum = MaxNum;
        evt.LimitNum = LimitNum;
        evt.MinGenNum = MinGenNum;
        evt.MaxGenNum = MaxGenNum;
        evt.MinInterval = MinInterval;
        evt.MaxInterval = MaxInterval;
        for (int i = 0; i < 8; i++)
        {
            if (i >= SpawnPointNames.Length)
            {
                evt.SpawnPointNames[i] = null;
                continue;
            }
            evt.SpawnPointNames[i] = (SpawnPointNames[i] == "") ? null : SpawnPointNames[i];
        }
        for (int i = 0; i < 32; i++)
        {
            if (i >= SpawnPartNames.Length)
            {
                evt.SpawnPartNames[i] = null;
                continue;
            }
            evt.SpawnPartNames[i] = (SpawnPartNames[i] == "") ? null : SpawnPartNames[i];
        }
        evt.SessionCondition = SessionCondition;
        evt.UnkT14 = UnkT14;
        evt.UnkT18 = UnkT18;
        return evt;
    }
}
