using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    GameObject player;
    Vector3 cameraToPlayer;
    public float chunkWidth;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        cameraToPlayer = player.transform.position - this.transform.position;
    }
	
	void Update () {
        this.transform.position = player.transform.position - cameraToPlayer;
        //this.transform.position = new Vector3(player.transform.position.x + (chunkWidth / 2.0f), this.transform.position.y, this.transform.position.z);
        this.transform.LookAt(player.transform);
    }
}
