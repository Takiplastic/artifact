 
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

    //　アイテムの種類
    [SerializeField]
    public Type itemType = Type.HPRecovery;
    //　アイテムの漢字名
    [SerializeField]
    private string kanjiName = "";
    //　アイテムの平仮名名
    [SerializeField]
    private string hiraganaName = "";
    //　アイテム情報
    [SerializeField]
    private string information = "";
    //　アイテムのパラメータ
    [SerializeField]
    private int amount = 0;
    //アイテムのオブジェクト
    [SerializeField]
    private GameObject itemObject = null;
    //最適な位置(武器用)
    [SerializeField]
    private Vector3 preferposition = Vector3.zero;
    //最適な回転(武器用)
    [SerializeField]
    private Vector3 preferrotation = Vector3.zero;
    //アイテムの効果範囲
    [SerializeField]
    private GameObject ItemArea = null;
    //　アイテムの種類を返す
    public Type GetItemType()
    {
        return itemType;
    }
    //　アイテムの名前を返す
    public string GetKanjiName()
    {
        return kanjiName;
    }
    //　アイテムの平仮名の名前を返す
    public string GetHiraganaName()
    {
        return hiraganaName;
    }
    //　アイテム情報を返す
    public string GetInformation()
    {
        return information;
    }
    //　アイテムの強さを返す
    public int GetAmount()
    {
        return amount;
    }
    //アイテムのオブジェクトを返す
    public GameObject GetItemObject()
    {
        return itemObject;
    }
    //最適な位置を返す
    public Vector3 GetPreferPosition()
    {
        return preferposition;
    }

    //最適な回転を返す
    public Vector3 GetPreferRotation()
    {
        return preferrotation;
    }
    //アイテムの効果範囲を返す
    public GameObject GetItemArea()
    {
        return ItemArea;
    }
}

