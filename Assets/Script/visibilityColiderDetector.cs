using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class visibilityColiderDetector : MonoBehaviour {

    public List<GameObject> invisibleList;

    public float opacity;

    public GameObject placementGreen;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag.Equals("PlayerVision"))
        {
            foreach(GameObject myObject in invisibleList)
            {
                Color myColor = myObject.GetComponent<Renderer>().material.color;
                myColor.a = opacity;
                myObject.GetComponent<Renderer>().material.color = myColor;
            }
        }

        if (coll.gameObject.tag.Equals("PlacementVision"))
        {
            placementGreen.SetActive(true);
        }
    }
    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.tag.Equals("PlacementVision"))
        {
            placementGreen.SetActive(true);
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
        if (coll.gameObject.tag.Equals("PlacementVision"))
        {
            placementGreen.SetActive(false);
        }
    }
}
