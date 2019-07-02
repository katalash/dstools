using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

public class DS3GXPointLight : DS3GXLightBase
{
    public DS3GXPointLight(PHook hook, PHPointer b, int index)
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
        if (BasePointer.ReadUInt64(0x0) != 0x143D3A7D8)
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
            float x = BasePointer.ReadSingle(0x150);
            float y = BasePointer.ReadSingle(0x154);
            float z = BasePointer.ReadSingle(0x158);
            return new Vector3(x, y, z);
        }
        set
        {
            if (!IsValid())
            {
                return;
            }
            BasePointer.WriteSingle(0x150, value.x);
            BasePointer.WriteSingle(0x154, value.y);
            BasePointer.WriteSingle(0x158, value.z);
        }
    }

    public override float Radius
    {
        get
        {
            return BasePointer.ReadSingle(0x15C);
        }
        set
        {
            BasePointer.WriteSingle(0x15C, value);
        }
    }

    public override float FlickerMin
    {
        get
        {
            return BasePointer.ReadSingle(0x74);
        }
        set
        {
            //BasePointer.WriteSingle(0x74, value);
        }
    }

    public override float FlickerMax
    {
        get
        {
            return BasePointer.ReadSingle(0x78);
        }
        set
        {
            //BasePointer.WriteSingle(0x78, value);
        }
    }

    public override float FlickerMult
    {
        get
        {
            return BasePointer.ReadSingle(0x7C);
        }
        set
        {
            //BasePointer.WriteSingle(0x7C, value);
        }
    }
}
