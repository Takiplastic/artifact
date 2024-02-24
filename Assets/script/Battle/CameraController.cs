using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //バーチャルカメラ
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera=null;
    //俯瞰カメラ
    [SerializeField]
    Transform OverView = null;
    // Start is called before the first frame update
    void Start()
    {
        ResetCamera();
    }
    
    //カメラを操作中のキャラの後ろに追従+キャラを移動可能にする
    public void SetCameraToCharaBack(GameObject character)
    {
        //キャラが移動できるようにスクリプトを有効化
        character.GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
        //カメラをキャラの背後の位置に追従
        cinemachineVirtualCamera.Follow = character.transform.Find("PlayerCameraRoot");
    }
    //カメラを操作中のキャラの前に移動
    public void SetCameraToCharaFront(GameObject character)
    {
        //カメラの初期位置を前にする
        cinemachineVirtualCamera.Follow = character.transform.Find("PlayerCameraRootFront");
    }
    //カメラを俯瞰に戻す+移動を無効にする(あれば)
    public void ResetCamera(GameObject character=null)
    {
        if (character != null)
        {
            character.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        }
        cinemachineVirtualCamera.Follow = OverView;
    }

}
