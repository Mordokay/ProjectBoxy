using UnityEngine;
using System.Collections;

public class GlobalMapController : MonoBehaviour {

    public int mapX = 10;
    public int mapY = 10;
    public int seed = 12345;

    float cubeWidth = 4.0f;
    public GameObject mapChunk;
    public GameObject coinManager;
    public float waterLevel = 0.95f;

    public int ChunksX = 3;
    public int ChunksY = 3;
    
    void Start () {

        for (int i = -(int)ChunksX / 2; i <= (int)ChunksX / 2; i++)
        {
            for (int j = -(int)ChunksY / 2; j <= (int)ChunksY / 2; j++)
            {
                GameObject myChunk = Instantiate(mapChunk) as GameObject;
                myChunk.GetComponent<MapTerrainGenerator>().sizeX = mapX;
                myChunk.GetComponent<MapTerrainGenerator>().sizeY = mapY;
                myChunk.GetComponent<MapTerrainGenerator>().seed = this.seed;
                myChunk.GetComponent<MapTerrainGenerator>().offset = new Vector2(i * mapX, j * mapY);
                myChunk.transform.parent = this.transform;

                myChunk.GetComponent<MapTerrainGenerator>().instanciateCubes();

                myChunk.transform.position = new Vector3(i * mapX * cubeWidth, 0.0f, j * mapY * cubeWidth);

                GameObject myCoinManager = Instantiate(coinManager) as GameObject;
                myCoinManager.transform.position = new Vector3(i * mapX * cubeWidth, 0.0f, j * mapY * cubeWidth);
                myCoinManager.transform.parent = this.transform;
                myCoinManager.GetComponent<CoinSpawner>().InstanciateCoins(myChunk.GetComponent<MapTerrainGenerator>().mapHeight, cubeWidth, i * mapX * cubeWidth, j * mapY * cubeWidth);
            }
        }

	}

    void Update()
    {

    }
}
