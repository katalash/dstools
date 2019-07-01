using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

public class DS2GXPointLight : DS2GXLightBase
{
    public DS2GXPointLight(PHook hook, PHPointer b, int index)
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
        if (BasePointer.ReadUInt64(0x0) != 0x1411DA3D0)
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
            float x = BasePointer.ReadSingle(0x50);
            float y = BasePointer.ReadSingle(0x54);
            float z = BasePointer.ReadSingle(0x58);
            return new Vector3(x, y, z);
        }
        set
        {
            if (!IsValid())
            {
                return;
            }
            BasePointer.WriteSingle(0x50, value.x);
            BasePointer.WriteSingle(0x54, value.y);
            BasePointer.WriteSingle(0x58, value.z);
        }
    }

    public override float Radius
    {
        get
        {
            return BasePointer.ReadSingle(0x5C);
        }
        set
        {
            BasePointer.WriteSingle(0x5C, value);
        }
    }
}
