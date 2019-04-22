using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Common functionality for MSB part editors. To be subclassed by game specific editors
/// because Unity doesn't like 1 editor to rule them all perse. Most of the actual logic
/// for all the games is consolidated here to maximize code reuse at the expense of more
/// complex logic and monolithicity :fatcat:
/// </summary>
public abstract class MSBPartEditorBase : Editor
{
    static bool ShowDefaultInspector = true;
    static bool ShowTreasures = true;
    protected enum MSBType
    {
        MSB1,
        MSB3,
        MSBBB,
        MSBSekiro,
    }
    protected MSBType _MSBType = MSBType.MSB1;

    protected void DrawTreasureEditor()
    {
        ShowTreasures = EditorGUILayout.Foldout(ShowTreasures, "Attached Treasures");
        if (ShowTreasures)
        {
            bool matched = false;
            var treasure = GameObject.Find("/MSBEvents/Treasures");
            var treasures = DarkSoulsTools.GetChildren(treasure);
            foreach (var t in treasures)
            {
                if (_MSBType == MSBType.MSB1)
                {

                }
                else if (_MSBType == MSBType.MSB3)
                {
                    var comp = t.GetComponent<MSB3TreasureEvent>();
                    if (comp.PartName2 == serializedObject.targetObject.name)
                    {
                        matched = true;
                        EditorGUILayout.LabelField($@"Treasure: {t.name}");
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField($@"Lot Param 1: {comp.ItemLot1}");
                        EditorGUILayout.LabelField($@"Lot Param 2: {comp.ItemLot2}");
                        if (GUILayout.Button("Select Treasure Event"))
                        {
                            Selection.activeGameObject = t;
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                else if (_MSBType == MSBType.MSBBB)
                {
                    var comp = t.GetComponent<MSBBBTreasureEvent>();
                    if (comp != null && comp.PartName2 == serializedObject.targetObject.name)
                    {
                        matched = true;
                        EditorGUILayout.LabelField($@"Treasure: {t.name}");
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField($@"Lot Param 1: {comp.ItemLot1}");
                        EditorGUILayout.LabelField($@"Lot Param 2: {comp.ItemLot2}");
                        if (GUILayout.Button("Select Treasure Event"))
                        {
                            Selection.activeGameObject = t;
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                else if (_MSBType == MSBType.MSBSekiro)
                {
                    var comp = t.GetComponent<MSBSTreasureEvent>();
                    if (comp != null && comp.TreasurePartName == serializedObject.targetObject.name)
                    {
                        matched = true;
                        EditorGUILayout.LabelField($@"Treasure: {t.name}");
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField($@"Lot Param ID: {comp.ItemLotID}");
                        if (GUILayout.Button("Select Treasure Event"))
                        {
                            Selection.activeGameObject = t;
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }
            if (!matched)
            {
                EditorGUILayout.LabelField($@"No attached treasures");
            }
        }
    }

    protected void DrawRawProperties()
    {
        ShowDefaultInspector = EditorGUILayout.Foldout(ShowDefaultInspector, "Raw MSB Properties");
        if (ShowDefaultInspector)
        {
            DrawDefaultInspector();
        }
    }
}
