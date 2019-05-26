using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MSB1CollisionPart))]
[CanEditMultipleObjects]
public class MSB1CollisionPartEditor : MSBPartEditorBase
{
    void OnEnable()
    {
        _MSBType = MSBType.MSB1;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawRenderGroups();
        DrawRawProperties();
    }
}