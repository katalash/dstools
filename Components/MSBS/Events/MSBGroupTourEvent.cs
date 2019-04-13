using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Group Tour")]
public class MSBSGroupTourEvent : MSBSEvent
{

    public int PlatoonIDScriptActive;
    public int State;
    public string[] GroupPartNames;
    

    public void SetEvent(MSBS.Event.GroupTour evt)
    {
        setBaseEvent(evt);
        PlatoonIDScriptActive = evt.PlatoonIDScriptActive;
        State = evt.State;
        GroupPartNames = evt.GroupPartNames;

    }

    public MSBS.Event.GroupTour Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.GroupTour();
        _Serialize(evt, parent);
        evt.PlatoonIDScriptActive = PlatoonIDScriptActive;
        evt.State = State;
        for (int i = 0; i < 32; i++)
        {
            if (i >= GroupPartNames.Length)
                break;
            evt.GroupPartNames[i] = (GroupPartNames[i] == "") ? null : GroupPartNames[i];
        }
        return evt;
    }
}
