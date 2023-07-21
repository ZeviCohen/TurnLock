using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public Vector3 spawnPoint;
    public Transform platform = null;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 vector, float speed)
    {
        transform.Translate(vector * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BoxOnPlatform"))
        {
            transform.parent = collision.gameObject.transform;
        }
        if (collision.gameObject.CompareTag("Player")) {
            if (gameObject.CompareTag("BoxOnPlatform")) {
                collision.transform.parent = platform;
            }

        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (rb.velocity.x<0.1 && rb.velocity.z < 0.1)
            {
                rb.AddForce(new Vector3(transform.position.x-collision.gameObject.transform.position.x, 0 , transform.position.z - collision.gameObject.transform.position.z));
            }

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("BoxOnPlatform"))
        {
            transform.parent = null;
        }
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.transform.parent = null;
        }
    }

    IEnumerator respawn()
    {
        yield return new WaitForSeconds(1.0f);
        transform.position = spawnPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            StartCoroutine(respawn());
        }
    }
}
