using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

public class DS2GXSpotLight : DS2GXLightBase
{
    public DS2GXSpotLight(PHook hook, PHPointer b, int index)
    {
        Hook = hook;
        BasePointer = b;
        Index = index;
    }

    public override bool IsValid()
    {
        if (!Hook.Hooked)
        {
            return false;
        }
        if (BasePointer == null)
        {
            return false;
        }
        // See if vtable pointer is valid
        if (BasePointer.ReadUInt64(0x0) != 0x1411D7808)
        {
            return false;
        }
        return true;
    }

    public override Vector3 Position
    {
        get
        {
            if (!IsValid())
            {
                return new Vector3(0.0f, 0.0f, 0.0f);
            }
            float x = BasePointer.ReadSingle(0x80);
            float y = BasePointer.ReadSingle(0x84);
            float z = BasePointer.ReadSingle(0x88);
            return new Vector3(x, y, z);
        }
        set
        {
            if (!IsValid())
            {
                return;
            }
            BasePointer.WriteSingle(0x80, value.x);
            BasePointer.WriteSingle(0x84, value.y);
            BasePointer.WriteSingle(0x88, value.z);
        }
    }

    public Matrix4x4 Transform
    {
        set
        {
            BasePointer.WriteSingle(0x50, value[0, 0]);
            BasePointer.WriteSingle(0x54, value[1, 0]);
            BasePointer.WriteSingle(0x58, value[2, 0]);
            BasePointer.WriteSingle(0x5C, value[3, 0]);
            BasePointer.WriteSingle(0x60, value[0, 1]);
            BasePointer.WriteSingle(0x64, value[1, 1]);
            BasePointer.WriteSingle(0x68, value[2, 1]);
            BasePointer.WriteSingle(0x6C, value[3, 1]);
            BasePointer.WriteSingle(0x70, value[0, 2]);
            BasePointer.WriteSingle(0x74, value[1, 2]);
            BasePointer.WriteSingle(0x78, value[2, 2]);
            BasePointer.WriteSingle(0x7C, value[3, 2]);
            BasePointer.WriteSingle(0x80, value[0, 3]);
            BasePointer.WriteSingle(0x84, value[1, 3]);
            BasePointer.WriteSingle(0x88, value[2, 3]);
            BasePointer.WriteSingle(0x8C, value[3, 3]);
        }
    }

    public Matrix4x4 Projection
    {
        set
        {
            BasePointer.WriteSingle(0x90, value[0, 0]);
            BasePointer.WriteSingle(0x94, value[1, 0]);
            BasePointer.WriteSingle(0x98, value[2, 0]);
            BasePointer.WriteSingle(0x9C, value[3, 0]);
            BasePointer.WriteSingle(0xA0, value[0, 1]);
            BasePointer.WriteSingle(0xA4, value[1, 1]);
            BasePointer.WriteSingle(0xA8, value[2, 1]);
            BasePointer.WriteSingle(0xAC, value[3, 1]);
            BasePointer.WriteSingle(0xB0, value[0, 2]);
            BasePointer.WriteSingle(0xB4, value[1, 2]);
            BasePointer.WriteSingle(0xB8, value[2, 2]);
            BasePointer.WriteSingle(0xBC, value[3, 2]);
            BasePointer.WriteSingle(0xC0, value[0, 3]);
            BasePointer.WriteSingle(0xC4, value[1, 3]);
            BasePointer.WriteSingle(0xC8, value[2, 3]);
            BasePointer.WriteSingle(0xCC, value[3, 3]);
        }
    }

    public override float Radius
    {
        get
        {
            return BasePointer.ReadSingle(0xD4);
        }
        set
        {
            BasePointer.WriteSingle(0xD4, value);
        }
    }

    public float NearClip
    {
        get
        {
            return BasePointer.ReadSingle(0xD0);
        }
        set
        {
            BasePointer.WriteSingle(0xD0, value);
        }
    }

    public float FieldOfView
    {
        get
        {
            return BasePointer.ReadSingle(0xD8);
        }
        set
        {
            BasePointer.WriteSingle(0xD8, value);
        }
    }

    public float FieldOfViewWidth
    {
        get
        {
            return BasePointer.ReadSingle(0xDC);
        }
        set
        {
            BasePointer.WriteSingle(0xDC, value);
        }
    }

    public float FieldOfViewRatio
    {
        get
        {
            return BasePointer.ReadSingle(0xE0);
        }
        set
        {
            BasePointer.WriteSingle(0xE0, value);
        }
    }

    public override float FlickerMin
    {
        get
        {
            return 0.0f;
        }
        set
        {
        }
    }

    public override float FlickerMax
    {
        get
        {
            return 0.0f;
        }
        set
        {
        }
    }

    public override float FlickerMult
    {
        get
        {
            return 0.0f;
        }
        set
        {
        }
    }
}
