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
    static bool ShowRenderGroups = true;
    protected enum MSBType
    {
        MSB1,
        MSB2SOTFS,
        MSB3,
        MSBBB,
        MSBSekiro,
    }
    protected MSBType _MSBType = MSBType.MSB1;

    static string GetModelAssetPath(MSBType type, string name)
    {
        string assetbase = "Assets";
        switch (type)
        {
            case MSBType.MSB1:
                assetbase += "/DS1";
                break;
            case MSBType.MSB2SOTFS:
                assetbase += "/DS2SOTFS";
                break;
            case MSBType.MSB3:
                assetbase += "/DS3";
                break;
            case MSBType.MSBBB:
                assetbase += "/Bloodborne";
                break;
            case MSBType.MSBSekiro:
                assetbase += "/Sekiro";
                break;
        }

        if (name.StartsWith("o"))
        {
            return $@"{assetbase}/Obj/{name}.prefab";
        }
        else  if (name.StartsWith("c"))
        {
            return $@"{assetbase}/Chr/{name}.prefab";
        }
        return "";
    }

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

    internal bool[] UnpackBitfield(uint field)
    {
        bool[] ret = new bool[32];
        for (int i = 0; i < 32; i++)
        {
            ret[i] = ((field >> i) & 0x1) == 1;
        }
        return ret;
    }

    internal uint PackBitfield(bool[] field)
    {
        uint ret = 0;
        for (int i = 0; i < 32; i++)
        {
            if (field[i])
            {
                ret |= ((uint)1 << i);
            }
        }
        return ret;
    }

    static void ApplyDrawgroups(uint[] drawgroups, MSBType type)
    {
        var mapPieces = GameObject.Find("MSBParts/MapPieces");
        for (int i = 0; i < mapPieces.transform.childCount; i++)
        {
            var mapPiece = mapPieces.transform.GetChild(i).gameObject;

            if (type == MSBType.MSB3)
            {
                var comp = mapPiece.GetComponent<MSB3MapPiecePart>();
                bool visible = ((drawgroups[0] & comp.DrawGroup1) != 0) ||
                               ((drawgroups[1] & comp.DrawGroup2) != 0) ||
                               ((drawgroups[2] & comp.DrawGroup3) != 0) ||
                               ((drawgroups[3] & comp.DrawGroup4) != 0) ||
                               ((drawgroups[4] & comp.DrawGroup5) != 0) ||
                               ((drawgroups[5] & comp.DrawGroup6) != 0) ||
                               ((drawgroups[6] & comp.DrawGroup7) != 0) ||
                               ((drawgroups[7] & comp.DrawGroup8) != 0);
                mapPiece.SetActive(visible);
            }
            else if (type == MSBType.MSBBB)
            {
                var comp = mapPiece.GetComponent<MSBBBMapPiecePart>();
                bool visible = ((drawgroups[0] & comp.DrawGroup1) != 0) ||
                               ((drawgroups[1] & comp.DrawGroup2) != 0) ||
                               ((drawgroups[2] & comp.DrawGroup3) != 0) ||
                               ((drawgroups[3] & comp.DrawGroup4) != 0) ||
                               ((drawgroups[4] & comp.DrawGroup5) != 0) ||
                               ((drawgroups[5] & comp.DrawGroup6) != 0) ||
                               ((drawgroups[6] & comp.DrawGroup7) != 0) ||
                               ((drawgroups[7] & comp.DrawGroup8) != 0);
                mapPiece.SetActive(visible);
            }
            else if (type == MSBType.MSB1)
            {
                var comp = mapPiece.GetComponent<MSB1MapPiecePart>();
                bool visible = ((drawgroups[0] & comp.DrawGroup1) != 0) ||
                               ((drawgroups[1] & comp.DrawGroup2) != 0) ||
                               ((drawgroups[2] & comp.DrawGroup3) != 0) ||
                               ((drawgroups[3] & comp.DrawGroup4) != 0);
                mapPiece.SetActive(visible);
            }
        }
    }

    static void ApplyDispgroups(uint[] dispgroups, MSBType type)
    {
        var mapPieces = GameObject.Find("MSBParts/MapPieces");
        for (int i = 0; i < mapPieces.transform.childCount; i++)
        {
            var mapPiece = mapPieces.transform.GetChild(i).gameObject;

            if (type == MSBType.MSB3)
            {
                var comp = mapPiece.GetComponent<MSB3MapPiecePart>();
                bool visible = ((dispgroups[0] & comp.DispGroup1) != 0) ||
                               ((dispgroups[1] & comp.DispGroup2) != 0) ||
                               ((dispgroups[2] & comp.DispGroup3) != 0) ||
                               ((dispgroups[3] & comp.DispGroup4) != 0) ||
                               ((dispgroups[4] & comp.DispGroup5) != 0) ||
                               ((dispgroups[5] & comp.DispGroup6) != 0) ||
                               ((dispgroups[6] & comp.DispGroup7) != 0) ||
                               ((dispgroups[7] & comp.DispGroup8) != 0);
                mapPiece.SetActive(visible);
            }
            else if (type == MSBType.MSBBB)
            {
                var comp = mapPiece.GetComponent<MSBBBMapPiecePart>();
                bool visible = ((dispgroups[0] & comp.DrawGroup1) != 0) ||
                               ((dispgroups[1] & comp.DrawGroup2) != 0) ||
                               ((dispgroups[2] & comp.DrawGroup3) != 0) ||
                               ((dispgroups[3] & comp.DrawGroup4) != 0) ||
                               ((dispgroups[4] & comp.DrawGroup5) != 0) ||
                               ((dispgroups[5] & comp.DrawGroup6) != 0) ||
                               ((dispgroups[6] & comp.DrawGroup7) != 0) ||
                               ((dispgroups[7] & comp.DrawGroup8) != 0);
                mapPiece.SetActive(visible);
            }
            else if (type == MSBType.MSB1)
            {
                var comp = mapPiece.GetComponent<MSB1MapPiecePart>();
                bool visible = ((dispgroups[0] & comp.DispGroup1) != 0) ||
                               ((dispgroups[1] & comp.DispGroup2) != 0) ||
                               ((dispgroups[2] & comp.DispGroup3) != 0) ||
                               ((dispgroups[3] & comp.DispGroup4) != 0);
                mapPiece.SetActive(visible);
            }
        }
    }

    protected void DrawRenderGroups()
    {
        ShowRenderGroups = EditorGUILayout.Foldout(ShowRenderGroups, "Rendering Groups");
        if (ShowRenderGroups)
        {
            int groupCount = (_MSBType == MSBType.MSB1 ? 4 : 8);
            bool[] mapstudiolayer;
            var drawgroups = new bool[groupCount][];
            var dispgroups = new bool[groupCount][];
            var backreadgroups = new bool[groupCount][];

            if (_MSBType == MSBType.MSB3 || _MSBType == MSBType.MSBSekiro)
            {
                var msl = (uint)serializedObject.FindProperty($@"MapStudioLayer").longValue;
                mapstudiolayer = UnpackBitfield(msl);
                EditorGUILayout.LabelField("Ceremony Layers:");
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 32; i++)
                {
                    if (i == 16)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    mapstudiolayer[i] = GUILayout.Toggle(mapstudiolayer[i], "");
                }
                EditorGUILayout.EndHorizontal();

                for (int g = 0; g < groupCount; g++)
                {
                    uint pack = PackBitfield(mapstudiolayer);
                    serializedObject.FindProperty($@"MapStudioLayer").longValue = pack;
                }
            }

            for (int g = 0; g < groupCount; g++)
            {
                var dg = (uint)serializedObject.FindProperty($@"DrawGroup{g+1}").longValue;
                drawgroups[g] = UnpackBitfield(dg);
            }
            EditorGUILayout.LabelField("Draw Groups:");
            for (int j = 0; j < groupCount; j++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 32; i++)
                {
                    if (i == 16)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    drawgroups[j][i] = GUILayout.Toggle(drawgroups[j][i], "");
                }
                EditorGUILayout.EndHorizontal();
            }
            for (int g = 0; g < groupCount; g++)
            {
                uint pack = PackBitfield(drawgroups[g]);
                serializedObject.FindProperty($@"DrawGroup{g+1}").longValue = pack;
            }
            if (GUILayout.Button("Show Visible objects"))
            {
                var dgs = new uint[groupCount];
                for (int g = 0; g < groupCount; g++)
                {
                    dgs[g] = (uint)serializedObject.FindProperty($@"DrawGroup{g + 1}").longValue;
                }
                ApplyDrawgroups(dgs, _MSBType);
            }

            for (int g = 0; g < groupCount; g++)
            {
                var dg = (uint)serializedObject.FindProperty($@"DispGroup{g + 1}").longValue;
                dispgroups[g] = UnpackBitfield(dg);
            }
            EditorGUILayout.LabelField("Display Groups:");
            for (int j = 0; j < groupCount; j++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 32; i++)
                {
                    if (i == 16)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    dispgroups[j][i] = GUILayout.Toggle(dispgroups[j][i], "");
                }
                EditorGUILayout.EndHorizontal();
            }
            for (int g = 0; g < groupCount; g++)
            {
                uint pack = PackBitfield(dispgroups[g]);
                serializedObject.FindProperty($@"DispGroup{g + 1}").longValue = pack;
            }
            if (GUILayout.Button("Show Visible objects"))
            {
                var dgs = new uint[groupCount];
                for (int g = 0; g < groupCount; g++)
                {
                    dgs[g] = (uint)serializedObject.FindProperty($@"DispGroup{g + 1}").longValue;
                }
                ApplyDispgroups(dgs, _MSBType);
            }

            if (_MSBType != MSBType.MSB1)
            {
                for (int g = 0; g < groupCount; g++)
                {
                    var dg = (uint)serializedObject.FindProperty($@"BackreadGroup{g + 1}").longValue;
                    backreadgroups[g] = UnpackBitfield(dg);
                }
                EditorGUILayout.LabelField("Backread Groups:");
                for (int j = 0; j < groupCount; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int i = 0; i < 32; i++)
                    {
                        if (i == 16)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        backreadgroups[j][i] = GUILayout.Toggle(backreadgroups[j][i], "");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                for (int g = 0; g < groupCount; g++)
                {
                    uint pack = PackBitfield(backreadgroups[g]);
                    serializedObject.FindProperty($@"BackreadGroup{g + 1}").longValue = pack;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

    protected void DrawRefreshModel()
    {
        if (targets.Count() == 1 && GUILayout.Button("Refresh Visible Model"))
        {
            var gtarg = ((MonoBehaviour)target).gameObject;
            var prefabPath = GetModelAssetPath(_MSBType, serializedObject.FindProperty("ModelName").stringValue);
            GameObject prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefabObj != null)
            {
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabObj);
                Undo.RegisterCreatedObjectUndo(newObj, "created prefab");
                PrefabUtility.SetPropertyModifications(newObj, PrefabUtility.GetPropertyModifications(gtarg));
                var addedcomps = PrefabUtility.GetAddedComponents(gtarg);
                foreach (var added in addedcomps)
                {
                    if (UnityEditorInternal.ComponentUtility.CopyComponent(added.instanceComponent))
                    {
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newObj);
                    }
                }
                newObj.name = gtarg.name;
                newObj.transform.position = gtarg.transform.position;
                newObj.transform.rotation = gtarg.transform.rotation;
                newObj.transform.localScale = gtarg.transform.localScale;
                newObj.transform.parent = gtarg.transform.parent;
                newObj.transform.SetSiblingIndex(gtarg.transform.GetSiblingIndex());
                newObj.layer = gtarg.layer;
                Undo.DestroyObjectImmediate(gtarg);
                Selection.activeObject = newObj;
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
