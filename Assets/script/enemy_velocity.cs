using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_velocity : MonoBehaviour
{
    //���x�����W��,�傫���قǑ��x������������(���C��������)
    //"�d��"�X�e�[�^�X���Ƃɕω�������
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
        //��葬�x�����������~�߂�
        if(rb.velocity.magnitude<25){
            rb.velocity=Vector3.zero;
        }
    }
}
