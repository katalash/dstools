using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

public abstract class DS2GXLightBase
{
    public abstract bool IsValid();

    protected PHPointer BasePointer;
    protected PHook Hook;

    /// <summary>
    /// Index of light in memory
    /// </summary>
    public int Index;

    public abstract Vector3 Position { get; set; }

    public Color Diffuse
    {
        get
        {
            float r = BasePointer.ReadSingle(0x10);
            float g = BasePointer.ReadSingle(0x14);
            float b = BasePointer.ReadSingle(0x18);
            return new Color(r, g, b);
        }
        set
        {
            BasePointer.WriteSingle(0x10, value.r);
            BasePointer.WriteSingle(0x14, value.g);
            BasePointer.WriteSingle(0x18, value.b);
        }
    }

    public float DiffusePower
    {
        get
        {
            return BasePointer.ReadSingle(0x1C);
        }
        set
        {
            BasePointer.WriteSingle(0x1C, value);
        }
    }

    public Color Specular
    {
        get
        {
            float r = BasePointer.ReadSingle(0x20);
            float g = BasePointer.ReadSingle(0x24);
            float b = BasePointer.ReadSingle(0x28);
            return new Color(r, g, b);
        }
        set
        {
            BasePointer.WriteSingle(0x20, value.r);
            BasePointer.WriteSingle(0x24, value.g);
            BasePointer.WriteSingle(0x28, value.b);
        }
    }

    public float SpecularPower
    {
        get
        {
            return BasePointer.ReadSingle(0x2C);
        }
        set
        {
            BasePointer.WriteSingle(0x2C, value);
        }
    }

    public abstract float Radius { get; set; }

    public bool EnableShadows
    {
        get
        {
            return BasePointer.ReadByte(0x49) == 1;
        }
        set
        {
            BasePointer.WriteBoolean(0x49, value);
        }
    }
}
