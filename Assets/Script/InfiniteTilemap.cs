using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteTilemap : MonoBehaviour
{
    [Header("Ground Settings")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase groundTile;

    [Header("Decoration Layer 1 (Kecil)")]
    [SerializeField] private Tilemap decoTilemap1; 
    [SerializeField] private TileBase[] smallDecorations; 
    [SerializeField, Range(0f, 1f)] private float deco1Density = 0.2f;

    [Header("Decoration Layer 2 (Besar)")]
    [SerializeField] private Tilemap decoTilemap2; 
    [SerializeField] private TileBase[] largeDecorations; 
    [SerializeField, Range(0f, 1f)] private float deco2Chance = 0.25f; // Peluang muncul per blok area
    [SerializeField] private int gridCellSize = 6; // Ukuran area kotak (dalam tile) agar objek besar tidak dempet/tumpuk

    [Header("Camera & Margin")]
    [SerializeField] private Camera cam;
    [SerializeField] private int margin = 2;

    private Vector3Int lastMin;
    private Vector3Int lastMax;
    private bool hasPainted;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void LateUpdate()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;

        Vector3Int min = tilemap.WorldToCell(new Vector3(c.x - halfW, c.y - halfH, 0))
                         - new Vector3Int(margin, margin, 0);
        Vector3Int max = tilemap.WorldToCell(new Vector3(c.x + halfW, c.y + halfH, 0))
                         + new Vector3Int(margin, margin, 0);

        if (hasPainted && min == lastMin && max == lastMax) return;

        // Bersihkan semua tilemap
        tilemap.ClearAllTiles();
        if (decoTilemap1 != null) decoTilemap1.ClearAllTiles();
        if (decoTilemap2 != null) decoTilemap2.ClearAllTiles();

        // 1. Generate Tanah (Ground) dan Layer 1 (Kecil)
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                tilemap.SetTile(cellPos, groundTile);

                // Layer 1 (Kecil)
                if (decoTilemap1 != null && smallDecorations != null && smallDecorations.Length > 0)
                {
                    float random1 = GetDeterministicRandom(x, y, 1337);
                    if (random1 < deco1Density)
                    {
                        int index = Mathf.FloorToInt(GetDeterministicRandom(x, y, 9999) * smallDecorations.Length);
                        index = Mathf.Clamp(index, 0, smallDecorations.Length - 1);
                        
                        if (smallDecorations[index] != null) 
                            decoTilemap1.SetTile(cellPos, smallDecorations[index]);
                    }
                }
            }
        }

        // 2. Generate Layer 2 (Besar) menggunakan Grid Partitioning (Dijamin tidak tumpuk & pasti muncul)
        if (decoTilemap2 != null && largeDecorations != null && largeDecorations.Length > 0)
        {
            // Hitung batasan indeks blok besar (gridCellSize) berdasarkan kamera
            int startCellX = Mathf.FloorToInt((float)min.x / gridCellSize);
            int endCellX = Mathf.CeilToInt((float)max.x / gridCellSize);
            int startCellY = Mathf.FloorToInt((float)min.y / gridCellSize);
            int endCellY = Mathf.CeilToInt((float)max.y / gridCellSize);

            for (int cx = startCellX; cx <= endCellX; cx++)
            {
                for (int cy = startCellY; cy <= endCellY; cy++)
                {
                    // Cek apakah blok area ini beruntung mendapat objek besar
                    float blockChance = GetDeterministicRandom(cx, cy, 7777);
                    if (blockChance < deco2Chance)
                    {
                        // Tentukan posisi pasti (x, y) di dalam blok area tersebut secara konsisten
                        int localX = Mathf.FloorToInt(GetDeterministicRandom(cx, cy, 1111) * gridCellSize);
                        int localY = Mathf.FloorToInt(GetDeterministicRandom(cx, cy, 2222) * gridCellSize);

                        int worldX = cx * gridCellSize + localX;
                        int worldY = cy * gridCellSize + localY;

                        Vector3Int largeCellPos = new Vector3Int(worldX, worldY, 0);

                        // Pilih variasi jenis dekorasi besar secara konsisten
                        int index = Mathf.FloorToInt(GetDeterministicRandom(cx, cy, 5555) * largeDecorations.Length);
                        index = Mathf.Clamp(index, 0, largeDecorations.Length - 1);

                        if (largeDecorations[index] != null)
                        {
                            decoTilemap2.SetTile(largeCellPos, largeDecorations[index]);
                        }
                    }
                }
            }
        }

        lastMin = min;
        lastMax = max;
        hasPainted = true;
    }

    private float GetDeterministicRandom(int x, int y, int seed)
    {
        int h = x * 374761393 + y * 668265263 + seed * 144655207;
        h = (h ^ (h >> 13)) * 1274126177;
        return Mathf.Abs((float)(h ^ (h >> 16)) / int.MaxValue);
    }
}