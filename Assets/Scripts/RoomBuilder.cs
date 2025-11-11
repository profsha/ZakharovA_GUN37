using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [Header("Размеры комнаты, м")]
    [SerializeField]
    private float width  = 10;
    [SerializeField]
    private float depth  = 8;
    [SerializeField]
    private float height = 2.5f;

    [Header("Мебель")]
    [SerializeField]
    private FurnitureEntry[] furniture;   // префаб + сколько штук
    [SerializeField]
    private float            minWallDist = 0.5f;
    [SerializeField]
    private float            minItemDist = 0.6f;

    [Header("Зона спавна игрока")]
    [SerializeField]
    private Vector2 spawnZone = new Vector2(2, 2);
    [SerializeField]
    private Color   gizmoCol  = new Color(0, 1, 0, 0.25f);

    [SerializeField] private GameObject playerPrefab;

    [Header("Seed (0 = рандом)")]
    [SerializeField]
    private int seed = 0;

    private Transform parent;
    private List<Bounds> occupied = new List<Bounds>();

    [System.Serializable]
    public class FurnitureEntry
    {
        public GameObject prefab;
        public int        count;
    }

    void Start()
    {
        Generate();
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (playerPrefab == null) return;
        Vector3 center = parent.position + new Vector3(width / 2, 0, depth / 2);
        float randomY = Random.Range(0f, 360f);
        var rotation = Quaternion.Euler(0f, randomY, 0f);
        Instantiate(playerPrefab, center, rotation);
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (parent) DestroyImmediate(parent.gameObject);
        parent = new GameObject("Room").transform;
        parent.position = transform.position;

        occupied.Clear();
        Random.InitState(seed != 0 ? seed : System.Environment.TickCount);

        BuildFloor();
        Build4Walls();
        PlaceFurniture();
    }

    void BuildFloor()
    {
        float w = width;
        float d = depth;
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.parent = parent;
        floor.transform.localPosition = new Vector3(w / 2, 0, d / 2);
        floor.transform.localScale = new Vector3(width / 10f, 1, depth / 10f);
    }

    void Build4Walls()
    {
        float w = width;
        float d = depth;
        float h = height;
        float t = 0.2f;

        SpawnWallCube("WallFront", new Vector3(w, h, t), new Vector3(w / 2, h / 2, -t / 2));
        SpawnWallCube("WallBack",   new Vector3(w, h, t), new Vector3(w / 2, h / 2, d + t / 2));
        SpawnWallCube("WallLeft",   new Vector3(t, h, d), new Vector3(-t / 2, h / 2, d / 2));
        SpawnWallCube("WallRight",  new Vector3(t, h, d), new Vector3(w + t / 2, h / 2, d / 2));
    }

    void SpawnWallCube(string name, Vector3 scale, Vector3 localPos)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.parent = parent;
        wall.transform.localPosition = localPos;
        wall.transform.localScale = scale;
    }

    void PlaceFurniture()
    {
        foreach (var entry in furniture)
        {
            if (entry.prefab == null) continue;
            for (int i = 0; i < entry.count; i++)
            {
                Vector3 p = RandomPoint();
                if (IsFree(p, entry.prefab))
                {
                    GameObject go = Instantiate(entry.prefab, parent);
                    go.transform.position = p;
                    go.transform.rotation = RandomYRot();
                    RegisterBounds(go);
                }
            }
        }
    }

    Vector3 RandomPoint()
    {
        return parent.position + new Vector3(
            Random.Range(minWallDist, width  - minWallDist),
            0,
            Random.Range(minWallDist, depth - minWallDist));
    }

    Quaternion RandomYRot() => Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);

    bool IsFree(Vector3 pos, GameObject prefab)
    {
        Vector3 center = parent.position + new Vector3(width / 2, 0, depth / 2);
        Vector3 local  = pos - center;
        if (Mathf.Abs(local.x) < spawnZone.x / 2 &&
            Mathf.Abs(local.z) < spawnZone.y / 2) return false;

        Bounds b = PrefabBounds(prefab);
        b.center = pos + Vector3.up * b.extents.y;
        foreach (var o in occupied)
            if (b.Intersects(o)) return false;
        return true;
    }

    Bounds PrefabBounds(GameObject prefab)
    {
        Renderer r = prefab.GetComponentInChildren<Renderer>();
        return r ? r.bounds : new Bounds(Vector3.zero, Vector3.one);
    }

    void RegisterBounds(GameObject go)
    {
        Renderer r = go.GetComponentInChildren<Renderer>();
        if (r) occupied.Add(r.bounds);
    }

    void OnDrawGizmosSelected()
    {
        if (!parent) return;
        Gizmos.color = gizmoCol;
        Vector3 c = parent.position + new Vector3(width / 2, 0.1f, depth / 2);
        Gizmos.DrawCube(c, new Vector3(spawnZone.x, 0.2f, spawnZone.y));
    }
    
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Build Room")]
    static void BuildRoomEditor()
    {
        var rg = FindObjectOfType<RoomBuilder>();
        if (rg) rg.Generate();
    }
#endif
}
