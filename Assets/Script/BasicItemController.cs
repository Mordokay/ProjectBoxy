using UnityEngine;
using System.Collections;

public class BasicItemController : MonoBehaviour {

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag.Equals("ItemCube"))
        {
            //Debug.Log("Destroyed : " + coll.gameObject.name);
            Destroy(coll.gameObject);
        }
    }
}
