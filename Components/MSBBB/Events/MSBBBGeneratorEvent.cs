using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Generator")]
public class MSBBBGeneratorEvent : MSBBBEvent
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

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.Generator)bevt;
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

    public override MSBBB.Event Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.Generator(parent.name);
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
                break;
            evt.SpawnPointNames[i] = (SpawnPointNames[i] == "") ? null : SpawnPointNames[i];
        }
        for (int i = 0; i < 32; i++)
        {
            if (i >= SpawnPartNames.Length)
                break;
            evt.SpawnPartNames[i] = (SpawnPartNames[i] == "") ? null : SpawnPartNames[i];
        }
        evt.UnkT10 = UnkT10;
        evt.UnkT14 = UnkT14;
        evt.UnkT18 = UnkT18;
        return evt;
    }
}
