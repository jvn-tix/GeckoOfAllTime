using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase groundTile;
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

        // Kalau area yang terlihat belum berubah, nggak perlu ngapa-ngapain
        if (hasPainted && min == lastMin && max == lastMax) return;

        tilemap.ClearAllTiles();
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
            }
        }

        lastMin = min;
        lastMax = max;
        hasPainted = true;
    }
}