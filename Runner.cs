using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour {
    private bool initialised = false;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    public float headDistance;
    public float leftDistance;
    public float rightDistance;
    private RaycastHit2D headSensor;
    private RaycastHit2D leftSensor;
    private RaycastHit2D rightSensor;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initialised = true;
    }

    void FixedUpdate()
    {
        if (initialised == true)
        {   
            headSensor = Physics2D.Raycast(transform.position + 0.5f * transform.up, transform.up, 3f, 9);
            if (headSensor.collider != null)
            {
                headDistance = headSensor.distance;
            }
            else
            {
                headDistance = 3f;
            }
            leftSensor = Physics2D.Raycast(transform.position + Quaternion.AngleAxis(60, transform.forward) * transform.up * 0.25f, Quaternion.AngleAxis(60, transform.forward) * transform.up, 3f, 9);
            if (leftSensor.collider != null)
            {
                leftDistance = leftSensor.distance;
            }
            else
            {
                leftDistance = 3f;
            }
            rightSensor = Physics2D.Raycast(transform.position + Quaternion.AngleAxis(-60, transform.forward) * transform.up * 0.25f, Quaternion.AngleAxis(-60, transform.forward) * transform.up, 3f, 9);
            if (rightSensor.collider != null)
            {
                rightDistance = rightSensor.distance;
            }
            else
            {
                rightDistance = 3f;
            }
            float[] inputs = { headDistance, leftDistance, rightDistance };
            float[] outputs = net.FeedForward(inputs);
            net.SetFitness(Vector3.Distance(transform.position, Vector3.zero));

            rBody.velocity = outputs[0] * transform.up * 5f;
            rBody.angularVelocity = outputs[1] * 1000f;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        rBody.velocity = Vector2.zero;
        rBody.angularVelocity = 0f;
        enabled = false;
    }
}
