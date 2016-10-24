using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

    public Vector3 rotation;

	void Update () {
        this.transform.Rotate(rotation * Time.deltaTime);
	}
}
