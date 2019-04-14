using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Event 23")]
public class MSBSEvent23Event : MSBSEvent
{

    public int UnkT00;
    public int[] UnkT04;
    public int[] UnkT24;
    public short UnkT44;
    public short UnkT46;
    public int UnkT48;
    public void SetEvent(MSBS.Event.Event23 evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        UnkT04 = evt.UnkT04;
        UnkT24 = evt.UnkT24;
        UnkT44 = evt.UnkT44;
        UnkT46 = evt.UnkT46;
        UnkT48 = evt.UnkT48;

    }

    public MSBS.Event.Event23 Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.Event23();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        for (int i = 0; i < 8; i++)
        {
            if (i >= UnkT04.Length)
                break;
            evt.UnkT04[i] = UnkT04[i];
        }
        for (int i = 0; i < 8; i++)
        {
            if (i >= UnkT24.Length)
                break;
            evt.UnkT24[i] = UnkT24[i];
        }
        evt.UnkT44 = UnkT44;
        evt.UnkT46 = UnkT46;
        evt.UnkT48 = UnkT48;
        return evt;
    }
}
