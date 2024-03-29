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
        {//Wで前方へ移動
            rb.velocity = transform.forward * speed;
            Debug.Log("movef");
        }   
        if (Input.GetKey(KeyCode.D))
        {//Dで右へ移動
            rb.velocity = transform.right * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {//Aで左へ移動
            rb.velocity = transform.right * -speed;
        }
        if (Input.GetKey(KeyCode.S))
        {//Aで後ろへ移動
            rb.velocity = transform.forward * -speed;
        }
    }
}
