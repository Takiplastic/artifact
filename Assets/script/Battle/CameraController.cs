using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //�o�[�`�����J����
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera=null;
    //���ՃJ����
    [SerializeField]
    Transform OverView = null;
    // Start is called before the first frame update
    void Start()
    {
        ResetCamera();
    }
    
    //�J�����𑀍쒆�̃L�����̌��ɒǏ]+�L�������ړ��\�ɂ���
    public void SetCameraToCharaBack(GameObject character)
    {
        //�L�������ړ��ł���悤�ɃX�N���v�g��L����
        character.GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
        //�J�������L�����̔w��̈ʒu�ɒǏ]
        cinemachineVirtualCamera.Follow = character.transform.Find("PlayerCameraRoot");
    }
    //�J�����𑀍쒆�̃L�����̑O�Ɉړ�
    public void SetCameraToCharaFront(GameObject character)
    {
        //�J�����̏����ʒu��O�ɂ���
        cinemachineVirtualCamera.Follow = character.transform.Find("PlayerCameraRootFront");
    }
    //�J��������Ղɖ߂�+�ړ��𖳌��ɂ���(�����)
    public void ResetCamera(GameObject character=null)
    {
        if (character != null)
        {
            character.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        }
        cinemachineVirtualCamera.Follow = OverView;
    }

}
