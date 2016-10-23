using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    GameObject player;
    Vector3 cameraToPlayer;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        cameraToPlayer = player.transform.position - this.transform.position;
        this.transform.LookAt(player.transform);
    }
	
	void Update () {
        this.transform.position = player.transform.position - cameraToPlayer;
    }
}
