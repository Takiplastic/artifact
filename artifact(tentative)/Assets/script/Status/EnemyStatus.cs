using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EnemyStatus", menuName = "CreateEnemyStatus")]
public class EnemyStatus : CharacterStatus
{

    //������ёϐ�
    [SerializeField]
    private int BlowEffect = 0;
    //�@�|�������ɓ�����o���l
    [SerializeField]
    private int gettingExperience = 10;
    //�@�|�������ɓ����邨��
    [SerializeField]
    private int gettingMoney = 10;
    //�@���Ƃ��A�C�e���Ɨ��Ƃ��m���i�p�[�Z���e�[�W�\���j
    [SerializeField]
    private ItemDictionary dropItemDictionary = null;

    public int GetBlowEffect()
    {
        return BlowEffect;
    }
    public void SetBlowEffect(int blow)
    {
        BlowEffect = Mathf.Max(0, blow);
    }
    public int GetGettingExperience()
    {
        return gettingExperience;
    }

    public int GetGettingMoney()
    {
        return gettingMoney;
    }

    //�@���Ƃ��A�C�e����ItemDictionary��Ԃ�
    public ItemDictionary GetDropItemDictionary()
    {
        return dropItemDictionary;
    }

    //�@�A�C�e���𗎂Ƃ��m����Ԃ�
    public int GetProbabilityOfDroppingItem(Item item)
    {
        return dropItemDictionary[item];
    }
}