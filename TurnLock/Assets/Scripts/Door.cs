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

    // Start is called before the first frame update
    void Start()
    {
        
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
