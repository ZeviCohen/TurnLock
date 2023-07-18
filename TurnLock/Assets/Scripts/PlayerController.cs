using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //For player movement
    public float speed = 5f;
    private float horizontalInput;

    //For ladder
    private bool onLadder;
    public float ladderSpeed = 3f;

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
            transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
        }
    }

    private void MoveWithPlatform()
    {
        transform.Translate(Vector3.right * Time.deltaTime * movingPlatform.speed * movingPlatform.direction);
    }

    private void MoveBox(Vector3 vector)
    {
        //Change animation and update speed
        speed = 3f;
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

        //For door collision
        if (collision.gameObject.CompareTag("Door"))
        {
            //Popup-TODO
            if (Input.GetKeyDown(KeyCode.UpArrow)||Input.GetKeyDown(KeyCode.W))
            {
                Door door = collision.gameObject.GetComponent<Door>();
                transform.position = door.connectingDoor.transform.position;
                transform.rotation = door.connectingDoor.transform.rotation;
                Camera.main.GetComponent<Rotate>().rotate(door.side);
            }
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

    private void OnTriggerEnter(Collider other)
    {
        //For ladder collision
        if (other.gameObject.CompareTag("Ladder"))
        {

            //Popup-TODO
            //Makes the player go up the ladder
            if (Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetKeyDown(KeyCode.W))
            {
                onLadder = true;
                transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z);
                transform.Translate(Vector3.up * Time.deltaTime * ladderSpeed);

            }
            //Makes the player go down the ladder
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                onLadder = true;
                transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z);
                transform.Translate(Vector3.down * Time.deltaTime * ladderSpeed);
            }
            //Checks when the player is at the top of the ladder
            if (transform.position.y >= other.gameObject.transform.position.y + 3)//TODO-Make the number fit pixels
            {
                transform.position = new Vector3(transform.position.x, other.transform.position.y+5, transform.position.z);//TODO-Adjust amount for pixels
                transform.Translate(Vector3.forward * Time.deltaTime * 2);//TODO-adjust for offset of ground
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}