using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class LevelScroller : MonoBehaviour
{
    // renderer tiles
    public const int tilesX = 32;
    public const int tilesZ = 32;

    // renderer cache rez (per tile)
    public const int meshRez = 32;          // mesh vertices per tile
    public const int cacheRez = 64;         // cache color samples per tile
    public const float meshScale = 2.0f;

    public const float minX = tilesX * -0.5f * meshScale;
    public const float maxX = tilesX * 0.5f * meshScale;

    Mesh mesh;
    MaterialPropertyBlock propertyBlock;        // store the dynamic settings for the material
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    public RenderTexture colorCache;
    public RenderTexture normalCache;
    public RenderTexture miscCache;
    public RenderTexture heightCache;
    public RenderTexture baseHeightCache;

    public float startScroll = 0.0f;
    public float scrollSpeed = 3.0f;
    public GameObject terrainRenderer;
    public List<LevelSection> sections;

    public Material heightMaterial;

    public bool followRenderingCamera;
    public float followDistance = 16.0f;

    float scrollAmount = 0.0f;                  // how much we are scrolled in the Z direction
    float startTime;

    private void OnDestroy()
    {
        if (mesh != null)
            DestroyImmediate(mesh);
    }

    private void Start()
    {
        startTime = Time.time;
    }

    void BuildMesh()
    {
        if (mesh != null)
        {
            if (Application.isPlaying)
                Destroy(mesh);
            else
                DestroyImmediate(mesh);
        }
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int width = tilesX * meshRez;
        int height = tilesZ * meshRez;
        Vector3[] pos = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];
        int[] i = new int[(width-1) * (height-1) * 6];

        float scaleX = (1.0f / meshRez);
        float scaleZ = (1.0f / meshRez);
        float offsetX = (tilesX * -0.5f);
        float offsetZ = (tilesZ * -0.5f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                int cur = x * height + z;

                float px = x * scaleX + offsetX;
                float pz = z * scaleZ + offsetZ;
                pos[cur] = new Vector3(px * meshScale, 0.0f, pz * meshScale);
                uvs[cur] = new Vector2(x / (width-1.0f), z / (height - 1.0f));

                if ((x < width-1) && (z < height-1))
                {
                    int curI = (x * (height-1) + z) * 6;
                    i[curI + 0] = cur;
                    i[curI + 1] = cur + 1;
                    i[curI + 2] = cur + height;
                    i[curI + 3] = cur + height;
                    i[curI + 4] = cur + 1;
                    i[curI + 5] = cur + height + 1;
                }
            }
        }

        mesh.vertices = pos;
        mesh.uv = uvs;
        mesh.SetIndices(i, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();

        // make sure we don't save it, or the scene file will get huge
        mesh.hideFlags = HideFlags.DontSave;
    }

    void SetupCaches()
    {
        if (colorCache != null)
            colorCache.Release();
        if (normalCache != null)
            normalCache.Release();
        if (miscCache != null)
            miscCache.Release();
        if (heightCache != null)
            heightCache.Release();
        if (baseHeightCache != null)
            baseHeightCache.Release();

        int cacheWidth = tilesX * cacheRez;
        int cacheHeight = tilesZ * cacheRez;
        int heightWidth = tilesX * cacheRez;
        int heightHeight = tilesZ * cacheRez;

        colorCache = new RenderTexture(cacheWidth, cacheHeight, 1, GraphicsFormat.R8G8B8A8_SRGB, 1) { name = "ColorCache", hideFlags = HideFlags.DontSave };
        normalCache = new RenderTexture(cacheWidth, cacheHeight, 1, GraphicsFormat.R8G8B8A8_UNorm, 1) { name = "NormalCache", hideFlags = HideFlags.DontSave, depth = 0 };
        miscCache = new RenderTexture(cacheWidth, cacheHeight, 1, GraphicsFormat.R8G8B8A8_UNorm, 1) { name = "MiscCache", hideFlags = HideFlags.DontSave, depth = 0 };
        heightCache = new RenderTexture(cacheWidth, cacheHeight, 1, GraphicsFormat.R16_UNorm, 1) { name = "HeightCache", hideFlags = HideFlags.DontSave, depth = 0 };
        baseHeightCache = new RenderTexture(heightWidth, heightHeight, 1, GraphicsFormat.R16_UNorm, 1) { name = "BaseHeight", hideFlags = HideFlags.DontSave, depth = 0 };

        colorCache.Create();
        normalCache.Create();
        miscCache.Create();
        heightCache.Create();
        baseHeightCache.Create();
    }

    public void RenderCache(float scrollAmount = 0.0f)
    {
        RenderTexture old = RenderTexture.active;

        RenderBuffer[] rbs = new RenderBuffer[4];
        rbs[0] = colorCache.colorBuffer;
        rbs[1] = normalCache.colorBuffer;
        rbs[2] = miscCache.colorBuffer;
        rbs[3] = heightCache.colorBuffer;

        RenderTargetSetup rts = new RenderTargetSetup(rbs, colorCache.depthBuffer);
        Graphics.SetRenderTarget(rts);
        GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        float firstSection = (scrollAmount / meshScale) / LevelSection.tilesZ;
        float lastSection = ((scrollAmount / meshScale) + LevelScroller.tilesZ) / LevelSection.tilesZ;

        for (int sectionIndex = Mathf.FloorToInt(firstSection); sectionIndex <= Mathf.FloorToInt(lastSection); sectionIndex++)
        {
            if ((sectionIndex >= 0) && (sectionIndex < sections.Count))
            {
                RenderCacheSection(sectionIndex, scrollAmount);
            }
        }

        RenderTexture.active = baseHeightCache;
        GL.Clear(true, true, Color.clear);

        for (int sectionIndex = Mathf.FloorToInt(firstSection); sectionIndex <= Mathf.FloorToInt(lastSection); sectionIndex++)
        {
            if ((sectionIndex >= 0) && (sectionIndex < sections.Count))
            {
                RenderHeightSection(sectionIndex, scrollAmount);
            }
        }

        GL.PopMatrix();
        RenderTexture.active = old;
    }

    void RenderCacheSection(int sectionIndex, float scrollAmount)
    {
        var section = sections[sectionIndex];
        float sectionScrollStart = sectionIndex * LevelSection.tilesZ * meshScale;
        float sectionScrollTiles = (scrollAmount - sectionScrollStart) / meshScale;

        // calculate where the section overlaps the current render target
        float startPct = Mathf.Clamp01(-sectionScrollTiles / tilesZ);
        float endPct = Mathf.Clamp01((LevelSection.tilesZ - sectionScrollTiles) / tilesZ);

        var materialData = section?.terrainSection?.materialData;
        if (materialData != null)
        {
            // loop all materials in the section
            for (int matIndex = 0; matIndex < materialData.Count; matIndex++)
            {
                var matData = materialData[matIndex];
                var mat = matData?.material;
                if (mat != null)
                {
                    mat.SetVector("_LevelOffset", new Vector4(0.0f, sectionScrollTiles / tilesZ, 0.0f, 0.0f));
                    mat.SetTexture("_WeightCache", matData.weightCache);
                    mat.SetTextureScale("_WeightCache", new Vector2(1.0f, LevelSection.tilesX / (float)LevelSection.tilesZ));
                    mat.SetTextureOffset("_WeightCache", new Vector2(0.0f, 0.0f));

                    mat.SetPass(0);

                    float minx = 0.0f;
                    float maxx = 1.0f;
                    float minz = startPct;
                    float maxz = endPct;

                    GL.Begin(GL.QUADS);
                    GL.TexCoord2(minx, minz); GL.Vertex(new Vector3(minx, minz, 0.0f));
                    GL.TexCoord2(minx, maxz); GL.Vertex(new Vector3(minx, maxz, 0.0f));
                    GL.TexCoord2(maxx, maxz); GL.Vertex(new Vector3(maxx, maxz, 0.0f));
                    GL.TexCoord2(maxx, minz); GL.Vertex(new Vector3(maxx, minz, 0.0f));
                    GL.End();
                }
            }
        }
    }

    void RenderHeightSection(int sectionIndex, float scrollAmount)
    {
        // render height
        if (heightMaterial != null)
        {
            var section = sections[sectionIndex];
            var terrain = section?.terrainSection;
            if (terrain == null)
                return;

            float sectionScrollStart = sectionIndex * LevelSection.tilesZ * meshScale;
            float sectionScrollTiles = (scrollAmount - sectionScrollStart) / meshScale;

            // calculate where the section overlaps the current render target
            float startPct = Mathf.Clamp01(-sectionScrollTiles / tilesZ);
            float endPct = Mathf.Clamp01((LevelSection.tilesZ - sectionScrollTiles) / tilesZ);

            var mat = heightMaterial;
            mat.SetVector("_LevelOffset", new Vector4(0.0f, sectionScrollTiles / tilesZ, 0.0f, 0.0f));
            mat.SetTexture("_HeightCache", terrain.heightCache);
            mat.SetTextureScale("_HeightCache", new Vector2(1.0f, LevelSection.tilesX / (float)LevelSection.tilesZ));
            mat.SetTextureOffset("_HeightCache", new Vector2(0.0f, 0.0f));

            mat.SetPass(0);

            float minx = 0.0f;
            float maxx = 1.0f;
            float minz = startPct;
            float maxz = endPct;

            GL.Begin(GL.QUADS);
            GL.TexCoord2(minx, minz); GL.Vertex(new Vector3(minx, minz, 0.0f));
            GL.TexCoord2(minx, maxz); GL.Vertex(new Vector3(minx, maxz, 0.0f));
            GL.TexCoord2(maxx, maxz); GL.Vertex(new Vector3(maxx, maxz, 0.0f));
            GL.TexCoord2(maxx, minz); GL.Vertex(new Vector3(maxx, minz, 0.0f));
            GL.End();
        }
    }

    void OnCameraBeginRendering(ScriptableRenderContext context, Camera camera)
    {
        if (followRenderingCamera && (camera != null))
        {
            SetupToFollowCamera(camera.transform);
        }
        else
        {
            SetupTargetPosition(transform.position);
        }
    }

    void SetupToFollowCamera(Transform cameraTransform)
    {
        Vector3 targetPos;
        {
            // make terrain follow the camera in x and z dimensions
            float distance = followDistance;     // TODO: world size of smallest mip
            Vector3 cameraPos = cameraTransform.position;
            Vector3 cameraDir = cameraTransform.forward;
            targetPos.x = cameraPos.x + cameraDir.x * distance;
            targetPos.y = 0.0f;
            targetPos.z = cameraPos.z + cameraDir.z * distance;
        }
        SetupTargetPosition(targetPos);
    }

    void SetupTargetPosition(Vector3 targetPos)
    {
        if (!Application.isPlaying)
        {
            SetScroll(targetPos.z - transform.position.z, false);
        }
    }

    private void OnEnable()
    {
        meshFilter = terrainRenderer.GetComponent<MeshFilter>();
        meshRenderer = terrainRenderer.GetComponent<MeshRenderer>();

        BuildMesh();
        if (meshFilter.sharedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(meshFilter.sharedMesh);
            else
                DestroyImmediate(meshFilter.sharedMesh);
        }
        meshFilter.sharedMesh = mesh;

        SetupCaches();

        // inject camera callback to calculate mip transform for each camera
        // Camera.onPreCull += OnCameraPreCull;     // built-in
        RenderPipelineManager.beginCameraRendering += OnCameraBeginRendering;
    }

    private void OnDisable()
    {
        // remove camera callback
        // Camera.onPreCull -= OnCameraPreCull;     // built-in
        RenderPipelineManager.beginCameraRendering -= OnCameraBeginRendering;

        if (colorCache != null)
        {
            colorCache.Release();
            colorCache = null;
        }
        if (normalCache != null)
        {
            normalCache.Release();
            normalCache = null;
        }
        if (miscCache != null)
        {
            miscCache.Release();
            miscCache = null;
        }
        if (heightCache != null)
        {
            heightCache.Release();
            heightCache = null;
        }
        if (baseHeightCache != null)
        {
            baseHeightCache.Release();
            baseHeightCache = null;
        }
    }

    void SetScroll(float newScrollAmount, bool moveToOrigin)
    {
        float scrollDelta = newScrollAmount - scrollAmount;

        scrollAmount = newScrollAmount;

        float terrainScroll = Mathf.Floor(scrollAmount / meshScale) * meshScale;
        RenderCache(terrainScroll);

        if (moveToOrigin)
        {
            foreach (var s in sections)
                s.transform.Translate(Vector3.forward * -scrollDelta);

            Vector3 terrainPos = terrainRenderer.transform.position;
            terrainPos.z = transform.position.z - (scrollAmount - terrainScroll);
            terrainRenderer.transform.position = terrainPos;
        }
        else
        {
            Vector3 terrainPos = terrainRenderer.transform.position;
            terrainPos.z = terrainScroll + transform.position.z;
            terrainRenderer.transform.position = terrainPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            float newScrollAmount = startScroll + (Time.time - startTime) * scrollSpeed;
            SetScroll(newScrollAmount, true);
        }

        {
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();

            propertyBlock.SetColor("TintColor", new Color(1.0f, 1.0f, 1.0f));

            propertyBlock.SetVector("UVScale", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            propertyBlock.SetTexture("ColorTex", colorCache);
            propertyBlock.SetTexture("NormalTex", normalCache);
            propertyBlock.SetTexture("MiscTex", miscCache);
            propertyBlock.SetTexture("HeightTex", heightCache);
            propertyBlock.SetTexture("BaseHeightTex", baseHeightCache);

            propertyBlock.SetVector("UVOffset", new Vector4(0.0f, scrollAmount / (tilesZ * meshScale), 0.0f, 0.0f));

            meshRenderer.SetPropertyBlock(propertyBlock, 0);
        }
    }
}
