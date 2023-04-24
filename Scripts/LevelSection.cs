using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class LevelSection : MonoBehaviour
{
    public const int tilesX = 32;
    public const int tilesZ = 128;

//    [SerializeField]
//    public List<TerrainSection.MaterialData> materialData;

    internal Mesh mesh;
    public MeshCollider meshCollider;
    internal MeshRenderer meshRenderer;

    public TerrainSection terrainSection;

    private void OnEnable()
    {
/*
        // used to resurrect and version materialData, if need be
        if ((terrainSection == null) && (materialData != null))
        {
            // create the terrain section and copy material data to it
            terrainSection = ScriptableObject.CreateInstance<TerrainSection>();
            terrainSection.materialData = new List<TerrainSection.MaterialData>();
            for (int x = 0; x < materialData.Count; x++)
            {
                terrainSection.materialData.Add(materialData[x]);
            }
            materialData = null;        // nuke old data

            AssetDatabase.CreateAsset(terrainSection, "Assets/Levels/Level0/TerrainData_" + this.GetInstanceID() + ".asset");
            // AssetDatabase.SaveAssets();
        }
*/

        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = !Application.isPlaying;

        // setup mesh as level section XZ bounds
        {
            // delete old one
            if (meshCollider.sharedMesh != null)
            {
                if (Application.isPlaying)
                    Destroy(meshCollider.sharedMesh);
                else
                    DestroyImmediate(meshCollider.sharedMesh);
            }

            mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            float minZ = LevelScroller.minX;
            float maxZ = minZ + tilesZ * LevelScroller.meshScale;
            mesh.vertices = new Vector3[4]
            {
                new Vector3(LevelScroller.minX, 0.0f, minZ),
                new Vector3(LevelScroller.minX, 0.0f, maxZ),
                new Vector3(LevelScroller.maxX, 0.0f, maxZ),
                new Vector3(LevelScroller.maxX, 0.0f, minZ)
            };
            mesh.uv = new Vector2[4]
            {
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
            };
            mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
            mesh.name = "SectionMesh";
            mesh.RecalculateBounds();
        }
        meshCollider.sharedMesh = mesh;
        meshCollider.enabled = true;    // Application.isPlaying;

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(meshFilter.sharedMesh);
            else
                DestroyImmediate(meshFilter.sharedMesh);
        }

        meshFilter.sharedMesh = mesh;

        terrainSection?.UpdateAllCaches();
    }

    private void OnDisable()
    {
    }

    void Update()
    {
    }
}
