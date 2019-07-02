using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Common functionality for light editing including live connection to game
/// </summary>
public abstract class BTLEditorBase : Editor
{
    static bool ShowDefaultInspector = true;
    protected enum BTLType
    {
        BTL2,
        BTL3,
        BTLBB,
        BTLSekiro,
    }
    protected BTLType _BTLType = BTLType.BTL2;

    protected void DrawLiveConnectionStatusDS2()
    {
        if (DS2LiveConnection.GetStatus() == DS2LiveConnection.ConnectionStatus.StatusStopped)
        {
            DS2LiveConnection.Connect();
        }
        if (DS2LiveConnection.GetStatus() == DS2LiveConnection.ConnectionStatus.StatusConnecting)
        {
            EditorGUILayout.LabelField("Attempting to connect to DS2 Game");
            if (GUILayout.Button("Force Reattempt"))
            {
                DS2LiveConnection.Stop();
                DS2LiveConnection.Connect();
            }
        }
        else if (DS2LiveConnection.GetStatus() == DS2LiveConnection.ConnectionStatus.StatusConnected)
        {
            EditorGUILayout.LabelField("Live Connected to DS2");
            var lightman = DS2LiveConnection.GetLightManager();
            if (lightman != null)
            {
                var pos = ((BTLDS3Light)target).gameObject.transform.position;
                var light = lightman.FindLightByPosition(pos);
                if (light != null)
                {
                    EditorGUILayout.LabelField("Connected to Light. Index " + light.Index);
                    ((BTLDS3Light)target).SetConnectedLight(light);
                }
                else
                {
                    EditorGUILayout.LabelField("Could not find in game light.");
                    EditorGUILayout.LabelField("Make sure you are in the correct level.");
                    EditorGUILayout.LabelField("Try exporting the BTL and reloading the map.");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Could not find Light Manager. You are likely not in the game");
            }
        }
    }

    protected void DrawLiveConnectionStatusDS3()
    {
        if (DS3LiveConnection.GetStatus() == DS3LiveConnection.ConnectionStatus.StatusStopped)
        {
            DS3LiveConnection.Connect();
        }
        if (DS3LiveConnection.GetStatus() == DS3LiveConnection.ConnectionStatus.StatusConnecting)
        {
            EditorGUILayout.LabelField("Attempting to connect to DS3 Game");
            if (GUILayout.Button("Force Reattempt"))
            {
                DS3LiveConnection.Stop();
                DS3LiveConnection.Connect();
            }
        }
        else if (DS3LiveConnection.GetStatus() == DS3LiveConnection.ConnectionStatus.StatusConnected)
        {
            EditorGUILayout.LabelField("Live Connected to DS3");
            var lightman = DS3LiveConnection.GetLightManager();
            if (lightman != null)
            {
                var pos = ((BTLDS3Light)target).gameObject.transform.position;
                var light = lightman.FindLightByPosition(pos);
                if (light != null)
                {
                    EditorGUILayout.LabelField("Connected to Light. Index " + light.Index);
                    ((BTLDS3Light)target).SetConnectedLight(light);
                }
                else
                {
                    EditorGUILayout.LabelField("Could not find in game light.");
                    EditorGUILayout.LabelField("Make sure you are in the correct level.");
                    EditorGUILayout.LabelField("Try exporting the BTL and reloading the map.");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Could not find Light Manager. You are likely not in the game");
            }
        }
    }

    protected void DrawRawProperties()
    {
        ShowDefaultInspector = EditorGUILayout.Foldout(ShowDefaultInspector, "Raw BTL Properties");
        if (ShowDefaultInspector)
        {
            DrawDefaultInspector();
        }
    }
}
