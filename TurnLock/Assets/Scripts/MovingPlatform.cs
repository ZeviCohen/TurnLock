using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    //For moving
    public float speed = 10f;
    public float direction;


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
            direction *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
