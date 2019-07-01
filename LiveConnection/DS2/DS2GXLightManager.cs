using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

// Hook into DS2's light manager
class DS2GXLightManager
{
    private PHook Hook;
    private PHPointer BasePointer;
    private int Offset;

    public bool IsValid()
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
        if (BasePointer.ReadUInt64(Offset) != 0x1411D77F8)
        {
            Debug.Log("Lightman base: " + String.Format("0x{0:X8}", BasePointer.Resolve().ToInt64()));
            Debug.Log("Lightman invalid pointer: " + String.Format("0x{0:X8}", BasePointer.ReadUInt64(0x0)));
            return false;
        }
        return true;
    }

    public DS2GXLightManager(PHook hook, PHPointer b, int offset)
    {
        Hook = hook;
        BasePointer = b;
        Offset = offset;
    }

    public List<DS2GXLightBase> GetLights()
    {
        if (!IsValid())
        {
            return null;
        }
        PHPointer arrayBase = Hook.CreateChildPointer(BasePointer, Offset + 0x10);
        PHPointer arrayTail = Hook.CreateChildPointer(BasePointer, Offset + 0x18);
        long size = (arrayTail.Resolve().ToInt64() - arrayBase.Resolve().ToInt64()) / 8;
        List<DS2GXLightBase> lights = new List<DS2GXLightBase>();
        for (int i = 0; i < size; i++)
        {
            PHPointer light = Hook.CreateChildPointer(arrayBase, i * 8);
            ulong vtable = light.ReadUInt64(0);
            if (vtable == 0x1411DA3D0)
            {
                lights.Add(new DS2GXPointLight(Hook, light, i));
            }
            else if (vtable == 0x1411D7808)
            {
                lights.Add(new DS2GXSpotLight(Hook, light, i));
            }
        }
        return lights;
    }

    // Find the closest light to a position within a tolerance
    public DS2GXLightBase FindLightByPosition(Vector3 pos, float tolerance=0.1f)
    {
        var lights = GetLights();
        if (lights == null)
        {
            return null;
        }
        DS2GXLightBase closest = null;
        float mindist = float.MaxValue;
        foreach (var light in lights)
        {
            float dist = Vector3.Distance(pos, light.Position);
            if (dist <= tolerance && dist < mindist)
            {
                closest = light;
                mindist = dist;
            }
        }
        return closest;
    }
}
