using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MSB2ObjectPart))]
[CanEditMultipleObjects]
public class MSB2ObjectPartEditor : MSBPartEditorBase
{
    void OnEnable()
    {
        _MSBType = MSBType.MSB2SOTFS;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //DrawTreasureEditor();
        DrawRawProperties();
        DrawRefreshModel();
    }
}