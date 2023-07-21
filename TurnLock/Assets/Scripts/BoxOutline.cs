using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOutline : MonoBehaviour
{
    public GameObject reward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            if (other.gameObject.transform.position.x<transform.position.x+3 && other.gameObject.transform.position.z < transform.position.z + 3 && other.gameObject.transform.position.x > transform.position.x - 3 && other.gameObject.transform.position.z > transform.position.z - 3 && other.gameObject.transform.position.y < transform.position.y + 3 && other.gameObject.transform.position.y > transform.position.y - 3)
            {
                reward.SetActive(true);
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
