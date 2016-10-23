using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public int sizeX = 20;
    public int sizeY = 20;
    public GameObject myCube;
    public GameObject cubeLimit;
    public TerrainType[] terrain;
    public float cubeScale = 4.0f;

    public Noise.NormalizeMode normalizedMode;
    [Tooltip("Generates a map based on a number")]
    public int seed = 1234;
    public Vector2 offset;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;
    public int[ , ] mapHeight;

    public bool autoUpdate;

    public Vector2 mapCenter = Vector2.zero;
    GameManager gm;

    void Start() {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gm.terrainCount = terrain.Length;
        instanciateCubes();
    }

    void CreateHeight()
    {
        mapHeight = new int[sizeX, sizeY];

        float[,] noiseMap = Noise.GeneratedNoiseMap(sizeX, sizeY, seed, noiseScale, octaves, persistence, lacunarity, mapCenter + offset, normalizedMode);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (noiseMap[x, y] >= 1)
                    noiseMap[x, y] = 0.999f;
                if (noiseMap[x, y] <= 0)
                    noiseMap[x, y] = 0.001f;

                mapHeight[x, y] = (int)((noiseMap[x, y] * 10.0f) / ((1.0f / terrain.Length) * 10.0f));
                //Debug.Log("just noise:  " + noiseMap[x, y] + "Noise: " + (noiseMap[x, y] * 10.0f) / ((1.0f / terrain.Length) * 10.0f) + "height:  " + mapHeight[x, y]);
            }
        }
    }

    public void removeMap()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        if (this.transform.childCount > 0)
        {
            //Debug.Log("RemovingAgain!!!!:  " + this.transform.childCount);
            removeMap();
        }
    }

    public void instanciateCubes() {

        removeMap();

        CreateHeight();

        for (int x = -1; x <= sizeX; x++)
        {
            for (int y = -1; y <= sizeY; y++)
            {
                if (x < 0 || x == sizeX || y < 0 || y == sizeY)
                {
                    GameObject myCubeLimit = Instantiate(cubeLimit) as GameObject;
                    myCubeLimit.transform.position = new Vector3(x * cubeScale, 0.0f, y * cubeScale);
                    myCubeLimit.transform.parent = this.transform;
                }
                else
                {
                    int currentheight = mapHeight[x, y];
                    int chosenMaterial = 0;
                    for (int i = 0; i < terrain.Length; i++)
                    {
                        if (currentheight >= terrain[i].height)
                        {
                            chosenMaterial = i;
                        }
                        else
                        {
                            break;
                        }
                    }
                    GameObject cubeAux = Instantiate(myCube) as GameObject;
                    cubeAux.transform.position = new Vector3(x * cubeScale, (int)(mapHeight[x, y]), y * cubeScale);
                    cubeAux.GetComponent<Renderer>().material = terrain[chosenMaterial].material;
                    cubeAux.transform.parent = this.transform;
                }
            }
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public int height;
        public Material material;
    }
}
