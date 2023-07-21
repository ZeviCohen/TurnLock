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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            if (collision.gameObject.transform.position.x<transform.position.x+5 && collision.gameObject.transform.position.z < transform.position.z + 5 && collision.gameObject.transform.position.x > transform.position.x - 5 && collision.gameObject.transform.position.z > transform.position.z - 5)
            {
                reward.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
