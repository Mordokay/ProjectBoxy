using UnityEngine;
using System.Collections;
using System;

public class PlayerMovementController : MonoBehaviour {

    //Swipe percentage based on screen width and height
    float minSwipeDistY = 0.1f;
    float minSwipeDistX = 0.1f;
    public float movementShift = 4.0f;
    public LayerMask boxLayer;
    public float cubeHeight = 1.0f;

    public bool diagonalMovement = false;

    private Vector2 startTouchPos;

    GameManager gm;

    public float jumpGravity = -9.81f;
    public float jumpHeight = 2.0f;
    public float velocityX0;
    public float velocityY0;
    public float velocityZ0;
    public float jumpDurationTime = 0.0f;

    public float totalXRotation = 0;
    public float totalZRotation = 0;

    public float PlayerRotationSpeed = 300.0f;

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

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        RaycastHit hit;
        if (Physics.Raycast(Vector3.zero + new Vector3(0.0f, 10.0f, 0.0f), Vector3.down, out hit, Mathf.Infinity, boxLayer))
        {
            this.transform.position = new Vector3(0.0f, hit.collider.gameObject.transform.position.y + cubeHeight, 0.0f);
        }
    }

    void Update()
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(totalXRotation, 0.0f, totalZRotation), Time.deltaTime * 10.0f);
        float angle = Quaternion.Angle(this.transform.rotation, Quaternion.Euler(totalXRotation, 0.0f, totalZRotation));

        if (angle < 1.0f)
        {
            this.transform.rotation = Quaternion.identity;
            totalXRotation = 0.0f;
            totalZRotation = 0.0f;
        }
        
        if (Jumping)
        {
            //Debug.Log("velocityX0: " + velocityX0 + "velocityZ0: " + velocityZ0);
            //Both Movements
            if (velocityX0 != 0 && velocityZ0 != 0 && jumpCalculated)
            {
                //Debug.Log("Both Movements: " + (new Vector3(getJumpPosHorizontal().x, getJumpPosHorizontal().y, getJumpPosForward().z) - this.transform.position));
                this.transform.position = getJumpPos();
            }
            //Horizontal Movement
            else if (velocityX0 != 0 && jumpCalculated)
            {
                //Debug.Log("getJumpPosHorizontal:  " + getJumpPosHorizontal());
                this.transform.position = getJumpPosHorizontal();
            }
            //Forward Movement
            else if (velocityZ0 != 0 && jumpCalculated)
            {
                //Debug.Log("getJumpPosForward:  " + getJumpPosForward());
                this.transform.position = getJumpPosForward();
            }

            if((this.transform.position - endPos).magnitude < 0.4f)
            {
                //Debug.Log("#############################################");
                Jumping = false;
                doubleJumping = false;
                jumpCalculated = false;
                lockSwipe = false;
                this.transform.position = endPos;
                velocityX0 = 0.0f;
                velocityY0 = 0.0f;
                velocityZ0 = 0.0f;
            }
            //Debug.Log((this.transform.position - endPos).magnitude);
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
                                    //Debug.Log("Up Swipe");
                                }
                                else if (swipeValue < 0)
                                {
                                    Move(MoveDir.Back);
                                    totalXRotation -= 90;
                                    //Debug.Log("Down Swipe");
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
                                    //Debug.Log("Right Swipe");
                                }
                                else if (swipeValue < 0)
                                {
                                    Move(MoveDir.Left);
                                    totalZRotation += 90.0f;
                                    //Debug.Log("left Swipe");
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
                                //Debug.Log("Up Swipe");
                            }
                            else if (swipeValue < 0)
                            {
                                Move(MoveDir.Back);
                                //Debug.Log("Down Swipe");
                            }
                        }

                        if (swipeDistHorizontal > minSwipeDistX)
                        {
                            float swipeValue = Mathf.Sign(touch.position.x - startTouchPos.x);
                            if (swipeValue > 0)
                            {
                                Move(MoveDir.Right);
                                //Debug.Log("Right Swipe");
                            }
                            else if (swipeValue < 0)
                            {
                                Move(MoveDir.Left);
                                //Debug.Log("left Swipe");
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void Move(MoveDir moveType)
    {
        bool mapLimitDetected = false;
        RaycastHit hit;

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
                //Going up detection
                if (Physics.Raycast(endPos, Vector3.forward, out hit, movementShift, boxLayer))
                {
                    //print("Found an object Up - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.up * cubeHeight);
                    //endPos += (Vector3.up * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                //Going Down detection
                else if (!Physics.Raycast(endPos + Vector3.forward * movementShift, Vector3.down, out hit, cubeHeight / 1.5f, boxLayer) &&
                    Physics.Raycast(endPos + Vector3.forward * movementShift, Vector3.down, out hit, cubeHeight * 100.0f, boxLayer))
                {
                    //print("Found an object Down - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.down * cubeHeight);
                    //endPos += (Vector3.down * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                endPos += (Vector3.forward * movementShift);
                break;

            case MoveDir.Back:
                //Going up detection
                if (Physics.Raycast(endPos, -Vector3.forward, out hit, movementShift, boxLayer))
                {
                    //print("Found an object Up - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.up * cubeHeight);
                    //endPos += (Vector3.up * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                //Going Down detection
                else if (!Physics.Raycast(endPos - Vector3.forward * movementShift, Vector3.down, out hit, cubeHeight / 1.5f, boxLayer) &&
                    Physics.Raycast(endPos - Vector3.forward * movementShift, Vector3.down, out hit, cubeHeight * 100.0f, boxLayer))
                {
                    //print("Found an object Down - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.down * cubeHeight);
                    //endPos += (Vector3.down * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                endPos += (-Vector3.forward * movementShift);
                break;

            case MoveDir.Left:
                //Going up detection
                if (Physics.Raycast(endPos, -Vector3.right, out hit, movementShift, boxLayer))
                {
                    //print("Found an object Up - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.up * cubeHeight);
                    //endPos += (Vector3.up * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                //Going Down detection
                else if (!Physics.Raycast(endPos - Vector3.right * movementShift, Vector3.down, out hit, cubeHeight / 1.5f, boxLayer) &&
                    Physics.Raycast(endPos - Vector3.right * movementShift, Vector3.down, out hit, cubeHeight * 100.0f, boxLayer))
                {
                    //print("Found an object Down - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.down * cubeHeight);
                    //endPos += (Vector3.down * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                endPos += (-Vector3.right * movementShift);
                break;

            case MoveDir.Right:
                //Going up detection
                if (Physics.Raycast(endPos, Vector3.right, out hit, movementShift, boxLayer))
                {
                    //print("Found an object Up - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.up * cubeHeight);
                    //endPos += (Vector3.up * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                //Going Down detection
                else if (!Physics.Raycast(endPos + Vector3.right * movementShift, Vector3.down, out hit, cubeHeight / 1.5f, boxLayer) &&
                    Physics.Raycast(endPos + Vector3.right * movementShift, Vector3.down, out hit, cubeHeight * 100.0f, boxLayer))
                {
                    //print("Found an object Down - distance: " + hit.distance + " Called: " + hit.collider.gameObject.name);
                    //this.transform.Translate(Vector3.down * cubeHeight);
                    //endPos += (Vector3.down * cubeHeight);
                    endPos = new Vector3(endPos.x, hit.collider.gameObject.transform.position.y + cubeHeight, endPos.z);
                }
                endPos += (Vector3.right * movementShift);
                break;
        }

        if (doubleJumping)
        {
            lockSwipe = true;
            //Debug.Log("lockSwipe!!!");
        }

        if (!mapLimitDetected)
        {
            CalculateJump();
        }
        else
        {
            Debug.Log("banana!!!!");
        }
    }

    void CalculateJump()
    {
        startJumpTime = Time.time;
        velocityY0 = Mathf.Sqrt(2.0f * jumpGravity * (-jumpHeight));
        //jumpDurationTime = (-velocityY0 - Mathf.Sqrt(velocityY0 * velocityY0 - 2.0f * jumpGravity * (startPos.y - endPos.y))) / jumpGravity;
        jumpDurationTime = (-velocityY0 - Mathf.Sqrt(velocityY0 * velocityY0 - 2.0f * jumpGravity * (startPos.y - endPos.y))) / jumpGravity;

        velocityX0 = (endPos.x - startPos.x) / jumpDurationTime;
        velocityZ0 = (endPos.z - startPos.z) / jumpDurationTime;
        
        //Debug.Log("startPos: " + startPos + "endPos: " + endPos + "startJumpTime:  " + startJumpTime + "Velocity Y0: " + velocityY0 + "Velocity X0: " + velocityX0 + "Velocity Z0: " + velocityZ0 + " Jump Duration Time: " + jumpDurationTime);
        jumpCalculated = true;
    }

    Vector3 getJumpPosHorizontal()
    {
        float x = startPos.x + velocityX0 * (Time.time - startJumpTime);
        float y = startPos.y + velocityY0 * (Time.time - startJumpTime) + (1.0f/2.0f) * jumpGravity * (Time.time - startJumpTime) * (Time.time - startJumpTime);

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
