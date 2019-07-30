using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Parts/Collision")]
public class MSB2CollisionPart : MSB2Part
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT04;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT08;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT0C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT10;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte UnkT12;

    /// <summary>
    /// Unknown.
    /// </summary>
    public byte UnkT13;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT14;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT18;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT1C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT20;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT24;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT26;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT28;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT2C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT2E;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT30;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT34;

    /// <summary>
    /// Unknown.
    /// </summary>
    public short UnkT36;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT38;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT3C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT40;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT44;

    public override void SetPart(MSB2.Part bpart)
    {
        var part = (MSB2.Part.Collision)bpart;
        setBasePart(part);

        UnkT00 = part.UnkT00;
        UnkT04 = part.UnkT04;
        UnkT08 = part.UnkT08;
        UnkT0C = part.UnkT0C;
        UnkT10 = part.UnkT10;
        UnkT12 = part.UnkT12;
        UnkT13 = part.UnkT13;
        UnkT14 = part.UnkT14;
        UnkT18 = part.UnkT18;
        UnkT1C = part.UnkT1C;
        UnkT20 = part.UnkT20;
        UnkT24 = part.UnkT24;
        UnkT26 = part.UnkT26;
        UnkT28 = part.UnkT28;
        UnkT2C = part.UnkT2C;
        UnkT2E = part.UnkT2E;
        UnkT30 = part.UnkT30;
        UnkT34 = part.UnkT34;
        UnkT36 = part.UnkT36;
        UnkT38 = part.UnkT38;
        UnkT3C = part.UnkT3C;
        UnkT40 = part.UnkT40;
        UnkT44 = part.UnkT44;
    }

    public override MSB2.Part Serialize(GameObject parent)
    {
        var part = new MSB2.Part.Collision();
        _Serialize(part, parent);

        part.UnkT00 = UnkT00;
        part.UnkT04 = UnkT04;
        part.UnkT08 = UnkT08;
        part.UnkT0C = UnkT0C;
        part.UnkT10 = UnkT10;
        part.UnkT12 = UnkT12;
        part.UnkT13 = UnkT13;
        part.UnkT14 = UnkT14;
        part.UnkT18 = UnkT18;
        part.UnkT1C = UnkT1C;
        part.UnkT20 = UnkT20;
        part.UnkT24 = UnkT24;
        part.UnkT26 = UnkT26;
        part.UnkT28 = UnkT28;
        part.UnkT2C = UnkT2C;
        part.UnkT2E = UnkT2E;
        part.UnkT30 = UnkT30;
        part.UnkT34 = UnkT34;
        part.UnkT36 = UnkT36;
        part.UnkT38 = UnkT38;
        part.UnkT3C = UnkT3C;
        part.UnkT40 = UnkT40;
        part.UnkT44 = UnkT44;
        return part;
    }
}
