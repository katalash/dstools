using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MSBBBCollisionPart))]
[CanEditMultipleObjects]
public class MSBBBCollisionPartEditor : MSBPartEditorBase
{
    void OnEnable()
    {
        _MSBType = MSBType.MSBBB;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawRenderGroups();
        DrawRawProperties();
    }
}