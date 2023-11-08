using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_velocity : MonoBehaviour
{
    //速度減少係数,大きいほど速度減少が小さい(摩擦が小さい)
    //"重量"ステータスごとに変化させる
    float mu=0.999f;
    Rigidbody rb; 
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        rb.velocity*=mu;
        //一定速度を下回ったら止める
        if(rb.velocity.magnitude<25){
            rb.velocity=Vector3.zero;
        }
    }
}
