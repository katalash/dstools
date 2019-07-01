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
class DS2SOTFSHook : PHook
{
    private PHPointer KatanaMainAppPointer;
    private PHPointer KatanaMainApp;
    private PHPointer KatanaDrawSystem;

    private DS2GXLightManager LightManager;

    public DS2SOTFSHook() : base(5000, 5000, p => p.ProcessName.ToLower() == "darksoulsii")
    {
        KatanaMainAppPointer = CreateBasePointer((IntPtr)0x14166C1D8);
        KatanaMainApp = CreateChildPointer(KatanaMainAppPointer, 0x0);
        KatanaDrawSystem = CreateChildPointer(KatanaMainApp, 0x348);
    }

    public DS2GXLightManager GetLightManager()
    {
        if (LightManager == null || !LightManager.IsValid())
        {
            Debug.Log("KatanaMainApp: " + String.Format("0x{0:X16}", KatanaMainApp.ReadUInt64(0x0)));
            Debug.Log("KatanaDrawSystem: " + String.Format("0x{0:X16}", KatanaDrawSystem.ReadUInt64(0x0)));
            Debug.Log("GXLightManager Pointer: " + String.Format("0x{0:X16}", KatanaDrawSystem.ReadUInt64(0x1D20)));
            LightManager = new DS2GXLightManager(this, KatanaDrawSystem, 0x1D20);
            if (!LightManager.IsValid())
            {
                return null;
            }
        }
        return LightManager;
    }
}
