using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EnemyStatus", menuName = "CreateEnemyStatus")]
public class EnemyStatus : CharacterStatus
{

    //吹っ飛び耐性
    [SerializeField]
    private int BlowEffect = 0;
    //　倒した時に得られる経験値
    [SerializeField]
    private int gettingExperience = 10;
    //　倒した時に得られるお金
    [SerializeField]
    private int gettingMoney = 10;
    //　落とすアイテムと落とす確率（パーセンテージ表示）
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

    //　落とすアイテムのItemDictionaryを返す
    public ItemDictionary GetDropItemDictionary()
    {
        return dropItemDictionary;
    }

    //　アイテムを落とす確率を返す
    public int GetProbabilityOfDroppingItem(Item item)
    {
        return dropItemDictionary[item];
    }
}