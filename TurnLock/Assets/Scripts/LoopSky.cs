using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSky : MonoBehaviour
{
    public float skyWidth = 96;
    public float speed = 10;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3 (skyWidth, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (transform.position.x > skyWidth) {
            transform.position -= offset;
        }
    }
}
