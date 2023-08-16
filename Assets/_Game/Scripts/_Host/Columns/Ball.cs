using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Ball : MonoBehaviour
{
    public float speed = 2;
    private Rigidbody rb;
    public bool elevate;
    public Renderer rend;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null)
            AddForce();
    }

    public void AddForce()
    {
        Vector3 movement = new Vector3(UnityEngine.Random.Range(-20, 20), 0.0f, UnityEngine.Random.Range(-20, 20));
        rb.AddForce(movement * speed);
    }

    public void Elevate()
    {
        elevate = true;
        Invoke("EndElevate", 4f);
    }

    void EndElevate()
    {
        elevate = false;
    }

    public void FixedUpdate()
    {
        if(elevate)
        {
            Vector3 movement = new Vector3(0, 10.8f, 0);
            rb.AddForce(movement * speed);
        }
    }
}
