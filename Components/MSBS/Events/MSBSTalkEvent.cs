using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Talk")]
public class MSBSTalkEvent : MSBSEvent
{

    public int UnkT00;
    public string[] EnemyNames;
    public int[] TalkIDs;
    public short UnkT44;
    public short UnkT46;
    public int UnkT48;
    public void SetEvent(MSBS.Event.Talk evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        EnemyNames = evt.EnemyNames;
        TalkIDs = evt.TalkIDs;
        UnkT44 = evt.UnkT44;
        UnkT46 = evt.UnkT46;
        UnkT48 = evt.UnkT48;

    }

    public MSBS.Event.Talk Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Talk();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        for (int i = 0; i < 8; i++)
        {
            if (i >= EnemyNames.Length)
                break;
            evt.EnemyNames[i] = (EnemyNames[i] == "") ? null : EnemyNames[i];
        }
        for (int i = 0; i < 8; i++)
        {
            if (i >= TalkIDs.Length)
                break;
            evt.TalkIDs[i] = TalkIDs[i];
        }
        evt.UnkT44 = UnkT44;
        evt.UnkT46 = UnkT46;
        evt.UnkT48 = UnkT48;
        return evt;
    }
}
