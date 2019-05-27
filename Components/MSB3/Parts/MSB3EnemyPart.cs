using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Parts/Enemy")]
public class MSB3EnemyPart : MSB3Part
{
    public MSB3GParamConfig GParamConfig;

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

    public string WalkRouteName;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int BackupEventAnimID, UnkT78;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT84;

    public void SetPart(MSB3.Part.Enemy part)
    {
        setBasePart(part);
        GParamConfig = gameObject.AddComponent<MSB3GParamConfig>();
        GParamConfig.setStruct(part.Gparam);
        CollisionName = part.CollisionName;
        ThinkParamID = part.ThinkParamID;
        NPCParamID = part.NPCParamID;
        TalkID = part.TalkID;
        CharaInitID = part.CharaInitID;
        UnkT04 = part.UnkT04;
        ChrManipulatorAllocationParameter = part.ChrManipulatorAllocationParameter;
        WalkRouteName = part.WalkRouteName;
        BackupEventAnimID = part.BackupEventAnimID;
        UnkT78 = part.UnkT78;
        UnkT84 = part.UnkT84;
    }

    public MSB3.Part.Enemy Serialize(GameObject parent)
    {
        var part = new MSB3.Part.Enemy(parent.name);

        _Serialize(part, parent);
        part.Gparam = GParamConfig.Serialize();
        part.CollisionName = (CollisionName == "") ? null : CollisionName;
        part.ThinkParamID = ThinkParamID;
        part.NPCParamID = NPCParamID;
        part.TalkID = TalkID;
        part.CharaInitID = CharaInitID;
        part.UnkT04 = UnkT04;
        part.ChrManipulatorAllocationParameter = ChrManipulatorAllocationParameter;
        part.WalkRouteName = (WalkRouteName == "") ? null : WalkRouteName;
        part.BackupEventAnimID = BackupEventAnimID;
        part.UnkT78 = UnkT78;
        part.UnkT84 = UnkT84;
        return part;
    }
}
