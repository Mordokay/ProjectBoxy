using UnityEngine;
using System.Collections;

public class CoinSpawner : MonoBehaviour {

    public GameObject Coin;
    public GameObject Tree;

    public float treeMinSize = 0.3f;
    public float treeMaxSize = 0.6f;

    public void InstanciateCoins(int[,] mapHeight, int[,] mapTree, float increment, float startX, float startY, float mapX, float mapY, int seed)
    {
        for (int i = 0; i < mapHeight.GetLength(0); i++)
        {
            for (int j = 0; j < mapHeight.GetLength(1); j++)
            {
                //Returns a value between 0.0f and 10.0f
                float myRandom = Random.value * 10.0f;

                if (mapHeight[i, j] == 1 && myRandom < 3.0f)
                {
                    Instantiate(Coin, new Vector3(startX + i * increment - (mapX / 2.0f) * increment, mapHeight[i, j] + 1.0f, startY + j * increment - (mapY / 2.0f) * increment),
                        Quaternion.Euler(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f))),transform);
                }
                else if(mapHeight[i, j] == 2 && mapTree[i, j] == 1.0f)
                {
                    GameObject myTree =Instantiate(Tree, new Vector3(startX + i * increment - (mapX / 2.0f) * increment, mapHeight[i, j] + 0.5f, startY + j * increment - (mapY / 2.0f) * increment),
                        Quaternion.identity, transform) as GameObject;
                    float randomScale = Random.Range(treeMinSize, treeMaxSize);
                    myTree.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                    myTree.transform.RotateAround(myTree.transform.position, Vector3.up, Random.value * 360.0f);
                }
            }
        }
    }
}
