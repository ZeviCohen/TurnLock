using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
