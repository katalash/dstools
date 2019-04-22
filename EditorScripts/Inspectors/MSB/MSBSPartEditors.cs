using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MSBSObjectPart))]
[CanEditMultipleObjects]
public class MSBSObjectPartEditor : MSBPartEditorBase
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