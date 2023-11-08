using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class player_attack : MonoBehaviour
{
    GameObject[] enemies;
    public float power=100.0f;    
    void Start(){
        enemies=GameObject.FindGameObjectsWithTag("enemy");
    }
        //�Փ˂���G�L�[�ŃG�l�~�[�𐁂���΂�
    void Update() {
        if(Input.GetKey(KeyCode.G)){
            int i;
            for(i=0; i<enemies.Length;i++){
                if(Vector3.Distance(transform.position,enemies[i].transform.position)<20)
                {
                    //�v���C���[����G�l�~�[�ւ̂̃x�N�g�����擾
                    Vector3 force = enemies[i].transform.localPosition-transform.localPosition;
                    force.Normalize();
                    //�G�l�~�[�ɑ��x��n��
                    enemies[i].GetComponent<Rigidbody>().AddForce(power*force,ForceMode.VelocityChange);   
                }
            }
            
        }    
    }  
   
}
