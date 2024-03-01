using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Mana", menuName = "CreateMana")]//エディタ上でスキルのデータを作成できるようにする
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
    //ダメージ,HP,MPへの効果量(最大量の何パーセントか)
    [SerializeField]
    private int amount=0;
    //バーストゲージの増加量
    [SerializeField]
    private int burstamount=0;
    //マナのプレハブ
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
