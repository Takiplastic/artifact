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
    {//魔法の属性　火,水,雷,無属性
        Fire,
        Water,
        Thunder,
        Other
    }

    //魔法力
    [SerializeField]
    private int magicPower = 0;
    //使うMP
    [SerializeField]
    private int amountToUseMagicPoints = 0;
    //魔法の属性
    [SerializeField]
    private MagicAttribute magicAttribute = MagicAttribute.Other;
    //魔法力を返す
    public int GetMagicPower()
    {
        return magicPower;
    }
    //魔法の属性を返す
    public MagicAttribute GetMagicAttribute()
    {
        return magicAttribute;
    }
    //使用MPを登録(使用MPは0以上,元の使用MPより増えることはない)
    public void SetAmoutToUseMagicPoints(int point)
    {
        amountToUseMagicPoints = Mathf.Max(0, Mathf.Min(amountToUseMagicPoints, point));
    }
    //使用MPを返す
    public int GetAmountToUseMagicPoints()
    {
        return amountToUseMagicPoints;
    }
    //デバッグ用
    public void ShowMagic()
    {
        Show();
        Debug.Log("MagicPower:" + GetMagicPower());
        Debug.Log("Attribute:" + GetMagicAttribute());
        Debug.Log("UseMP:" + GetAmountToUseMagicPoints());
    }
}
