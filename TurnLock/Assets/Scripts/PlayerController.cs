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
    public float ladderSpeed = 10f;
    public float ladderLength = 38.61533f;

    //For moving platform
    public MovingPlatform movingPlatform = null;

    //For box
    public Box box = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (!onLadder)
        {
            transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime*-1);
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

    private void OnCollisionEnter(Collision collision)
    {

        //For box collision
        if (collision.gameObject.CompareTag("Box"))
        {
            box = collision.gameObject.GetComponent<Box>();
            if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A))
            {
                MoveBox(Vector3.left);
            } else if (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D))
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

    private void OnTriggerStay(Collider other)
    {
        //For ladder collision
        if (other.gameObject.CompareTag("Ladder"))
        {

            //Popup-TODO
            //Makes the player go up the ladder
            if (Input.GetKey(KeyCode.UpArrow)|| Input.GetKey(KeyCode.W))
            {
                onLadder = true;
                transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, transform.position.z);
                transform.Translate(Vector3.up * Time.deltaTime * ladderSpeed, Space.World);
                gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
            ////Checks when the player is at the top of the ladder
            if (transform.position.y >= other.gameObject.transform.position.y + (ladderLength / 2))//TODO-Make the number fit pixels
            {
                transform.position = new Vector3(transform.position.x, other.transform.position.y + (ladderLength / 2)+2, transform.position.z);//TODO-Adjust amount for pixels
                transform.Translate(Vector3.forward * 2);//TODO-adjust for offset of ground
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                onLadder = false;
            }
        }
        //For door collision
        if (other.gameObject.CompareTag("Door"))
        {
            //Popup-TODO
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                Door door = other.gameObject.GetComponent<Door>();
                transform.position = new Vector3(door.connectingDoor.transform.position.x,transform.position.y,transform.position.z);
                transform.rotation = door.connectingDoor.transform.rotation;
                transform.Rotate(new Vector3(0,0,180));
                Camera.main.GetComponent<Rotate>().rotate(door.side);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}