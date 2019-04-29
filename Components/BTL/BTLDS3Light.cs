using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SoulsFormats;

class BTLDS3Light : MonoBehaviour
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk00;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk04;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk08;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk0C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk18;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool Unk1C;

    /// <summary>
    /// Color of the light on diffuse surfaces.
    /// </summary>
    public Color DiffuseColor;

    /// <summary>
    /// Intensity of diffuse lighting.
    /// </summary>
    public float DiffusePower;

    /// <summary>
    /// Color of the light on reflective surfaces.
    /// </summary>
    public Color SpecularColor;

    /// <summary>
    /// Unknown.
    /// </summary>
    public bool CastsShadows;

    /// <summary>
    /// Intensity of specular lighting.
    /// </summary>
    public float SpecularPower;

    /// <summary>
    /// Tightness of the spot light beam.
    /// </summary>
    public float ConeAngle;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk30;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk34;

    /// <summary>
    /// Rotation of a spot light.
    /// </summary>
    public Vector3 Rotation;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk50;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk54;

    /// <summary>
    /// Distance the light shines.
    /// </summary>
    public float Radius;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk5C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int Unk64;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk68;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk6C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk70;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk74;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk78;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk7C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk98;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float Unk9C;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkA0;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkA4;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkAC;

    /// <summary>
    /// Stretches the spot light beam.
    /// </summary>
    public float Width;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkBC;

    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkC0;

    /// <summary>
    /// Unknown.
    /// </summary>
    public float UnkC4;

    public virtual void SetFromLight(BTL.Light l)
    {
        Unk00 = l.Unk00;
        Unk04 = l.Unk04;
        Unk08 = l.Unk08;
        Unk0C = l.Unk0C;
        Unk18 = l.Unk18;
        Unk1C = l.Unk1C;
        DiffuseColor = new Color((float)l.DiffuseColor.R / 255.0f, (float)l.DiffuseColor.G / 255.0f, (float)l.DiffuseColor.B / 255.0f, (float)l.DiffuseColor.A / 255.0f);
        DiffusePower = l.DiffusePower;
        SpecularColor = new Color((float)l.SpecularColor.R / 255.0f, (float)l.SpecularColor.G / 255.0f, (float)l.SpecularColor.B / 255.0f, (float)l.SpecularColor.A / 255.0f);
        SpecularPower = l.SpecularPower;
        ConeAngle = l.ConeAngle;
        CastsShadows = l.Unk27;
        Unk30 = l.Unk30;
        Unk34 = l.Unk34;
        Rotation = new Vector3(l.Rotation.X * Mathf.Rad2Deg, l.Rotation.Y * Mathf.Rad2Deg, l.Rotation.Z * Mathf.Rad2Deg);
        Unk50 = l.Unk50;
        Unk54 = l.Unk54;
        Radius = l.Radius;
        Unk5C = l.Unk5C;
        Unk64 = l.Unk64;
        Unk68 = l.Unk68;
        Unk6C = l.Unk6C;
        Unk70 = l.Unk70;
        Unk74 = l.Unk74;
        Unk78 = l.Unk78;
        Unk7C = l.Unk7C;
        Unk98 = l.Unk98;
        Unk9C = l.Unk9C;
        UnkA0 = l.UnkA0;
        UnkA4 = l.UnkA4;
        UnkAC = l.UnkAC;
        Width = l.Width;
        UnkBC = l.UnkBC;
        UnkC0 = l.UnkC0;
        UnkC4 = l.UnkC4;
    }

    public virtual BTL.Light Serialize(GameObject parent, BTL.Light light = null)
    {
        BTL.Light l = light;
        if (light == null)
        {
            l = new BTL.Light();
        }
        l.Name = parent.name;
        l.Position = new System.Numerics.Vector3(parent.transform.localPosition.x, parent.transform.localPosition.y, parent.transform.localPosition.z);
        l.Unk00 = Unk00;
        l.Unk04 = Unk04;
        l.Unk08 = Unk08;
        l.Unk0C = Unk0C;
        l.Unk18 = Unk18;
        l.Unk1C = Unk1C;
        l.DiffuseColor = System.Drawing.Color.FromArgb((byte)(DiffuseColor.a * 255.0f), (byte)(DiffuseColor.r * 255.0f), (byte)(DiffuseColor.g * 255.0f), (byte)(DiffuseColor.b * 255.0f));
        l.DiffusePower = DiffusePower;
        l.SpecularColor = System.Drawing.Color.FromArgb((byte)(SpecularColor.a * 255.0f), (byte)(SpecularColor.r * 255.0f), (byte)(SpecularColor.g * 255.0f), (byte)(SpecularColor.b * 255.0f));
        l.SpecularPower = SpecularPower;
        l.ConeAngle = ConeAngle;
        l.Unk27 = CastsShadows;
        l.Unk30 = Unk30;
        l.Unk34 = Unk34;
        l.Rotation = new System.Numerics.Vector3(Rotation.x * Mathf.Deg2Rad, Rotation.y * Mathf.Deg2Rad, Rotation.z * Mathf.Deg2Rad);
        l.Unk50 = Unk50;
        l.Unk54 = Unk54;
        l.Radius = Radius;
        l.Unk5C = Unk5C;
        l.Unk64 = Unk64;
        l.Unk68 = Unk68;
        l.Unk6C = Unk6C;
        l.Unk70 = Unk70;
        l.Unk74 = Unk74;
        l.Unk78 = Unk78;
        l.Unk7C = Unk7C;
        l.Unk98 = Unk98;
        l.Unk9C = Unk9C;
        l.UnkA0 = UnkA0;
        l.UnkA4 = UnkA4;
        l.UnkAC = UnkAC;
        l.Width = Width;
        l.UnkBC = UnkBC;
        l.UnkC0 = UnkC0;
        l.UnkC4 = UnkC4;
        return l;
    }
}
