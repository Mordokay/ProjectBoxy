using UnityEngine;
using System.Collections;

public class BasicItemController : MonoBehaviour {

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag.Equals("ItemCube"))
        {
            Destroy(coll.gameObject);
        }
    }
}
