using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    public GameObject ballPrehab;

    public bool ifThrownBall = false;
    public GameObject ball;
    public int throwForce = 1000;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Throw();
        }
    }

    void Throw()
    {
        if (!ifThrownBall)
        {
            GameObject newBall = Instantiate(ballPrehab, transform.position, transform.rotation);
            newBall.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce);
            ball = newBall;
            ifThrownBall = true;
        }
    }
}
