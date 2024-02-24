using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanMoveSimple : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 3.0f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {//W‚Å‘O•û‚ÖˆÚ“®
            rb.velocity = transform.forward * speed;
            Debug.Log("movef");
        }   
        if (Input.GetKey(KeyCode.D))
        {//D‚Å‰E‚ÖˆÚ“®
            rb.velocity = transform.right * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {//A‚Å¶‚ÖˆÚ“®
            rb.velocity = transform.right * -speed;
        }
        if (Input.GetKey(KeyCode.S))
        {//A‚ÅŒã‚ë‚ÖˆÚ“®
            rb.velocity = transform.forward * -speed;
        }
    }
}
