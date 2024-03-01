using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
[CreateAssetMenu(fileName ="Magic",menuName ="CreateMagic")]
public class Magic : Skill
{

    public enum MagicAttribute
    {//���@�̑����@��,��,��,������
        Fire,
        Water,
        Thunder,
        Other
    }

    //���@��
    [SerializeField]
    private int magicPower = 0;
    //�g��MP
    [SerializeField]
    private int amountToUseMagicPoints = 0;
    //���@�̑���
    [SerializeField]
    private MagicAttribute magicAttribute = MagicAttribute.Other;
    //���@�͂�Ԃ�
    public int GetMagicPower()
    {
        return magicPower;
    }
    //���@�̑�����Ԃ�
    public MagicAttribute GetMagicAttribute()
    {
        return magicAttribute;
    }
    //�g�pMP��o�^(�g�pMP��0�ȏ�,���̎g�pMP��葝���邱�Ƃ͂Ȃ�)
    public void SetAmoutToUseMagicPoints(int point)
    {
        amountToUseMagicPoints = Mathf.Max(0, Mathf.Min(amountToUseMagicPoints, point));
    }
    //�g�pMP��Ԃ�
    public int GetAmountToUseMagicPoints()
    {
        return amountToUseMagicPoints;
    }
    //�f�o�b�O�p
    public void ShowMagic()
    {
        Show();
        Debug.Log("MagicPower:" + GetMagicPower());
        Debug.Log("Attribute:" + GetMagicAttribute());
        Debug.Log("UseMP:" + GetAmountToUseMagicPoints());
    }
}
