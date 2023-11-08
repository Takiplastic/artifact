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
        //衝突かつGキーでエネミーを吹き飛ばす
    void Update() {
        if(Input.GetKey(KeyCode.G)){
            int i;
            for(i=0; i<enemies.Length;i++){
                if(Vector3.Distance(transform.position,enemies[i].transform.position)<20)
                {
                    //プレイヤーからエネミーへののベクトルを取得
                    Vector3 force = enemies[i].transform.localPosition-transform.localPosition;
                    force.Normalize();
                    //エネミーに速度を渡す
                    enemies[i].GetComponent<Rigidbody>().AddForce(power*force,ForceMode.VelocityChange);   
                }
            }
            
        }    
    }  
   
}
