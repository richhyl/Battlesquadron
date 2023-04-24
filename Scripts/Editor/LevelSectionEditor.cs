using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSection))]
public class LevelSectionEditor : Editor
{
    int editMaterial = 0;
    int editMode = 0;
    float strength = 1.0f;

    string[] editModes = new string[] { "None", "Add Remove", "Clear", "Add Height", "Set Height"};

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        LevelSection section = (LevelSection)target;
        TerrainSection terrain = section?.terrainSection;

        SerializedProperty terrainProp = serializedObject.FindProperty("terrainSection");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(terrainProp, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        if (terrain == null)
            return;

        CreateEditor(terrain).OnInspectorGUI();

        editMaterial = EditorGUILayout.Popup("Edit Material", editMaterial, terrain.materialData.Select(m => m.material?.name ?? "Mat").ToArray());
        editMode = EditorGUILayout.Popup("Edit Mode", editMode, editModes);

        // show edit options
        strength = EditorGUILayout.Slider("Strength", strength, 0.0f, 1.0f);

        if (GUILayout.Button("Clear"))
        {
            var arr = terrain.materialData[editMaterial].tileData;
            terrain.BeginEdit("ClearMaterial");
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                c.r = 0;
                arr[i] = c;
            }
            terrain.EndEdit(editMaterial);
        }

        if (GUILayout.Button("Fill"))
        {
            var arr = terrain.materialData[editMaterial].tileData;
            terrain.BeginEdit("FillMaterial");
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                c.r = 255;
                arr[i] = c;
            }
            terrain.EndEdit(editMaterial);
        }
    }

    public void OnSceneGUICallback(SceneView sceneView)
    {
        if (editMode == 0)
            return;

        LevelSection section = (LevelSection) target;
        TerrainSection terrain = section?.terrainSection;

        var parentObj = section?.transform?.parent?.gameObject;
        if (parentObj == null)
            return;

        var scroller = parentObj.GetComponent<LevelScroller>();
        if (scroller == null)
            return;

        Event e = Event.current;

        RaycastHit rayhit;
        bool hit = false;
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            // Debug.Log("Mouse: " + e.mousePosition + " Ray: " + mouseRay);
            hit = section.meshCollider.Raycast(mouseRay, out rayhit, Mathf.Infinity);
        }
        Vector2 hitUV = rayhit.textureCoord;

        int myControlId = GUIUtility.GetControlID(983588, FocusType.Passive);

        var eventType = e.GetTypeForControl(myControlId);
        switch (eventType)
        {
            case EventType.Layout:
                HandleUtility.AddDefaultControl(myControlId);
                break;
            case EventType.MouseMove:
                break;
            case EventType.MouseDown:
            case EventType.MouseDrag:
                if (EditorGUIUtility.hotControl != 0 && EditorGUIUtility.hotControl != myControlId)
                    return;
                if (eventType == EventType.MouseDrag && EditorGUIUtility.hotControl != myControlId)
                    return;
                if (e.alt || e.button != 0)
                    return;
                HandleUtility.AddDefaultControl(myControlId);
                if (e.type == EventType.MouseDown)
                    EditorGUIUtility.hotControl = myControlId;

                if (hit)
                {
                    // invoke tool at hitUV
                    int tileX = Mathf.RoundToInt(hitUV.x * LevelSection.tilesX);
                    int tileZ = Mathf.RoundToInt(hitUV.y * LevelSection.tilesZ);
                    // Debug.Log("Hit " + tileX + ", " + tileZ);

                    int amount = (int) (strength * 255.5f);

                    int minX = tileX;
                    int maxX = tileX + 1;
                    if (e.control)
                    {
                        // apply to whole line
                        minX = 0;
                        maxX = LevelSection.tilesX;
                    }

                    if (editMode == 1)
                    {
                        // Add/Remove
                        terrain.BeginEdit("AddRemove");
                        for (int x = minX; x < maxX; x++)
                        {
                            var c = terrain.materialData[editMaterial].tileData[tileZ * LevelSection.tilesX + x];
                            if (!e.shift)
                                c.r = (byte)Math.Min(c.r + amount, 255);
                            else
                                c.r = (byte)Math.Max(c.r - amount, 0);
                            terrain.materialData[editMaterial].tileData[tileZ * LevelSection.tilesX + x] = c;
                        }
                        terrain.EndEdit(editMaterial);
                    }
                    else if (editMode == 2)
                    {
                        // Clear
                        for (int m = 0; m < terrain.materialData.Count; m++)
                        {
                            terrain.BeginEdit("Clear");
                            for (int x = minX; x < maxX; x++)
                            {
                                var c = terrain.materialData[m].tileData[tileZ * LevelSection.tilesX + x];
                                c.r = 0;
                                terrain.materialData[m].tileData[tileZ * LevelSection.tilesX + x] = c;
                            }
                            terrain.EndEdit(m);
                        }
                    }
                    else if (editMode == 3)
                    {
                        int hamount = (int) (strength * 65535.5f);
                        terrain.BeginEdit("Add Height");
                        for (int x = minX; x < maxX; x++)
                        {
                            var h = terrain.heightData[tileZ * LevelSection.tilesX + x];
                            if (!e.shift)
                                h = (UInt16) Math.Min(h + hamount, 65535);
                            else
                                h = (UInt16) Math.Max(h - hamount, 0);
                            terrain.heightData[tileZ * LevelSection.tilesX + x] = h;
                        }
                        terrain.EndEditHeight();
                    }
                    else if (editMode == 4)
                    {
                        UInt16 hamount = (UInt16)(strength * 65535.5f);
                        terrain.BeginEdit("Set Height");
                        for (int x = minX; x < maxX; x++)
                        {
                            var h = terrain.heightData[tileZ * LevelSection.tilesX + x];
                            h = hamount;
                            terrain.heightData[tileZ * LevelSection.tilesX + x] = h;
                        }
                        terrain.EndEditHeight();
                    }

                    scroller.RenderCache();
                }

                Event.current.Use();
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl != myControlId)
                    return;
                GUIUtility.hotControl = 0;
                Event.current.Use();
                break;
        }
    }

    public void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUICallback;
    }

    public void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUICallback;
    }
}
