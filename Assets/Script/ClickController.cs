using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour {

    Vector2 startTouchPos;
    Vector2 endTouchPos;

    public LayerMask touchLayer;
    public LayerMask gridTouchLayer;
    public LayerMask boxLayer;

    public GameObject placementCube;

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTouchPos = Input.mousePosition;
            Vector2 drag = new Vector2(endTouchPos.x / Screen.width, endTouchPos.y / Screen.height) - new Vector2(startTouchPos.x / Screen.width, startTouchPos.y / Screen.height);

            if (drag.magnitude < 0.05f)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 20.0f, touchLayer))
                {
                    //Detects touch on a tree
                    if (hit.collider.gameObject.tag.Equals("Tree"))
                    {
                        Vector3 treePos = hit.collider.gameObject.transform.parent.position;
                        //Only destroys tree that is in front and to the sides. Cannot destroy tree bellow player.
                        if ((player.transform.position - treePos).magnitude < 5.0f && player.transform.position.z <= treePos.z
                            && !(player.transform.position.x == treePos.x && player.transform.position.z == treePos.z))
                        {
                            Debug.Log("Chop some Wood baby!!!");
                            Destroy(hit.collider.gameObject.transform.parent.gameObject);
                            player.GetComponent<PlayerMovementController>().UpdateAdjacentGrid();
                        }
                    }
                }
                if (Physics.Raycast(ray, out hit, 20.0f, gridTouchLayer))
                {
                    GameObject myCube = Instantiate(placementCube) as GameObject;
                    myCube.transform.position = hit.collider.gameObject.transform.position + (hit.collider.gameObject.transform.up / 2.0f) * 1.3333f;
                    player.GetComponent<PlayerMovementController>().UpdateAdjacentGrid();
                    //Debug.Log("Instanciated cube: " + myCube.name + " at position: " + myCube.transform.position);
                    //Debug.Log("Up vector: "+ hit.collider.gameObject.transform.up);
                }

            }
        }

        //RightClick to destroy cubes!!!
        if (Input.GetMouseButtonDown(1))
        {
            startTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            endTouchPos = Input.mousePosition;
            Vector2 drag = new Vector2(endTouchPos.x / Screen.width, endTouchPos.y / Screen.height) - new Vector2(startTouchPos.x / Screen.width, startTouchPos.y / Screen.height);

            if (drag.magnitude < 0.05f)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 20.0f, boxLayer) && hit.collider.tag.Equals("PlacementBox"))
                {
                    Debug.Log("Destroyed : " + hit.collider + " at Position: " + hit.collider.gameObject.transform.position);
                    DestroyImmediate(hit.collider.gameObject);
                    player.GetComponent<PlayerMovementController>().UpdateAdjacentGrid();
                    player.GetComponent<PlayerMovementController>().CheckPlayerFall();
                }
            }
        }
        /*
        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");
            */

    }
}
