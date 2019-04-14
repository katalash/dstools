using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Enemy")]
public class MSBSEnemyPart : MSBSDummyEnemyPart
{
    public MSBSUnkStruct1Part Unk1;


    public void SetPart(MSBS.Part.Enemy part)
    {
        base.SetPart(part);
        Unk1 = gameObject.AddComponent<MSBSUnkStruct1Part>();
        Unk1.setStruct(part.Unk1);
    }

    public new MSBS.Part.Enemy Serialize(GameObject parent)
    {
        var part = new MSBS.Part.Enemy();
        _Serialize(part, parent);
        part.Unk5 = Unk5.Serialize();
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkParamID = TalkParamID;
        part.ChrManipulatorAllocationParameter = ChrManipulatorAllocationParameter;
        part.CharaInitID = CharaInitID;
        part.CollisionPartName = CollisionPartName;
        part.UnkT20 = UnkT20;
        part.UnkT22 = UnkT22;
        part.UnkT24 = UnkT24;
        part.BackupEventAnimID = BackupEventAnimID;
        part.EventFlagID = EventFlagID;
        part.EventFlagCompareState = EventFlagCompareState;
        part.UnkT48 = UnkT48;
        part.UnkT4C = UnkT4C;
        part.UnkT50 = UnkT50;
        part.UnkT78 = UnkT78;
        part.UnkT84 = UnkT84;
        part.Unk1 = Unk1.Serialize();
        return part;
    }
}
