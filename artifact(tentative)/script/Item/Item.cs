 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{
    public enum Type
    {
        HPRecovery,
        MPRecovery,
        PoisonRecovery,
        NumbnessRecovery,
        WeaponAll,
        WeaponUnityChan,
        WeaponYuji,
        ArmorAll,
        ArmorUnityChan,
        ArmorYuji,
        Valuables
    }

    //�@�A�C�e���̎��
    [SerializeField]
    public Type itemType = Type.HPRecovery;
    //�@�A�C�e���̊�����
    [SerializeField]
    private string kanjiName = "";
    //�@�A�C�e���̕�������
    [SerializeField]
    private string hiraganaName = "";
    //�@�A�C�e�����
    [SerializeField]
    private string information = "";
    //�@�A�C�e���̃p�����[�^
    [SerializeField]
    private int amount = 0;
    //�A�C�e���̃I�u�W�F�N�g
    [SerializeField]
    private GameObject itemObject = null;
    //�œK�Ȉʒu(����p)
    [SerializeField]
    private Vector3 preferposition = Vector3.zero;
    //�œK�ȉ�](����p)
    [SerializeField]
    private Vector3 preferrotation = Vector3.zero;
    //�A�C�e���̌��ʔ͈�
    [SerializeField]
    private GameObject ItemArea = null;
    //�@�A�C�e���̎�ނ�Ԃ�
    public Type GetItemType()
    {
        return itemType;
    }
    //�@�A�C�e���̖��O��Ԃ�
    public string GetKanjiName()
    {
        return kanjiName;
    }
    //�@�A�C�e���̕������̖��O��Ԃ�
    public string GetHiraganaName()
    {
        return hiraganaName;
    }
    //�@�A�C�e������Ԃ�
    public string GetInformation()
    {
        return information;
    }
    //�@�A�C�e���̋�����Ԃ�
    public int GetAmount()
    {
        return amount;
    }
    //�A�C�e���̃I�u�W�F�N�g��Ԃ�
    public GameObject GetItemObject()
    {
        return itemObject;
    }
    //�œK�Ȉʒu��Ԃ�
    public Vector3 GetPreferPosition()
    {
        return preferposition;
    }

    //�œK�ȉ�]��Ԃ�
    public Vector3 GetPreferRotation()
    {
        return preferrotation;
    }
    //�A�C�e���̌��ʔ͈͂�Ԃ�
    public GameObject GetItemArea()
    {
        return ItemArea;
    }
}

