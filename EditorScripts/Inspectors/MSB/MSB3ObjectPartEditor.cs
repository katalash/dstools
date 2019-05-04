using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MSB3ObjectPart))]
[CanEditMultipleObjects]
public class MSB3ObjectPartEditor : MSBPartEditorBase
{
    void OnEnable()
    {
        _MSBType = MSBType.MSB3;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTreasureEditor();
        DrawRawProperties();
    }
}