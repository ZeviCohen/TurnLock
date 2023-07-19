using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateKey : MonoBehaviour
{

    public float turnspeed = 0.01f;
    public float spawnY;
    private float direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(upAndDown());
    }

    IEnumerator upAndDown()
    {
        yield return new WaitForSeconds(turnspeed);
        if (direction == 1)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 10, Space.World);
        }
        else if (direction == -1)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 10, Space.World);
        }
        if (transform.position.y >= spawnY + 2)
        {
            direction = -1;
        }
        else if (transform.position.y <= spawnY - 0.5f)
        {
            direction = 1;
        }
        StartCoroutine(upAndDown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
