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
public abstract class MSBEventEditorBase : Editor
{
    static bool ShowDefaultInspector = true;
    static bool ShowTreasureEditor = true;
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
        ShowTreasureEditor = EditorGUILayout.Foldout(ShowTreasureEditor, "Treasure Editor");
        if (ShowTreasureEditor)
        {
            if (serializedObject.targetObjects.Count() == 1)
            {
                if (GUILayout.Button("Go to Target Object"))
                {
                    string objectName;
                    if (_MSBType == MSBType.MSBSekiro)
                    {
                        objectName = serializedObject.FindProperty("TreasurePartName").stringValue;
                    }
                    else
                    {
                        objectName = serializedObject.FindProperty("PartName2").stringValue;
                    }
                    var obj = GameObject.Find($@"/MSBParts/Objects/{objectName}");
                    if (obj == null)
                    {
                        obj = GameObject.Find($@"/MSBParts/DummyObjects/{objectName}");
                    }
                    if (obj == null)
                    {
                        EditorUtility.DisplayDialog("Object not found", $@"Couldn't find object {obj}", "Ok");
                    }
                    else
                    {
                        Selection.activeGameObject = obj;
                        SceneView.FrameLastActiveSceneView();
                    }
                }

                var itemLots = new int[2];
                if (_MSBType == MSBType.MSB3 ||_MSBType == MSBType.MSBBB)
                {
                    itemLots[0] = serializedObject.FindProperty("ItemLot1").intValue;
                    itemLots[1] = serializedObject.FindProperty("ItemLot2").intValue;
                }
                else if (_MSBType == MSBType.MSBSekiro)
                {
                    itemLots[0] = serializedObject.FindProperty("ItemLotID").intValue;
                    itemLots[1] = -1;
                }

                foreach (var lot in itemLots)
                {
                    if (lot == -1)
                    {
                        EditorGUILayout.LabelField($@"Lot Param: No lot");
                    }
                    else
                    {
                        EditorGUILayout.LabelField($@"Lot Param: {lot}");
                        EditorGUI.indentLevel++;
                        var lotParam = ItemLotParamUtils.LookupItemLot(lot);
                        if (lotParam != null)
                        {
                            EditorGUILayout.BeginVertical();
                            for (int i = 0; i < 8; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField($@"Item ID {i + 1}:", GUILayout.MaxWidth(80.0f));
                                lotParam.ItemID[i] = EditorGUILayout.IntField(lotParam.ItemID[i]);
                                if (EditorGUILayout.DropdownButton(new GUIContent($@"{FMGUtils.LookupItemName(lotParam.ItemID[i])}"), FocusType.Keyboard))
                                {
                                    GenericMenu menu = new GenericMenu();
                                    foreach (var item in ItemLotParamUtils.ItemNameList)
                                    {
                                        if (item.Item2 == "?ItemName?")
                                            continue;
                                        menu.AddItem(new GUIContent($@"{item.Item2} ({item.Item1})"), item.Item1 == lotParam.ItemID[i], o => {
                                            Tuple<int, ItemLotParam, int, int> stuff = (Tuple<int, ItemLotParam, int, int>)o;
                                            stuff.Item2.ItemID[stuff.Item4] = stuff.Item3;
                                            ItemLotParamUtils.UpdateItemLot(stuff.Item1, stuff.Item2);
                                            this.Repaint();
                                        }, new Tuple<int, ItemLotParam, int, int>(lot, lotParam, item.Item1, i));
                                    }
                                    menu.ShowAsContext();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            ItemLotParamUtils.UpdateItemLot(lot, lotParam);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                if (GUILayout.Button("Save Params"))
                {
                    ItemLotParamUtils.SaveParams();
                }
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
