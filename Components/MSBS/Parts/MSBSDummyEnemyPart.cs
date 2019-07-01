using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Parts/Dummy Enemy")]
public class MSBSDummyEnemyPart : MSBSPart
{
    public MSBSGParamConfig Gparam;
    public int ThinkParamID;
    public int NPCParamID;
    public int UnkT10;
    public short ChrManipulatorAllocationParameter;
    public int CharaInitID;
    public string CollisionPartName;
    public short UnkT20;
    public short UnkT22;
    public int UnkT24;
    public int BackupEventAnimID;
    public int EventFlagID;
    public int EventFlagCompareState;
    public int UnkT48;
    public int UnkT4C;
    public int UnkT50;
    public int UnkT78;
    public float UnkT84;

    public void SetPart(MSBS.Part.DummyEnemy part)
    {
        setBasePart(part);
        Gparam = gameObject.AddComponent<MSBSGParamConfig>();
        Gparam.setStruct(part.Gparam);
        ThinkParamID = part.ThinkParamID;
        NPCParamID = part.NPCParamID;
        UnkT10 = part.UnkT10;
        ChrManipulatorAllocationParameter = part.ChrManipulatorAllocationParameter;
        CharaInitID = part.CharaInitID;
        CollisionPartName = part.CollisionPartName;
        UnkT20 = part.UnkT20;
        UnkT22 = part.UnkT22;
        UnkT24 = part.UnkT24;
        BackupEventAnimID = part.BackupEventAnimID;
        EventFlagID = part.EventFlagID;
        EventFlagCompareState = part.EventFlagCompareState;
        UnkT48 = part.UnkT48;
        UnkT4C = part.UnkT4C;
        UnkT50 = part.UnkT50;
        UnkT78 = part.UnkT78;
        UnkT84 = part.UnkT84;
    }

    public MSBS.Part.DummyEnemy Serialize(GameObject parent)
    {
        var part = new MSBS.Part.DummyEnemy();
        _Serialize(part, parent);
        part.Gparam = Gparam.Serialize();
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.UnkT10 = UnkT10;
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
        return part;
    }
}
