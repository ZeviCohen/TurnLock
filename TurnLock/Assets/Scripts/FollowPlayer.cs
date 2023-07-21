using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // camera will follow this object
    public Transform target;
    // offset between camera and target
    public Vector3 Offset;
    // change this value to get desired smoothness
    public float SmoothTime = 0.3f;
    // This value will change at the runtime depending on target movement. Initialize with zero vector.
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {

    }

    public void follow()
    {
        if (transform.rotation.eulerAngles.y == 90)
        {
            Offset = new Vector3(-50,-40,0);
        }
        else if (transform.rotation.eulerAngles.y == 270)
        {
            Offset = new Vector3(50,-40,0);
        }
        else if (transform.rotation.eulerAngles.y == 0)
        {
            Offset = new Vector3(0,-40,-50);
        }
        else if (transform.rotation.eulerAngles.y == 180)
        {
            Offset = new Vector3(0,-40,50);
        }
        // update position
        Vector3 targetPosition = target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
