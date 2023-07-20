using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public Vector3 spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
    }

    public void Move(Vector3 vector, float speed)
    {
        transform.Translate(vector * speed * Time.deltaTime);
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
