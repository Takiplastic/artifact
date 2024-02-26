using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraDirection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //カメラの向きにマーカーを表示(キャンバスの向きを変える)
        transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 180f);        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 180f); 
    }
}
