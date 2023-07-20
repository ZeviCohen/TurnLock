using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public Vector3 spawnPoint;
    public Transform platform = null;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
    }

    public void Move(Vector3 vector, float speed)
    {
        transform.Translate(vector * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BoxOnPlatform"))
        {
            transform.parent = collision.gameObject.GetComponent<Box>().platform;
        }
        if (collision.gameObject.CompareTag("Player"))
        {

        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("BoxOnPlatform"))
        {
            transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            transform.position = spawnPoint;
        }
    }
}
