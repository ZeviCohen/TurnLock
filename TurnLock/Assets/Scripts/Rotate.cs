using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private float currentSide = 0;
    public float turnSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        currentSide = 0;
    }

    public void peekBack()
    {
        transform.eulerAngles = new Vector3(0, currentSide, 0);
    }

    public void rotate(float side, bool peek)
    {
        if (peek)
        {
            if(currentSide == 0 && side == -90)
            {
                print("hi");
                StartCoroutine(rotateAnimation(270,peek));
            }
            else if (currentSide == 270 && side == 90)
            {
                StartCoroutine(rotateAnimation(0,peek));
            }
            else
            {
                StartCoroutine(rotateAnimation(currentSide + side,peek));
            }
        }
        else
        {
            StartCoroutine(rotateAnimation(side,peek));
        }
    }

    IEnumerator rotateAnimation(float side, bool peek)
    {
        float num;
        if (!(currentSide == 0 && side == 270)&&!(currentSide == 270 && side == 0))
        {
            num = Mathf.Abs(side - currentSide);
        }
        else
        {
            num = 90f;
        }
        for (int i = 0; i < num/2; i++)
        {
            yield return new WaitForSeconds(turnSpeed);
            if ((currentSide<side&&!(currentSide == 0 && side == 270)) || (currentSide == 270 && side==0)) {
                transform.Rotate(0,2,0);
            } else
            {
                transform.Rotate(0, -2, 0);
            }
        }
        if (!peek)
        {
            currentSide = side;
        }
        else
        {
            GameObject.FindObjectOfType<PlayerController>().rotateAnimation = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
