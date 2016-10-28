using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placementController : MonoBehaviour
{
    public LayerMask selectedLayers;
    public int overlapCount;

    void Start()
    {
        overlapCount = Physics.OverlapBox(transform.position, Vector3.one * 0.1f, Quaternion.identity, selectedLayers).Length;
        if (Physics.OverlapBox(transform.position, Vector3.one * 0.1f, Quaternion.identity, selectedLayers).Length > 1)
        {
            this.GetComponent<MeshRenderer>().enabled = false;
            this.GetComponent<BoxCollider>().enabled = false;
        }
        else {
            this.GetComponent<BoxCollider>().enabled = true;
            this.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    void Update()
    {
        if (Physics.OverlapBox(transform.position, Vector3.one * 0.1f, Quaternion.identity, selectedLayers).Length > 1)
        {
            this.GetComponent<MeshRenderer>().enabled = false;
            this.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            this.GetComponent<BoxCollider>().enabled = true;
            this.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
