using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Mana", menuName = "CreateMana")]//�G�f�B�^��ŃX�L���̃f�[�^���쐬�ł���悤�ɂ���
public class Mana : ScriptableObject
{
    public enum Type 
    {
        Damage,
        HealingHp,
        HealingMp,
        Power,
        StrikingStrength,
        MagicPower,
        MagicStrength,
        Poison,
        Numbness
    }

    [SerializeField]
    private Type type = Type.Damage;
    //�_���[�W,HP,MP�ւ̌��ʗ�(�ő�ʂ̉��p�[�Z���g��)
    [SerializeField]
    private int amount=0;
    //�o�[�X�g�Q�[�W�̑�����
    [SerializeField]
    private int burstamount=0;
    //�}�i�̃v���n�u
    [SerializeField]
    private GameObject manaprefab=null;
    public Type GetManaType()
    {
        return type;
    }
    public int GetAmount()
    {
        return amount;
    }
    public int GetBurstamount()
    {
        return burstamount;
    }
    public GameObject GetManaPrefab()
    {
        return manaprefab;
    }


}
