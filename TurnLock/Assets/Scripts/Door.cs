using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //To connect the doors
    public GameObject connectingDoor;

    //To determine the rotation for player based on the side
    public float side;

    //For the door open/close
    public Material doorOpen;
    public Material doorClose;

    //For locked door
    public bool unlocked = true;
    public bool hasLock;
    public GameObject Lock;
    public float shakeSpeed = 0.1f;
    private float direction = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void LockedAnimation()
    {
        StartCoroutine(animation());
    }

    IEnumerator animation()
    {
        for (int i=0; i < 20; i++)
        {
            yield return new WaitForSeconds(shakeSpeed);
            if (direction == 1)
            {
                transform.Translate(Vector3.right * Time.deltaTime * 5, Space.World);
            }
            else if (direction == -1)
            {
                transform.Translate(Vector3.left * Time.deltaTime * 5, Space.World);
            }
            if (i % 2 == 0)
            {
                direction = 1;
            }
            if (i % 2 == 1)
            {
                direction = -1;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lock"))
        {
            unlocked = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
