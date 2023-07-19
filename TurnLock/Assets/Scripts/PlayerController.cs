using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    //For camera
    public GameObject Camera;
    private bool peeking = false;

    //Increases gravity
    

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        Physics.gravity *= 2;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(1f,1f);
        rb = GetComponent<Rigidbody>();
    }

    private void Move()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (!onLadder)
        {
            if(Mathf.Abs(rb.velocity.x) < velocityMax || Mathf.Abs(rb.velocity.z) < velocityMax)
            {
                print(rb.velocity);
                rb.AddForce(Vector3.right * horizontalInput * speed);
            }
            if (horizontalInput != 0)
            {
                if (animationDictionary["walk"] == 0)
                {
                    playerAnim.SetTrigger("walk");
                    resetAnimations("walk");
                }
                Camera.GetComponent<Rotate>().peekBack();
            }
            if (horizontalInput==0 && animationDictionary["idle"]==0)
            {
                rb.velocity = Vector3.zero;
                playerAnim.SetTrigger("idle");
                resetAnimations("idle");
            }
            if (horizontalInput>0)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalInput<0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    private void MoveWithPlatform()
    {
        transform.Translate(Vector3.right * Time.deltaTime * movingPlatform.speed * movingPlatform.direction);
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

    IEnumerator goInDoorAnimation(Collider other)
    {
        rotateAnimation = true;
        Door door = other.gameObject.GetComponent<Door>();
        Door connectingDoor = door.connectingDoor.GetComponent<Door>();
        //Doors becomes open
        other.GetComponent<MeshRenderer>().material = door.doorOpen;
        door.connectingDoor.GetComponent<MeshRenderer>().material = door.doorOpen;
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
        yield return new WaitForSeconds(2.5f);
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
        yield return new WaitForSeconds(2.0f);
        //Player moves out of door
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        yield return new WaitForSeconds(0.1f);
        transform.Translate(Vector3.back);
        //Close door
        other.GetComponent<MeshRenderer>().material = door.doorClose;
        door.connectingDoor.GetComponent<MeshRenderer>().material = door.doorClose;
        //Cooldown
        doorDelay = false;
        rotateAnimation = false;
        StartCoroutine(doorCooldown());
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!rotateAnimation)
        {
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

            //For Box collision
            if (collision.gameObject.CompareTag("Box")&& rb.velocity.x <1 && rb.velocity.z<1)
            {
                if (animationDictionary["run"] == 0)
                {
                    playerAnim.SetTrigger("run");
                    resetAnimations("run");
                }
                box = collision.gameObject;
                box.GetComponent<Rigidbody>().AddForce(new Vector3(box.transform.position.x-transform.position.x, 0, box.transform.position.z - transform.position.z)*20);//TODO
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

                ////Checks when the player is at the top of the ladder
                if (transform.position.y >= other.gameObject.transform.position.y + (ladderLength / 2) && !goingDown)
                {
                    transform.position = new Vector3(transform.position.x, other.transform.position.y + (ladderLength / 2) + 3, transform.position.z);
                    transform.Translate(Vector3.forward);
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
                    transform.Translate(Vector3.back * 10);
                    gameObject.GetComponent<Rigidbody>().useGravity = true;
                    onLadder = true;
                    playerAnim.SetTrigger("climb");
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
                if (Input.GetKeyDown(KeyCode.P)&&!peeking)
                {
                    Camera.GetComponent<Rotate>().rotate(90, true);
                    rotateAnimation = true;
                    peeking = true;
                }
            }
            if (other.gameObject.CompareTag("PeekColliderRight"))
            {
                if (Input.GetKeyDown(KeyCode.P)&&!peeking)
                {
                    Camera.GetComponent<Rotate>().rotate(-90, true);
                    rotateAnimation = true;
                    peeking = true;
                }
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

        if (rigidbody != null)
        {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();

            rigidbody.AddForceAtPosition(forceDirection*forceMagnitude,transform.position,ForceMode.Impulse);
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