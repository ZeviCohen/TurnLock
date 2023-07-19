using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    //For moving
    public float speed = 15f;
    public float direction = 1f;

    bool allowChange = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Move()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (allowChange)
            {
                print("hi");
                direction *= -1;
                StartCoroutine(change());
            }
        }
    }

    IEnumerator change()
    {
        allowChange = false;
        yield return new WaitForSeconds(0.5f);
        allowChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
