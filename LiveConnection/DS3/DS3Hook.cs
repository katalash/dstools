using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;
using UnityEngine;

/// <summary>
/// Hook into DS2 exe for live editing of certain things like lights
/// </summary>
class DS3Hook : PHook
{
    private PHPointer RendManPointer;

    private DS3GXLightManager LightManager;

    public DS3Hook() : base(5000, 5000, p => p.ProcessName.ToLower() == "darksoulsiii")
    {
        RendManPointer = CreateBasePointer((IntPtr)0x1447809c8);
    }

    public DS3GXLightManager GetLightManager()
    {
        if (LightManager == null || !LightManager.IsValid())
        {
            //Debug.Log("KatanaMainApp: " + String.Format("0x{0:X16}", KatanaMainApp.ReadUInt64(0x0)));
            //Debug.Log("KatanaDrawSystem: " + String.Format("0x{0:X16}", KatanaDrawSystem.ReadUInt64(0x0)));
            //Debug.Log("GXLightManager Pointer: " + String.Format("0x{0:X16}", KatanaDrawSystem.ReadUInt64(0x1D20)));
            LightManager = new DS3GXLightManager(this, CreateChildPointer(RendManPointer, 0x0, 0x10, 0x340, 0xF0, 0x41D0), 0);
            if (!LightManager.IsValid())
            {
                return null;
            }
        }
        return LightManager;
    }
}
