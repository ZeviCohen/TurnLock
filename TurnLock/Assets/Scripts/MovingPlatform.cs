using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    //For moving
    public float speed = 15f;
    public float direction = 1f;

    bool allowChange = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Move()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (allowChange)
            {
                direction *= -1;
                StartCoroutine(change());
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = transform;
        }
        if (collision.gameObject.CompareTag("Box"))
        {
            collision.gameObject.transform.parent = transform;
            collision.gameObject.tag = "BoxOnPlatform";
            collision.gameObject.GetComponent<Box>().platform = transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
        if (collision.gameObject.CompareTag("Box"))
        {
            collision.gameObject.transform.parent = null;
            collision.gameObject.tag = "Box";
            collision.gameObject.GetComponent<Box>().platform = null;
        }
    }

    IEnumerator change()
    {
        allowChange = false;
        yield return new WaitForSeconds(0.5f);
        allowChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
