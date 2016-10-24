using UnityEngine;
using System.Collections;

public class followObject : MonoBehaviour {

    public GameObject myObject;

    public bool canFixX;
    public float fixX;

    public bool canFixY;
    public float fixY;

    public bool canFixZ;
    public float fixZ;

    void Update () {
        transform.position = myObject.transform.position;
        if (canFixX)
            transform.position = new Vector3(fixX, myObject.transform.position.y, myObject.transform.position.z);
        if (canFixY)
            transform.position = new Vector3(myObject.transform.position.x, fixY, myObject.transform.position.z);
        if (canFixZ)
            transform.position = new Vector3(myObject.transform.position.x, myObject.transform.position.y, fixZ);
    }
}
