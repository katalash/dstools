using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DS2LiveConnection
{
    public enum ConnectionStatus
    {
        StatusStopped,
        StatusConnecting,
        StatusConnected,
    }

    private static ConnectionStatus Status = ConnectionStatus.StatusStopped;
    private static DS2SOTFSHook Hook = null;

    public static void Connect()
    {
        if (Hook == null)
        {
            Hook = new DS2SOTFSHook();
        }
        Hook.Start();
        Status = ConnectionStatus.StatusConnecting;
    }

    public static void Stop()
    {
        if (Hook != null)
        {
            Hook.Stop();
            Hook = null;
        }
        Status = ConnectionStatus.StatusStopped;
    }

    public static ConnectionStatus GetStatus()
    {
        if (Hook != null && Status != ConnectionStatus.StatusStopped)
        {
            if (Hook.Hooked)
            {
                Status = ConnectionStatus.StatusConnected;
            }
            else
            {
                Status = ConnectionStatus.StatusConnecting;
            }
        }
        return Status;
    }

    public static DS2GXLightManager GetLightManager()
    {
        if (Hook != null)
        {
            return Hook.GetLightManager();
        }
        return null;
    }
}
