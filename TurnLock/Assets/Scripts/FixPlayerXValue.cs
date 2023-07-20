using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPlayerXValue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            while (other.transform.position.x == 2) {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
