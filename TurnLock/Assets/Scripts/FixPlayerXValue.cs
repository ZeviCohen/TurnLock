using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPlayerXValue : MonoBehaviour
{
    public float newXVal = -138;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.transform.position = new Vector3(newXVal, other.transform.position.y, other.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
