using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //UI
    public GameObject key;

    //Player spawn
    public Vector3 spawnPoint;

    //Player components
    private Animator playerAnim;
    private SpriteRenderer spriteRenderer;
    private Rigidbody rb;

    //For Animation
    private IDictionary<string, int> animationDictionary = new Dictionary<string,int>() {
        { "idle", 0},
        {"walk",0 },
        {"run",0 },
        {"climb",0 }
    };

    //For player movement
    public float speed = 20f;
    private float horizontalInput;
    public float velocityMax = 50f;

    //For ladder
    public bool onLadder;
    public bool goingDown = false;
    public float ladderSpeed = 10f;
    public float ladderLength = 38.61533f;

    //For moving platform
    public MovingPlatform movingPlatform = null;

    //For box
    public GameObject box = null;
    public float forceMagnitude = 5f;


    //For door
    private bool doorDelay = true;
    public bool rotateAnimation = false;
    public int keyCount = 0;
    public float startDoorSide;
    public float endDoorSide;
    public bool lockX = false;

    //For camera
    public GameObject Camera;
    private bool peeking = false;

    //For gravity
    public float gravityMultiplier = 30f;
    

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();

        //For gravity
        print(Physics.gravity);
        Physics.gravity *= gravityMultiplier;
        print(Physics.gravity);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(1f,1f);

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        spawnPoint = transform.position;
        StartCoroutine(spawnAnimation());
    }

    private void Move()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (!onLadder)
        {
            if(Mathf.Abs(rb.velocity.x) < velocityMax && Mathf.Abs(rb.velocity.z) < velocityMax)
            {
                //rb.velocity = transform.right * horizontalInput * speed;
                rb.AddForce(transform.right * horizontalInput * speed);
                // print(transform.right * horizontalInput * speed);
            }
            if (horizontalInput != 0)
            {
                if (animationDictionary["walk"] == 0)
                {
                    playerAnim.SetTrigger("walk");
                    resetAnimations("walk");
                }
                Camera.GetComponent<Rotate>().peekBack();
                peeking = false;
            }
            if (horizontalInput == 0 && animationDictionary["idle"] == 0)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                playerAnim.SetTrigger("idle");
                resetAnimations("idle");
            }
            if (horizontalInput > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalInput < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    private void resetAnimations(string animation)
    {
        List<string> keys = new List<string>(animationDictionary.Keys);
        foreach (string anim in keys)
        {
            if (anim!=animation) {
                animationDictionary[anim] = 0;
            }
            else
            {
                animationDictionary[anim] = 1;
            }

        }
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

    IEnumerator spawnAnimation()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(0.5f);
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(0.5f);
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        rotateAnimation = true;
        yield return new WaitForSeconds(1.0f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        rotateAnimation = false;
        spawnPoint = transform.position;
    }

    IEnumerator goInDoorAnimation(Collider other)
    {
        rb.velocity = Vector3.zero;
        rotateAnimation = true;
        Door door = other.gameObject.GetComponent<Door>();
        startDoorSide = door.side;
        Door connectingDoor = door.connectingDoor.GetComponent<Door>();
        endDoorSide = connectingDoor.side;
        //Doors becomes open
        other.GetComponent<MeshRenderer>().material = door.doorOpen;
        door.connectingDoor.GetComponent<MeshRenderer>().material = door.doorOpen;
        if(Math.Abs(startDoorSide-endDoorSide)==90|| Math.Abs(startDoorSide - endDoorSide) == 270)
        {
            lockX= !lockX;
        }
        //Player moves into door
        playerAnim.SetTrigger("climb");
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        //Turn player invisible
        spriteRenderer.enabled = false;
        //Camera rotates
        Camera.GetComponent<Rotate>().rotate(connectingDoor.side, false);
        yield return new WaitForSeconds(2f);
        //Player teleports
        playerAnim.SetTrigger("idle");
        transform.position = door.connectingDoor.transform.position;
        Vector3 angles = door.connectingDoor.transform.rotation.eulerAngles;
        Vector3 newAngles = new Vector3(angles.x, angles.y, transform.rotation.eulerAngles.z);
        Quaternion finishAngles = new Quaternion();
        finishAngles.eulerAngles = newAngles;
        transform.rotation = finishAngles;
        // Player reappears
        spriteRenderer.enabled = true;
        //Player moves out of door
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        if (lockX)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
            yield return new WaitForSeconds(0.5f);
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            yield return new WaitForSeconds(0.5f);
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            yield return new WaitForSeconds(0.5f);
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
            yield return new WaitForSeconds(0.5f);
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        //Close door
        other.GetComponent<MeshRenderer>().material = door.doorClose;
        door.connectingDoor.GetComponent<MeshRenderer>().material = door.doorClose;
        //Update spawnpoint
        spawnPoint = transform.position;
        //Cooldown
        doorDelay = false;
        rotateAnimation = false;
        StartCoroutine(doorCooldown());
    }

    IEnumerator goInEndDoorAnimation(Collider other)
    {
        rotateAnimation = true;
        EndDoor enddoor = other.gameObject.GetComponent<EndDoor>();
        //Doors becomes open
        other.GetComponent<MeshRenderer>().material = enddoor.doorOpen;
        //Player moves into door
        playerAnim.SetTrigger("climb");
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.forward);
        yield return new WaitForSeconds(0.1f);
        //Turn player invisible
        spriteRenderer.enabled = false;
        //Close door
        other.GetComponent<MeshRenderer>().material = enddoor.doorClose;
        //Cooldown
        doorDelay = false;
        rotateAnimation = false;
        StartCoroutine(doorCooldown());
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!rotateAnimation)
        {
            //For ground collision
            if (collision.gameObject.CompareTag("Ground"))
            {
                onLadder = false;
                if (animationDictionary["idle"] == 0 && animationDictionary["walk"]==0)
                {
                    playerAnim.SetTrigger("idle");
                    resetAnimations("idle");
                }
            }

            //For Box collision
            if (collision.gameObject.CompareTag("Box")&& rb.velocity.x <1 && rb.velocity.z<1)
            {
                if (animationDictionary["run"] == 0)
                {
                    playerAnim.SetTrigger("run");
                    resetAnimations("run");
                }
                box = collision.gameObject;
                box.GetComponent<Rigidbody>().AddForce(new Vector3(box.transform.position.x-transform.position.x, 0, box.transform.position.z - transform.position.z));
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
                        if (animationDictionary["climb"] == 0)
                        {
                            playerAnim.SetTrigger("climb");
                            resetAnimations("climb");
                        }
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
                        if (animationDictionary["climb"] == 0)
                        {
                            playerAnim.SetTrigger("climb");
                            resetAnimations("climb");
                        }
                    }
                    else
                    {
                        gameObject.GetComponent<Rigidbody>().useGravity = true;
                        onLadder = false;
                        playerAnim.SetTrigger("idle");
                        resetAnimations("idle");
                        
                    }
                }

                //Checks when the player is at the top of the ladder
                if (transform.position.y >= other.gameObject.transform.position.y + (ladderLength / 2) && !goingDown)
                {
                    transform.position = new Vector3(transform.position.x, other.transform.position.y + (ladderLength / 2) + 3, transform.position.z);
                    if (other.gameObject.transform.parent.GetComponent<Ladder>().sideLadder)
                    {
                        if (other.gameObject.transform.parent.GetComponent<Ladder>().offLadder =="left")
                        {
                            transform.Translate(Vector3.left);
                        }
                        else if (other.gameObject.transform.parent.GetComponent<Ladder>().offLadder == "right")
                        {
                            transform.Translate(Vector3.right);
                        }

                    }
                    else
                    {
                        transform.Translate(Vector3.forward);
                    }
                    gameObject.GetComponent<Rigidbody>().useGravity = true;
                    onLadder = false;
                    playerAnim.SetTrigger("idle");
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
                    if (other.gameObject.transform.parent.GetComponent<Ladder>().sideLadder)
                    {
                        if (other.gameObject.transform.parent.GetComponent<Ladder>().offLadder == "left")
                        {
                            transform.Translate(Vector3.right*10);
                        }
                        else if (other.gameObject.transform.parent.GetComponent<Ladder>().offLadder == "right")
                        {
                            transform.Translate(Vector3.left*10);
                        }

                    }
                    else {
                        transform.Translate(Vector3.back * 10);
                    }
                    gameObject.GetComponent<Rigidbody>().useGravity = true;
                    onLadder = true;
                    playerAnim.SetTrigger("climb");
                    if (transform.rotation.eulerAngles.y == 90 || transform.rotation.eulerAngles.y == 270)
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotation;
                        transform.position = new Vector3(transform.position.x, transform.position.y, other.gameObject.transform.position.z);
                        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
                    }
                    else
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotation;
                        transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, transform.position.z);
                        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                    }

                    StartCoroutine(goingDownReset());

                }
            }

            //For door collision
            if (other.gameObject.CompareTag("Door"))
            {
                if (!other.gameObject.GetComponent<Door>().startingDoor)
                {
                    //Popup
                    other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E) && doorDelay)
                    {
                        if (other.gameObject.GetComponent<Door>().unlocked)
                        {
                            StartCoroutine(goInDoorAnimation(other));
                        }
                        if (!other.gameObject.GetComponent<Door>().unlocked)
                        {
                            if (keyCount > 0)
                            {
                                other.gameObject.GetComponent<Door>().unlocked = true;
                                other.gameObject.GetComponent<Door>().connectingDoor.GetComponent<Door>().unlocked = true;
                                if (other.gameObject.GetComponent<Door>().hasLock)
                                {
                                    Destroy(other.gameObject.GetComponent<Door>().Lock);
                                    Destroy(other.gameObject.GetComponent<Door>().connectingDoor.GetComponent<Door>().Lock);
                                }
                                keyCount--;
                                key.SetActive(false);
                                StartCoroutine(goInDoorAnimation(other));
                            }
                            else
                            {
                                other.gameObject.GetComponent<Door>().LockedAnimation();
                            }
                        }
                    }
                }
            }

            //For Key Collision
            if (other.gameObject.CompareTag("Key"))
            {
                keyCount++;
                key.SetActive(true);
                Destroy(other.gameObject);
                //TODO- Make ui for key appear
            }


            if (other.gameObject.CompareTag("EndDoor"))
            {
                //Popup
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) && doorDelay)
                {
                    StartCoroutine(goInEndDoorAnimation(other));
                }
            }
            //For Peeking
            if (other.gameObject.CompareTag("PeekColliderLeft"))
            {
                //Popup
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.P)&&!peeking)
                {
                    Camera.GetComponent<Rotate>().rotate(90, true);
                    rotateAnimation = true;
                    peeking = true;
                }
            }
            if (other.gameObject.CompareTag("PeekColliderRight"))
            {
                //Popup
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.P)&&!peeking)
                {
                    Camera.GetComponent<Rotate>().rotate(-90, true);
                    rotateAnimation = true;
                    peeking = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            if (!other.gameObject.GetComponent<Door>().startingDoor)
            {
                //Popup
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        if (other.gameObject.CompareTag("EndDoor"))
        {
            //Popup
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("PeekColliderLeft"))
        {
            //Popup
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("PeekColliderRight"))
        {
            //Popup
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }


    }

    public void checkDeath()
    {
        if (transform.position.y < -45)
        {
            transform.position = spawnPoint;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!rotateAnimation)
        {
            Move();
        }
        if (onLadder)
        {
            rb.velocity = Vector3.zero;
        }
        checkDeath();
    }

    //For camera follow
    private void LateUpdate()
    {
        if (!rotateAnimation&&!peeking)
        {
            Camera.GetComponent<FollowPlayer>().follow();
        }
    }
}