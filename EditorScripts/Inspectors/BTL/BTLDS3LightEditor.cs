using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BTLDS3Light))]
[CanEditMultipleObjects]
public class BTLDS3LightEditor : BTLEditorBase
{
    void OnEnable()
    {
        _BTLType = BTLType.BTL2;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (DarkSoulsTools.GetGameType() == DarkSoulsTools.GameType.DarkSoulsIISOTFS)
        {
            DrawLiveConnectionStatus();
        }
        DrawRawProperties();
    }
}