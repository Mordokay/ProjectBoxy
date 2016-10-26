using UnityEngine;
using System.Collections;

public class TouchController : MonoBehaviour {

    Vector2 startTouchPos;
    Vector2 endTouchPos;

    public LayerMask touchLayer;

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {

                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPos = touch.position;
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
                                }
                                
                            }
                        }
                    }
                    break;
            }

        }
    }
}
