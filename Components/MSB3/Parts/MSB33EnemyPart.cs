using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Enemy")]
public class MSB3EnemyPart : MSB3Part
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public string CollisionName;

    /// <summary>
    /// Controls enemy AI.
    /// </summary>
    public int ThinkParamID;

    /// <summary>
    /// Controls enemy stats.
    /// </summary>
    public int NPCParamID;

    /// <summary>
    /// Controls enemy speech.
    /// </summary>
    public int TalkID;

    /// <summary>
    /// Controls enemy equipment.
    /// </summary>
    public int CharaInitID;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT04, ChrManipulatorAllocationParameter;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT20, BackupEventAnimID, UnkT78;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT84;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT8C, UnkT94, UnkT9C, UnkTA4, UnkTAC, UnkTC0, UnkTC4, UnkTC8, UnkTCC;

    public void SetPart(MSB3.Part.Enemy part)
    {
        setBasePart(part);
        CollisionName = part.CollisionName;
        ThinkParamID = part.ThinkParamID;
        NPCParamID = part.NPCParamID;
        TalkID = part.TalkID;
        CharaInitID = part.CharaInitID;
        UnkT04 = part.UnkT04;
        ChrManipulatorAllocationParameter = part.ChrManipulatorAllocationParameter;
        UnkT20 = part.UnkT20;
        BackupEventAnimID = part.BackupEventAnimID;
        UnkT78 = part.UnkT78;
        UnkT84 = part.UnkT84;
        UnkT8C = part.UnkT8C;
        UnkT94 = part.UnkT94;
        UnkT9C = part.UnkT9C;
        UnkTA4 = part.UnkTA4;
        UnkTAC = part.UnkTAC;
        UnkTC0 = part.UnkTC0;
        UnkTC4 = part.UnkTC4;
        UnkTC8 = part.UnkTC8;
        UnkTCC = part.UnkTCC;
    }

    public MSB3.Part.Enemy Serialize(GameObject parent)
    {
        var part = new MSB3.Part.Enemy(ID, parent.name);

        _Serialize(part, parent);
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkID = TalkID;
        part.CharaInitID = CharaInitID;
        part.UnkT04 = UnkT04;
        part.ChrManipulatorAllocationParameter = ChrManipulatorAllocationParameter;
        part.UnkT20 = UnkT20;
        part.BackupEventAnimID = BackupEventAnimID;
        part.UnkT78 = UnkT78;
        part.UnkT84 = UnkT84;
        part.UnkT8C = UnkT8C;
        part.UnkT94 = UnkT94;
        part.UnkT9C = UnkT9C;
        part.UnkTA4 = UnkTA4;
        part.UnkTAC = UnkTAC;
        part.UnkTC0 = UnkTC0;
        part.UnkTC4 = UnkTC4;
        part.UnkTC8 = UnkTC8;
        part.UnkTCC = UnkTCC;
        return part;
    }
}
