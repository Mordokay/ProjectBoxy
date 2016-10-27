using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerMovementController : MonoBehaviour
{

    //Swipe percentage based on screen width and height
    float minSwipeDistY = 0.1f;
    float minSwipeDistX = 0.1f;
    public float movementShift = 4.0f;
    public LayerMask boxLayer;
    public LayerMask touchLayer;

    public bool diagonalMovement = false;

    private Vector2 startTouchPos;

    public float jumpGravity = -9.81f;
    public float jumpHeight = 2.0f;
    public float velocityX0;
    public float velocityY0;
    public float velocityZ0;
    public float jumpDurationTime = 0.0f;

    public float totalXRotation = 0;
    public float totalZRotation = 0;

    public float PlayerRotationSpeed = 10.0f;

    public GameObject GlobalMap;

    List<GameObject> adjacentGrid;

    public float startJumpTime;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool Jumping = false;
    public bool doubleJumping = false;
    public bool jumpCalculated = false;
    public bool lockSwipe = false;
    public enum MoveDir
    {
        Front,
        Back,
        Left,
        Right
    }

    Vector3 adjacentPos;

    void Start()
    {
        adjacentGrid = new List<GameObject>();
    }

    void Update()
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(totalXRotation, 0.0f, totalZRotation), Time.deltaTime * PlayerRotationSpeed);
        float angle = Quaternion.Angle(this.transform.rotation, Quaternion.Euler(totalXRotation, 0.0f, totalZRotation));

        if (angle < 1.0f)
        {
            this.transform.rotation = Quaternion.identity;
            totalXRotation = 0.0f;
            totalZRotation = 0.0f;
        }

        if (Jumping)
        {
            //Lerping prevents "falling off the map bug" when sometimes stop condition fails
            if ((this.transform.position - endPos).magnitude > 1.0f)
            {
                if (velocityX0 != 0 && velocityZ0 != 0 && jumpCalculated)
                {
                    this.transform.position = getJumpPos();
                }
                else if (velocityZ0 != 0 && jumpCalculated)
                {
                    this.transform.position = getJumpPosForward();
                }
                else
                {
                    this.transform.position = getJumpPosHorizontal();
                }
            }
            else if ((this.transform.position - endPos).magnitude > 0.005f)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, endPos, Time.deltaTime * (200.0f / jumpHeight));
            }
            else
            {
                Jumping = false;
                doubleJumping = false;
                jumpCalculated = false;
                lockSwipe = false;
                this.transform.position = endPos;
                velocityX0 = 0.0f;
                velocityY0 = 0.0f;
                velocityZ0 = 0.0f;

                GlobalMap.GetComponent<GlobalMapController>().UpdateMap();

                EnableAdjacentGrid();
            }
        }
        if (Input.touchCount > 0 && !lockSwipe)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {

                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    break;

                case TouchPhase.Ended:

                    float swipeDistVertical = (new Vector3(0, touch.position.y / Screen.height, 0) - new Vector3(0, startTouchPos.y / Screen.height, 0)).magnitude;
                    float swipeDistHorizontal = (new Vector3(touch.position.x / Screen.width, 0, 0) - new Vector3(startTouchPos.x / Screen.width, 0, 0)).magnitude;

                    if (!diagonalMovement)
                    {
                        //Only one swipe direction ... no diagonal movement allowed
                        if (swipeDistVertical > swipeDistHorizontal)
                        {
                            if (swipeDistVertical > minSwipeDistY)
                            {
                                float swipeValue = Mathf.Sign(touch.position.y - startTouchPos.y);

                                if (swipeValue > 0)
                                {
                                    Move(MoveDir.Front);
                                    totalXRotation += 90;
                                }
                                else if (swipeValue < 0)
                                {
                                    Move(MoveDir.Back);
                                    totalXRotation -= 90;
                                }
                            }
                        }
                        else
                        {
                            if (swipeDistHorizontal > minSwipeDistX)
                            {
                                float swipeValue = Mathf.Sign(touch.position.x - startTouchPos.x);
                                if (swipeValue > 0)
                                {
                                    Move(MoveDir.Right);
                                    totalZRotation -= 90.0f;
                                }
                                else if (swipeValue < 0)
                                {
                                    Move(MoveDir.Left);
                                    totalZRotation += 90.0f;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (swipeDistVertical > minSwipeDistY)
                        {
                            float swipeValue = Mathf.Sign(touch.position.y - startTouchPos.y);
                            if (swipeValue > 0)
                            {
                                Move(MoveDir.Front);
                                totalXRotation += 90;
                            }
                            else if (swipeValue < 0)
                            {
                                Move(MoveDir.Back);
                                totalXRotation -= 90;
                            }
                        }

                        if (swipeDistHorizontal > minSwipeDistX)
                        {
                            float swipeValue = Mathf.Sign(touch.position.x - startTouchPos.x);
                            if (swipeValue > 0)
                            {
                                Move(MoveDir.Right);
                                totalZRotation -= 90.0f;
                            }
                            else if (swipeValue < 0)
                            {
                                Move(MoveDir.Left);
                                totalZRotation += 90.0f;
                            }
                        }
                    }
                    break;
            }
        }

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && !lockSwipe)
        {
            Move(MoveDir.Left);
            totalZRotation += 90.0f;
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !lockSwipe)
        {
            Move(MoveDir.Right);
            totalZRotation -= 90.0f;
        }
        else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !lockSwipe)
        {
            Move(MoveDir.Front);
            totalXRotation += 90;
        }

        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !lockSwipe)
        {
            Move(MoveDir.Back);
            totalXRotation -= 90;
        }
    }

    public void EnableAdjacentGrid(){

        //Debug.Log("adjacentGrid count: " + adjacentGrid.Count);
        foreach (GameObject grid in adjacentGrid)
        {
            grid.SetActive(false);
        }
        adjacentGrid.Clear();

        RaycastHit hit;

        adjacentPos = this.transform.position + Vector3.forward * movementShift;
        if (Physics.Raycast(adjacentPos + Vector3.up * 100.0f, Vector3.down, out hit, Mathf.Infinity, touchLayer))
        {
            if (hit.collider.tag.Equals("Grid") && Mathf.Abs(hit.collider.transform.position.y + 0.5f - this.transform.position.y) <= 1.0f)
            {
                //Debug.Log("Hit at Pos: " + hit.collider.transform.GetChild(0).name);
                hit.collider.transform.GetChild(0).gameObject.SetActive(true);
                adjacentGrid.Add(hit.collider.transform.GetChild(0).gameObject);
            }
        }

        adjacentPos = this.transform.position - Vector3.forward * movementShift;
        if (Physics.Raycast(adjacentPos + Vector3.up * 100.0f, Vector3.down, out hit, Mathf.Infinity, touchLayer))
        {
            if (hit.collider.tag.Equals("Grid") && Mathf.Abs(hit.collider.transform.position.y + 0.5f - this.transform.position.y) <= 1.0f)
            {
                //Debug.Log("Hit at Pos: " + hit.collider.transform.GetChild(0).name);
                hit.collider.transform.GetChild(0).gameObject.SetActive(true);
                adjacentGrid.Add(hit.collider.transform.GetChild(0).gameObject);
            }
        }

        adjacentPos = this.transform.position - Vector3.right * movementShift;
        if (Physics.Raycast(adjacentPos + Vector3.up * 100.0f, Vector3.down, out hit, Mathf.Infinity, touchLayer))
        {
            if (hit.collider.tag.Equals("Grid") && Mathf.Abs(hit.collider.transform.position.y + 0.5f - this.transform.position.y) <= 1.0f)
            {
                //Debug.Log("Hit at Pos: " + hit.collider.transform.GetChild(0).name);
                hit.collider.transform.GetChild(0).gameObject.SetActive(true);
                adjacentGrid.Add(hit.collider.transform.GetChild(0).gameObject);
            }
        }

        adjacentPos = this.transform.position + Vector3.right * movementShift;
        if (Physics.Raycast(adjacentPos + Vector3.up * 100.0f, Vector3.down, out hit, Mathf.Infinity, touchLayer))
        {
            if (hit.collider.tag.Equals("Grid") && Mathf.Abs(hit.collider.transform.position.y + 0.5f - this.transform.position.y) <= 1.0f)
            {
                //Debug.Log("Hit at Pos: " + hit.collider.transform.GetChild(0).name);
                hit.collider.transform.GetChild(0).gameObject.SetActive(true);
                adjacentGrid.Add(hit.collider.transform.GetChild(0).gameObject);
            }
        }

        adjacentPos = this.transform.position;
        if (Physics.Raycast(adjacentPos + Vector3.up * 100.0f, Vector3.down, out hit, Mathf.Infinity, touchLayer))
        {
            if (hit.collider.tag.Equals("Grid") && Mathf.Abs(hit.collider.transform.position.y + 0.5f - this.transform.position.y) <= 1.0f)
            {
                //Debug.Log("Hit at Pos: " + hit.collider.transform.GetChild(0).name);
                hit.collider.transform.GetChild(0).gameObject.SetActive(true);
                adjacentGrid.Add(hit.collider.transform.GetChild(0).gameObject);
            }
        }
        Debug.Log("adjacentGrid count: " + adjacentGrid.Count);
    }

    private void Move(MoveDir moveType)
    {
        bool mapLimitDetected = false;
        RaycastHit hit;

        /*
        //Map Limit Detection
        switch (moveType)
        {
            case MoveDir.Front:
                if (Physics.Raycast(endPos, Vector3.forward, out hit, movementShift, boxLayer) && hit.transform.tag == "MapLimit")
                {
                    mapLimitDetected = true;
                    return;
                }
                break;
            case MoveDir.Back:
                if (Physics.Raycast(endPos, -Vector3.forward, out hit, movementShift, boxLayer) && hit.transform.tag == "MapLimit")
                {
                    mapLimitDetected = true;
                    return;
                }
                break;
            case MoveDir.Left:
                if (Physics.Raycast(endPos, -Vector3.right, out hit, movementShift, boxLayer) && hit.transform.tag == "MapLimit")
                {
                    mapLimitDetected = true;
                    return;
                }
                break;
            case MoveDir.Right:
                if (Physics.Raycast(endPos, Vector3.right, out hit, movementShift, boxLayer) && hit.transform.tag == "MapLimit")
                {
                    mapLimitDetected = true;
                    return;
                }
                break;
        }
        */
        if (!Jumping)
        {
            Jumping = true;
            startPos = this.transform.position;
            endPos = this.transform.position;
        }
        else if (!doubleJumping)
        {
            doubleJumping = true;
            lockSwipe = true;
            startPos = this.transform.position;
        }

        switch (moveType)
        {
            //The value 100 means that the cube can jump 100.0f down!!!
            case MoveDir.Front:
                endPos += Vector3.forward * movementShift;
                break;

            case MoveDir.Back:
                endPos -= Vector3.forward * movementShift;
                break;

            case MoveDir.Left:
                endPos -= Vector3.right * movementShift;
                break;

            case MoveDir.Right:
                endPos += Vector3.right * movementShift;
                break;
        }
        if (Physics.Raycast(new Vector3(endPos.x, this.transform.position.y, endPos.z) + Vector3.up * jumpHeight * 0.9f, Vector3.down, out hit, 100.0f, boxLayer))
        {
            Debug.Log(hit.collider.gameObject.name);
            if(hit.collider.tag == "PlacementBox")
            {
                endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + hit.collider.gameObject.transform.localScale.y, endPos.z);
            }
            else
            {
                endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + hit.collider.gameObject.transform.parent.localScale.y, endPos.z);
            }
            
        }

        if (doubleJumping)
        {
            lockSwipe = true;
        }

        if (!mapLimitDetected)
        {
            CalculateJump();
        }
    }

    void CalculateJump()
    {
        startJumpTime = Time.time;
        velocityY0 = Mathf.Sqrt(2.0f * jumpGravity * (-jumpHeight));
        jumpDurationTime = (-velocityY0 - Mathf.Sqrt(velocityY0 * velocityY0 - 2.0f * jumpGravity * (startPos.y - endPos.y))) / jumpGravity;

        velocityX0 = (endPos.x - startPos.x) / jumpDurationTime;
        velocityZ0 = (endPos.z - startPos.z) / jumpDurationTime;

        jumpCalculated = true;
    }

    Vector3 getJumpPosHorizontal()
    {
        float x = startPos.x + velocityX0 * (Time.time - startJumpTime);
        float y = startPos.y + velocityY0 * (Time.time - startJumpTime) + (1.0f / 2.0f) * jumpGravity * (Time.time - startJumpTime) * (Time.time - startJumpTime);

        return new Vector3(x, y, startPos.z);
    }
    Vector3 getJumpPosForward()
    {
        float z = startPos.z + velocityZ0 * (Time.time - startJumpTime);
        float y = startPos.y + velocityY0 * (Time.time - startJumpTime) + (1.0f / 2.0f) * jumpGravity * (Time.time - startJumpTime) * (Time.time - startJumpTime);

        return new Vector3(startPos.x, y, z);
    }

    Vector3 getJumpPos()
    {
        float x = startPos.x + velocityX0 * (Time.time - startJumpTime);
        float y = startPos.y + velocityY0 * (Time.time - startJumpTime) + (1.0f / 2.0f) * jumpGravity * (Time.time - startJumpTime) * (Time.time - startJumpTime);
        float z = startPos.z + velocityZ0 * (Time.time - startJumpTime);

        return new Vector3(x, y, z);
    }
}