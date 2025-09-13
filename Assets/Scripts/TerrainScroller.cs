using UnityEngine;
using UnityEngine.UIElements;

public class TerrainScroller : MonoBehaviour
{
    private Terrain terrain;
    public int layerIndex;
    public float scrollSpeed = 3f;
    public GameObject teleportVolume;
    public GameObject defaultVolume;

    private Vector2 offset;
    private bool running = false;


    private void Start()
    {
        terrain = GetComponent<Terrain>();
    }

    private void OnEnable()
    {
        Teleport.teleportEvent += Running;
    }

    private void OnDisable()
    {
        Teleport.teleportEvent -= Running;
    }

    private void Update()
    {
        if (terrain != null && running)
        {
            TerrainLayer[] layers = terrain.terrainData.terrainLayers;
            if (layers.Length > layerIndex)
            {
                offset.x += scrollSpeed * Time.deltaTime;
                offset.y += scrollSpeed * Time.deltaTime;
                layers[layerIndex].tileOffset = offset;
            }
        }
    }

    void Running()
    {
        running = !running;
        teleportVolume.SetActive(running);
        defaultVolume.SetActive(!running);
    }
}
