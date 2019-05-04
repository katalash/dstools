using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MSBSTreasureEvent))]
[CanEditMultipleObjects]
public class MSBSTreasureEventEditor : MSBEventEditorBase
{
    void OnEnable()
    {
        _MSBType = MSBType.MSBSekiro;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTreasureEditor();
        DrawRawProperties();
    }
}