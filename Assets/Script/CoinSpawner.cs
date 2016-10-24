using UnityEngine;
using System.Collections;

public class CoinSpawner : MonoBehaviour {

    public GameObject Coin;

    public void InstanciateCoins(int[,] mapHeight, float increment, float startX, float startY)
    {
        for (int i = 0; i < mapHeight.GetLength(0); i++)
        {
            for (int j = 0; j < mapHeight.GetLength(1); j++)
            {
                if (mapHeight[i, j] == 1 && Random.Range(0.0f, 10.0f) < 3.0f)
                {
                    Instantiate(Coin, new Vector3(startX + i * increment, mapHeight[i, j] + 1.0f, startY + j * increment),
                        Quaternion.Euler(new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f))),transform);
                }
            }
        }
    }
}
