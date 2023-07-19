using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //For player movement
    public float speed = 20f;
    private float horizontalInput;

    //For ladder
    public bool onLadder;
    public bool goingDown = false;
    public float ladderSpeed = 10f;
    public float ladderLength = 38.61533f;

    //For moving platform
    public MovingPlatform movingPlatform = null;

    //For box
    public Box box = null;

    //Animator
    private Animator playerAnim;

    //For door
    private bool doorDelay = true;
    public bool rotateAnimation = false;

    //For camera
    public GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerAnim.SetTrigger("");
    }

    private void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (!onLadder)
        {
            transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime*-1);
        }
        if (horizontalInput != 0)
        {
            Camera.GetComponent<Rotate>().peekBack();
        }
    }

    private void MoveWithPlatform()
    {
        transform.Translate(Vector3.right * Time.deltaTime * movingPlatform.speed * movingPlatform.direction);
    }

    private void MoveBox(Vector3 vector)
    {
        //Change animation and update speed
        speed = 10f;
        //Move the box
        box.Move(vector, speed);
    }

    IEnumerator goingDownReset()
    {
        yield return new WaitForSeconds(2.0f);
        goingDown = false;
    }

    IEnumerator doorCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        doorDelay = true;
    }

    IEnumerator goInDoorAnimation(Collider other)
    {
        rotateAnimation = true;
        Door door = other.gameObject.GetComponent<Door>();
        Door connectingDoor = door.connectingDoor.GetComponent<Door>();
        //Doors becomes open

        //Player moves into door
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        //Camera rotates
        Camera.GetComponent<Rotate>().rotate(connectingDoor.side, false);
        yield return new WaitForSeconds(5.0f);
        //Player teleports
        transform.position = door.connectingDoor.transform.position;
        transform.rotation = door.connectingDoor.transform.rotation;
        yield return new WaitForSeconds(2.0f);
        //Player moves out of door
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        //Close door
        //Cooldown
        doorDelay = false;
        rotateAnimation = false;
        StartCoroutine(doorCooldown());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!rotateAnimation)
        {
            //For box collision
            if (collision.gameObject.CompareTag("Box"))
            {
                box = collision.gameObject.GetComponent<Box>();
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    MoveBox(Vector3.left);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    MoveBox(Vector3.right);
                }
            }
            else
            {
                box = null;
            }

            //For moving platform collision
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                movingPlatform = collision.gameObject.GetComponent<MovingPlatform>();
                MoveWithPlatform();
            }
            else
            {
                movingPlatform = null;
            }

            //For ground collision
            if (collision.gameObject.CompareTag("Ground"))
            {
                onLadder = false;
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!rotateAnimation)
        {
            //For ladder collision
            if (other.gameObject.CompareTag("Ladder"))
            {

                //Makes the player go up the ladder
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    //Checks if player is going up or down
                    if (transform.position.y < other.gameObject.transform.position.y + (ladderLength / 2))
                    {
                        onLadder = true;
                        if (transform.rotation.eulerAngles.y == 90 || transform.rotation.eulerAngles.y == 270)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, other.gameObject.transform.position.z);
                        }
                        else
                        {
                            transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, transform.position.z);
                        }
                        transform.Translate(Vector3.up * Time.deltaTime * ladderSpeed, Space.World);
                        gameObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    if (transform.position.y >= other.gameObject.transform.position.y - (ladderLength / 2) + 12)
                    {
                        onLadder = true;
                        if (transform.rotation.eulerAngles.y == 90 || transform.rotation.eulerAngles.y == 270)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, other.gameObject.transform.position.z);
                        }
                        else
                        {
                            transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, transform.position.z);
                        }
                        transform.Translate(Vector3.down * Time.deltaTime * ladderSpeed, Space.World);
                        gameObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                    else
                    {
                        gameObject.GetComponent<Rigidbody>().useGravity = true;
                        onLadder = false;
                    }
                }

                ////Checks when the player is at the top of the ladder
                if (transform.position.y >= other.gameObject.transform.position.y + (ladderLength / 2) && !goingDown)//TODO-Make the number fit pixels
                {
                    transform.position = new Vector3(transform.position.x, other.transform.position.y + (ladderLength / 2) + 3, transform.position.z);
                    transform.Translate(Vector3.forward);
                    gameObject.GetComponent<Rigidbody>().useGravity = true;
                    onLadder = false;
                }
            }
            //For top of ladder collision
            if (other.gameObject.CompareTag("TopOfLadder"))
            {
                //Makes the player go down the ladder
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    //Checks if the player is going up or down
                    goingDown = true;
                    transform.Translate(Vector3.back * 10);
                    gameObject.GetComponent<Rigidbody>().useGravity = true;
                    onLadder = true;
                    if (transform.rotation.eulerAngles.y == 90 || transform.rotation.eulerAngles.y == 270)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, other.gameObject.transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, transform.position.z);
                    }

                    StartCoroutine(goingDownReset());

                }
            }

            //For door collision
            if (other.gameObject.CompareTag("Door"))
            {
                //Popup-TODO
                if (Input.GetKeyDown(KeyCode.E) && doorDelay)
                {
                    StartCoroutine(goInDoorAnimation(other));
                }
            }

            //For Peeking
            if (other.gameObject.CompareTag("PeekColliderLeft"))
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    Camera.GetComponent<Rotate>().rotate(90, true);
                }
            }
            if (other.gameObject.CompareTag("PeekColliderRight"))
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    Camera.GetComponent<Rotate>().rotate(-90, true);
                    rotateAnimation = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!rotateAnimation)
        {
            Move();
        }
    }
}