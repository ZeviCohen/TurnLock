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

    //For moving platform
    private MovingPlatform movingPlatform;

    //For box
    private GameObject box;

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

    }

    private void OnCollisionEnter(Collision collision)
    {

        //For box collision
        if (collision.gameObject.CompareTag("Box"))
        {
            box = collision.gameObject;
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

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //For ladder collision
        if (collision.gameObject.CompareTag("Ladder"))
        {
            //Popup-TODO
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}