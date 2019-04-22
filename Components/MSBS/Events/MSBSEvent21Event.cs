using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Event 21")]
public class MSBSEvent21Event : MSBSEvent
{
    public string[] PartNames;

    public void SetEvent(MSBS.Event.Event21 evt)
    {
        setBaseEvent(evt);
        PartNames = evt.Event21PartNames;
    }

    public MSBS.Event.Event21 Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Event21();
        _Serialize(evt, parent);
        for (int i = 0; i < 32; i++)
        {
            if (i >= PartNames.Length)
            {
                evt.Event21PartNames[i] = null;
                continue;
            }
            evt.Event21PartNames[i] = (PartNames[i] == "") ? null : PartNames[i];
        }
        return evt;
    }
}
