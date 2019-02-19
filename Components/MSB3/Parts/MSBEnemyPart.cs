using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

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
    /// Unknown, probably more paramIDs.
    /// </summary>
    public int UnkT04, UnkT07, UnkT08, UnkT09;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT11, UnkT12, UnkT13, UnkT14, UnkT15, UnkT16, UnkT17, UnkT18, UnkT19;

    public void SetPart(MSB3.Part.Enemy part)
    {
        setBasePart(part);
        CollisionName = part.CollisionName;
        ThinkParamID = part.ThinkParamID;
        NPCParamID = part.NPCParamID;
        TalkID = part.TalkID;
        CharaInitID = part.CharaInitID;
        UnkT04 = part.UnkT04;
        UnkT07 = part.UnkT07;
        UnkT08 = part.UnkT08;
        UnkT09 = part.UnkT09;
        UnkT10 = part.UnkT10;
        UnkT11 = part.UnkT11;
        UnkT12 = part.UnkT12;
        UnkT13 = part.UnkT13;
        UnkT14 = part.UnkT14;
        UnkT15 = part.UnkT15;
        UnkT16 = part.UnkT16;
        UnkT17 = part.UnkT17;
        UnkT18 = part.UnkT18;
        UnkT19 = part.UnkT19;
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
        part.UnkT07 = UnkT07;
        part.UnkT08 = UnkT08;
        part.UnkT09 = UnkT09;
        part.UnkT10 = UnkT10;
        part.UnkT11 = UnkT11;
        part.UnkT12 = UnkT12;
        part.UnkT13 = UnkT13;
        part.UnkT14 = UnkT14;
        part.UnkT15 = UnkT15;
        part.UnkT16 = UnkT16;
        part.UnkT17 = UnkT17;
        part.UnkT18 = UnkT18;
        part.UnkT19 = UnkT19;
        return part;
    }
}
