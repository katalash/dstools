using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Events/Group Tour")]
public class MSB3GroupTourEvent : MSB3Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int PlatoonIDScriptActivate, State;

    /// <summary>
    /// Unknown.
    /// </summary>
    public string[] GroupPartsNames;

    public void SetEvent(MSB3.Event.GroupTour evt)
    {
        setBaseEvent(evt);
        PlatoonIDScriptActivate = evt.PlatoonIDScriptActivate;
        State = evt.State;
        GroupPartsNames = evt.GroupPartsNames;
    }

    public MSB3.Event.GroupTour Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.GroupTour(ID, parent.name);
        _Serialize(evt, parent);
        evt.PlatoonIDScriptActivate = PlatoonIDScriptActivate;
        evt.State = State;
        for (int i = 0; i < 32; i++)
        {
            if (i >= GroupPartsNames.Length)
            {
                evt.GroupPartsNames[i] = null;
                continue;
            }
            evt.GroupPartsNames[i] = (GroupPartsNames[i] == "") ? null : GroupPartsNames[i];
        }

        return evt;
    }
}
