using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

// Hook into DS3's light manager
class DS3GXLightManager
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
        if (BasePointer.ReadUInt64(Offset) != 0x143D08498)
        {
            Debug.Log("Lightman base: " + String.Format("0x{0:X8}", BasePointer.Resolve().ToInt64()));
            Debug.Log("Lightman invalid pointer: " + String.Format("0x{0:X8}", BasePointer.ReadUInt64(0x0)));
            return false;
        }
        Debug.Log("Found lightman base: " + String.Format("0x{0:X8}", BasePointer.Resolve().ToInt64()));
        return true;
    }

    public DS3GXLightManager(PHook hook, PHPointer b, int offset)
    {
        Hook = hook;
        BasePointer = b;
        Offset = offset;
    }

    public List<DS3GXLightBase> GetLights()
    {
        if (!IsValid())
        {
            return null;
        }
        PHPointer arrayBase = Hook.CreateChildPointer(BasePointer, Offset + 0x10);
        PHPointer arrayTail = Hook.CreateChildPointer(BasePointer, Offset + 0x18);
        long size = (arrayTail.Resolve().ToInt64() - arrayBase.Resolve().ToInt64()) / 8;
        List<DS3GXLightBase> lights = new List<DS3GXLightBase>();
        for (int i = 0; i < size; i++)
        {
            PHPointer light = Hook.CreateChildPointer(arrayBase, i * 8);
            ulong vtable = light.ReadUInt64(0);
            if (vtable == 0x143D3A7D8)
            {
                lights.Add(new DS3GXPointLight(Hook, light, i));
            }
            else if (vtable == 0x143D3A8B8)
            {
                lights.Add(new DS3GXSpotLight(Hook, light, i));
            }
        }
        return lights;
    }

    // Find the closest light to a position within a tolerance
    public DS3GXLightBase FindLightByPosition(Vector3 pos, float tolerance=0.1f)
    {
        var lights = GetLights();
        if (lights == null)
        {
            return null;
        }
        DS3GXLightBase closest = null;
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
