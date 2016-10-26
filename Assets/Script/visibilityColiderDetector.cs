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
                //Debug.Log(myObject.name + "  Color: " + myColor);
                myObject.GetComponent<Renderer>().material.color = myColor;
                
            }
            //Debug.Log("GameObject entered VisionController: " + coll.gameObject.name + " at position:   " + this.transform.position);
            //Debug.Log("Destroyed : " + coll.gameObject.name);
            //Destroy(coll.gameObject);
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
            //Debug.Log("GameObject exited VisionController: " + coll.gameObject.name + " at position:   " + coll.gameObject.transform.position);
            //Debug.Log("Destroyed : " + coll.gameObject.name);
            //Destroy(coll.gameObject);
        }
    }
}
