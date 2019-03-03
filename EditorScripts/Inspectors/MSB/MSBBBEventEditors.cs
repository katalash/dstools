using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MSBBBTreasureEvent))]
[CanEditMultipleObjects]
public class MSBBBTreasureEventEditor : MSBEventEditorBase
{
    void OnEnable()
    {
        _MSBType = MSBType.MSBBB;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTreasureEditor();
        DrawRawProperties();
    }
}