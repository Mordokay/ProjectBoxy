using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class visibilityColiderDetector : MonoBehaviour {

    public List<GameObject> invisibleList;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag.Equals("PlayerVision"))
        {
            foreach(GameObject myObject in invisibleList)
            {
                Color myColor = myObject.GetComponent<Renderer>().material.color;
                myColor.a = 0.0f;
                myObject.GetComponent<Renderer>().material.color = myColor;
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {

        if (coll.gameObject.tag.Equals("PlayerVision"))
        {
            foreach (GameObject myObject in invisibleList)
            {
                Color myColor = myObject.GetComponent<Renderer>().material.color;
                myColor.a = 1.0f;
                myObject.GetComponent<Renderer>().material.color = myColor;
            }
        }
    }
}
