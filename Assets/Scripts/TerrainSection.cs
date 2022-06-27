using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class TerrainSection : ScriptableObject, ISerializationCallbackReceiver
{
    [Serializable]
    public class MaterialData
    {
        [SerializeField]
        public Material material;

        [HideInInspector]
        [SerializeField]
        public Color32[] tileData;

        // non serialized runtime state
        internal bool weightCacheDirty = true;
        Texture2D m_weightCache;

        public Texture2D weightCache
        {
            get
            {
                if (weightCacheDirty || m_weightCache == null)
                    UpdateCache();
                return m_weightCache;
            }
        }

        public MaterialData()
        {
            int count = LevelSection.tilesX * LevelSection.tilesZ;
            tileData = new Color32[count];
        }

        internal void UpdateCache()
        {
            if (m_weightCache == null)
            {
                m_weightCache = new Texture2D(LevelSection.tilesX, LevelSection.tilesZ, GraphicsFormat.R8G8B8A8_UNorm, 1, TextureCreationFlags.None)
                {
                    name = "WeightCache_" + material.name,
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.DontSave,
                };
            }
            m_weightCache.SetPixels32(tileData);
            m_weightCache.Apply();
            weightCacheDirty = false;
        }
    }

    public struct Tile
    {
        public byte weight;
    }

    [SerializeField]
    public List<MaterialData> materialData = new List<MaterialData>();

    [SerializeField]
    public UInt16[] heightData;

    // non serialized runtime state
    internal bool heightCacheDirty = true;
    Texture2D m_heightCache;
    Color[] m_heightColors;

    public Texture2D heightCache
    {
        get
        {
            if (heightCacheDirty || m_heightCache == null)
                UpdateHeightCache();
            return m_heightCache;
        }
    }

    internal void UpdateHeightCache()
    {
        if (m_heightCache == null)
        {
            m_heightCache = new Texture2D(LevelSection.tilesX, LevelSection.tilesZ, GraphicsFormat.R16_UNorm, 1, TextureCreationFlags.None)
            {
                name = "HeightCache_" + name,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };
        }

        int count = LevelSection.tilesX * LevelSection.tilesZ;
        if ((m_heightColors == null) || (m_heightColors.Length != count))
        {
            m_heightColors = new Color[count];
        }

        for (int x = 0; x < count; x++)
            m_heightColors[x] = new Color(heightData[x] / 65535.0f, 0.0f, 0.0f, 1.0f);

        m_heightCache.SetPixels(m_heightColors);
        m_heightCache.Apply();
        heightCacheDirty = false;
    }

    private void OnEnable()
    {
        int count = LevelSection.tilesX * LevelSection.tilesZ;
        if ((heightData == null) || (heightData.Length < count))
        {
            heightData = new UInt16[count];
            heightCacheDirty = true;
        }

        if (materialData == null)
            materialData = new List<MaterialData>();
    }

    public void UpdateAllCaches()
    {
        for (int m = 0; m < materialData.Count; m++)
            materialData[m].UpdateCache();
        UpdateHeightCache();
    }

    public void BeginEdit(string undoName)
    {
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, undoName);
#endif
    }

    public void EndEdit(int materialIndex)
    {
        materialData[materialIndex].weightCacheDirty = true;
        // UpdateCache();
    }

    public void EndEditHeight()
    {
        heightCacheDirty = true;
        // UpdateHeightCache();
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // flag all caches dirty, we're not sure what changed
        for (int m = 0; m < materialData.Count; m++)
            materialData[m].weightCacheDirty = true;
        heightCacheDirty = true;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }
}
