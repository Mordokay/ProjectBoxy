using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalMapController : MonoBehaviour {

    public int mapX;
    public int mapY;
    public int seed = 12345;

    float cubeWidth = 4.0f;
    float cubeHeight = 1.0f;
    public GameObject mapChunk;
    public GameObject coinManager;

    public int ChunksX = 3;
    public int ChunksY = 3;

    public int oldXCoord = -999;
    public int oldYCoord = -999;

    public int depthOfView;

    public List<ChunkData> myChunks;

    public Vector2 playerStartPos;
    GameObject player;

    void Start () {

        //Maps must always be even number
        if((mapX % 2) != 0)
        {
            mapX += 1;
        }
        if ((mapY % 2) != 0)
        {
            mapY += 1;
        }

        myChunks = new List<ChunkData>();
        player = GameObject.FindGameObjectWithTag("Player");

        for (int i = -(int)ChunksX / 2; i <= (int)ChunksX / 2; i++)
        {
            for (int j = -(int)ChunksY / 2; j <= (int)ChunksY / 2; j++)
            {
                InstanciateChunk(i, j);
            }
        }

        RaycastHit hit;
        Vector3 raycastPos = new Vector3(playerStartPos.x * cubeWidth, 10.0f, playerStartPos.y * cubeWidth);
        if (Physics.Raycast(raycastPos, Vector3.down, out hit, Mathf.Infinity))
        {
           GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(raycastPos.x,
               hit.collider.gameObject.transform.position.y + cubeHeight, raycastPos.z);
        }

        UpdateMap();
    }

    public GameObject InstanciateChunk(int i, int j)
    {
        GameObject myChunk = Instantiate(mapChunk) as GameObject;
        myChunk.name = "mapChunk ( " + i + " , " + j + " )";
        myChunk.GetComponent<MapTerrainGenerator>().sizeX = mapX;
        myChunk.GetComponent<MapTerrainGenerator>().sizeY = mapY;
        myChunk.GetComponent<MapTerrainGenerator>().seed = this.seed;
        myChunk.GetComponent<MapTerrainGenerator>().offset = new Vector2(i * mapX, j * mapY);
        myChunk.transform.parent = this.transform;

        myChunk.GetComponent<MapTerrainGenerator>().instanciateCubes();

        myChunk.transform.position = new Vector3(i * mapX * cubeWidth, 0.0f, j * mapY * cubeWidth);

        GameObject myCoinManager = Instantiate(coinManager) as GameObject;
        myCoinManager.transform.position = new Vector3(i * mapX * cubeWidth, 0.0f, j * mapY * cubeWidth);
        myCoinManager.transform.parent = myChunk.transform;
        myCoinManager.GetComponent<CoinSpawner>().InstanciateCoins(myChunk.GetComponent<MapTerrainGenerator>().mapHeight, cubeWidth, i * mapX * cubeWidth, j * mapY * cubeWidth, mapX, mapY);

        myChunks.Add(new ChunkData(myChunk, i, j, true));

        return myChunk;
    }

    public void UpdateMap() {
        int xCoord = Mathf.RoundToInt(player.transform.position.x / (mapX * cubeWidth));
        int yCoord = Mathf.RoundToInt(player.transform.position.z / (mapY * cubeWidth));

        //Debug.Log("xCoord: " + xCoord + " yCoord: " + yCoord + " oldXCoord: " + oldXCoord + " yCoord: " + yCoord);
        //Player Chunk has changed
        if (xCoord != oldXCoord || xCoord != oldXCoord || yCoord != oldYCoord || yCoord != oldYCoord)
        {
            foreach (ChunkData chunk in myChunks)
            {
                chunk.myChunk.SetActive(false);
            }
            //Debug.Log("xCoord : " + xCoord + " yCoord : " + yCoord);
            //Debug.Log((xCoord - depthOfView) + " <= x <= " + (xCoord + depthOfView) + "  ----  " + (yCoord - depthOfView) + " <= y <= " + (yCoord + depthOfView));
            for (int X = xCoord - depthOfView; X <= xCoord + depthOfView; X++)
            {
                for (int Y = yCoord - depthOfView; Y <= yCoord + depthOfView; Y++)
                {
                    bool chunkFound = false;
                    foreach (ChunkData chunk in myChunks)
                    {
                        if (chunk.posX == X && chunk.posY == Y)
                        {
                            chunk.myChunk.SetActive(true);
                            chunkFound = true;
                            break;
                        }
                    }
                    if (!chunkFound)
                    {
                        InstanciateChunk(X, Y).SetActive(true);
                        Debug.Log("Instanciated a Chunk at position: ( " + X + " , " + Y + " )");
                    }
                }
            }
            oldXCoord = xCoord;
            oldYCoord = yCoord;
        }
    }

    [System.Serializable]
    public class ChunkData
    {
        public GameObject myChunk;
        public int posX;
        public int posY;
        public bool isActive;

        public ChunkData(GameObject myChunk, int i, int j, bool isActive)
        {
            this.myChunk = myChunk;
            this.posX = i;
            this.posY = j;
            this.isActive = isActive;
        }
    }
}
