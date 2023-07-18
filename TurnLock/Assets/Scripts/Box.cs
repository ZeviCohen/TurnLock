using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Move(Vector3 vector, float speed)
    {
        transform.Translate(vector * speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
