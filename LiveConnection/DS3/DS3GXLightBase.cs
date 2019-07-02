using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

public abstract class DS3GXLightBase
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
            float r = BasePointer.ReadSingle(0x70);
            float g = BasePointer.ReadSingle(0x74);
            float b = BasePointer.ReadSingle(0x78);
            return new Color(r, g, b);
        }
        set
        {
            BasePointer.WriteSingle(0x70, value.r);
            BasePointer.WriteSingle(0x74, value.g);
            BasePointer.WriteSingle(0x78, value.b);
        }
    }

    public float DiffusePower
    {
        get
        {
            return BasePointer.ReadSingle(0x7C);
        }
        set
        {
            BasePointer.WriteSingle(0x7C, value);
        }
    }

    public Color Specular
    {
        get
        {
            float r = BasePointer.ReadSingle(0x80);
            float g = BasePointer.ReadSingle(0x84);
            float b = BasePointer.ReadSingle(0x88);
            return new Color(r, g, b);
        }
        set
        {
            BasePointer.WriteSingle(0x80, value.r);
            BasePointer.WriteSingle(0x84, value.g);
            BasePointer.WriteSingle(0x88, value.b);
        }
    }

    public float SpecularPower
    {
        get
        {
            return BasePointer.ReadSingle(0x8C);
        }
        set
        {
            BasePointer.WriteSingle(0x8C, value);
        }
    }

    public abstract float Radius { get; set; }

    public bool EnableShadows
    {
        get
        {
            return false;
            //return BasePointer.ReadByte(0x49) == 1;
        }
        set
        {
            //BasePointer.WriteBoolean(0x49, value);
        }
    }

    public abstract float FlickerMin { get; set; }
    public abstract float FlickerMax { get; set; }
    public abstract float FlickerMult { get; set; }
}
