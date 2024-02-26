using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectNumericalDisplayScript : MonoBehaviour
{
    public enum NumberType
    {
        Damage,
        Healing
    }

    //�_���[�W�|�C���g�\���p�v���n�u
    [SerializeField]
    private GameObject damagePointText;
    //�񕜃|�C���g�\���p�v���n�u
    [SerializeField]
    private GameObject healingPointText;
    //�|�C���g�̕\���I�t�Z�b�g�l
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.8f, -0.5f);
    //�|�C���g�̕\�����W����1
    [SerializeField]
    private float min=-1;
    //�|�C���g�̕\�����W����2
    [SerializeField]
    private float max = 1;
    //��1����:�_���[�W���񕜂�,��2����:�ΏۃL����,��3����:�\�����镶��
    public void InstantiatePointText(NumberType numberType, Transform target, int point)
    {
        Debug.Log("SetText");
        float rand1 = Random.Range(min, max); float rand2 = Random.Range(min, max); float rand3 = Random.Range(min, max);
        Vector3 randv = new Vector3(rand1, rand2, rand3);
        var rot = Quaternion.LookRotation(target.position - Camera.main.transform.position);
        if (numberType == NumberType.Damage)
        {
            var pointTextIns = Instantiate<GameObject>(damagePointText, target.position + offset+randv, rot);
            pointTextIns.GetComponent<TextMeshPro>().text = point.ToString();
            Destroy(pointTextIns, 3f);
        }else if (numberType == NumberType.Healing)
        {
            var pointTextIns = Instantiate<GameObject>(healingPointText, target.position + offset+randv, rot);
            pointTextIns.GetComponent<TextMeshPro>().text = point.ToString();
            Destroy(pointTextIns, 3f);
        }
    }
}
