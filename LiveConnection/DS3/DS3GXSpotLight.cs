using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

public class DS3GXSpotLight : DS3GXLightBase
{
    public DS3GXSpotLight(PHook hook, PHPointer b, int index)
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
        if (BasePointer.ReadUInt64(0x0) != 0x143D3A8B8)
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
            float x = BasePointer.ReadSingle(0x180);
            float y = BasePointer.ReadSingle(0x184);
            float z = BasePointer.ReadSingle(0x188);
            return new Vector3(x, y, z);
        }
        set
        {
            if (!IsValid())
            {
                return;
            }
            BasePointer.WriteSingle(0x180, value.x);
            BasePointer.WriteSingle(0x184, value.y);
            BasePointer.WriteSingle(0x188, value.z);
        }
    }

    public Matrix4x4 Transform
    {
        set
        {
            BasePointer.WriteSingle(0x150, value[0, 0]);
            BasePointer.WriteSingle(0x154, value[1, 0]);
            BasePointer.WriteSingle(0x158, value[2, 0]);
            BasePointer.WriteSingle(0x15C, value[3, 0]);
            BasePointer.WriteSingle(0x160, value[0, 1]);
            BasePointer.WriteSingle(0x164, value[1, 1]);
            BasePointer.WriteSingle(0x168, value[2, 1]);
            BasePointer.WriteSingle(0x16C, value[3, 1]);
            BasePointer.WriteSingle(0x170, value[0, 2]);
            BasePointer.WriteSingle(0x174, value[1, 2]);
            BasePointer.WriteSingle(0x178, value[2, 2]);
            BasePointer.WriteSingle(0x17C, value[3, 2]);
            BasePointer.WriteSingle(0x180, value[0, 3]);
            BasePointer.WriteSingle(0x184, value[1, 3]);
            BasePointer.WriteSingle(0x188, value[2, 3]);
            BasePointer.WriteSingle(0x18C, value[3, 3]);
        }
    }

    public override float Radius
    {
        get
        {
            return 0.0f;
        }
        set
        {
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
