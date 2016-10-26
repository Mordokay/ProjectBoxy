using UnityEngine;
using System.Collections;

public class VisionController : MonoBehaviour {

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void Update () {
        this.transform.localScale = new Vector3(1.0f, 1.0f, (player.transform.position - Camera.main.transform.position).magnitude * 0.9f);
        this.transform.position = Camera.main.transform.position + (player.transform.position - Camera.main.transform.position) / 2.0f;
        this.transform.LookAt(player.transform);
    }
}
