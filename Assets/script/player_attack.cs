using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class player_attack : MonoBehaviour
{
    GameObject[] enemies;
    public float power=1.0f;    
    void Start(){
        enemies=GameObject.FindGameObjectsWithTag("enemy");
    }
        //�Փ˂���G�L�[�ŃG�l�~�[�𐁂���΂�
    void Update() {
        if(Input.GetKey(KeyCode.G)){
            int i;
            for(i=0; i<enemies.Length;i++){
                //x���W��z���W�̂ݔ�r
                Vector3 player_xz = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 enemy_xz = new Vector3(enemies[i].transform.position.x, 0, enemies[i].transform.position.z);

                if (Vector3.Distance(player_xz,enemy_xz)<4)
                {
                    //�v���C���[����G�l�~�[�ւ̂̃x�N�g�����擾
                    Vector3 force = enemy_xz-player_xz;
                    force.Normalize();
                    //�G�l�~�[�ɑ��x��n��
                    Debug.Log("F:" + power * force);
                    enemies[i].GetComponent<Rigidbody>().AddForce(power*force,ForceMode.Impulse);
                }
            }
            
        }    
    }  
   
}
