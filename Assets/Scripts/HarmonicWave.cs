
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmonicWave : MonoBehaviour
{
    public float frequency = 1;
    public float amplitude = 40;
    public float theta = 0;
    public int maxWiggleSpeed = 10;
    private GameObject body;
    // Start is called before the first frame update
    void Start()
    {
        body = transform.parent.gameObject;
        theta = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateWiggleSpeed();
        Debug.Log(body.GetComponent<Boid>().velocity.magnitude);
        float angle = Mathf.Sin(theta) * amplitude;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.down);
        transform.rotation = q * body.transform.rotation;
        theta += Mathf.PI * 2.0f * Time.deltaTime * frequency;
    }

    void CalculateWiggleSpeed()
    {
        Vector3 vel = body.GetComponent<Boid>().velocity;
        float maxVel = body.GetComponent<Boid>().maxSpeed;
        frequency = (vel.magnitude / maxVel) * maxWiggleSpeed;
    }
}