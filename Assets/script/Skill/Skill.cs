using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
[CreateAssetMenu(fileName ="Skill",menuName ="CreateSkill")]//エディタ上でスキルのデータを作成できるようにする
public class Skill : ScriptableObject
{
   public enum Type
    {
        DirectAttack,
        GetAway,
        Guard,
        Item,
        MagicAttack,
        RecoveryMagic,
        PoisonnouRecoveryMagic,
        NumbnessRecoveryMagic,
        IncreaseAttackPowerMagic,
        IncreaseDefencePowerMagic,
        IncreaseMagicPowerMagic,
        IncreaseMagicStrengthMagic,
        BurstFinish
    }

    public enum PosType 
    {
        Top,
        Center,
        Buttom,
        Weapon
    }

    [SerializeField]
    private Type skillType = Type.DirectAttack;
    [SerializeField]
    private string kanjiName = "";
    [SerializeField]
    private string hiraganaName = "";
    [SerializeField]
    private string information = "";
    //使用者のエフェクト
    [SerializeField]
    private GameObject skillUserEffect = null;
    //魔法を受ける側のエフェクト
    [SerializeField]
    private GameObject skillReceivingSideEffect = null;
    //使用者エフェクトを表示する場所
    [SerializeField]
    private PosType userPosType = PosType.Center;
    //ターゲット側のエフェクトを表示する場所
    [SerializeField]
    private PosType targetPosType = PosType.Center;
    //スキルの効果範囲
    [SerializeField]
    private GameObject skillArea = null;
    //スキルの種類を返す
    public Type GetSkillType()
    {
        return skillType;
    }
    //使用者エフェクトを表示する場所を返す
    public PosType GetUserPosType()
    {
        return userPosType;
    }
    //ターゲットエフェクトを表示する場所を返す
    public PosType GetTargetPosType()
    {
        return targetPosType;
    }
    //スキルの名前を返す
    public string GetKanjiName()
    {
        return kanjiName;
    }
    //　スキルの平仮名の名前を返す
    public string GetHiraganaName()
    {
        return hiraganaName;
    }
    //　スキル情報を返す
    public string GetInformation()
    {
        return information;
    }
    //　使用者のエフェクトを返す
    public GameObject GetSkillUserEffect()
    {
        return skillUserEffect;
    }
    //　スキルを受ける側のエフェクトを返す
    public GameObject GetSkillReceivingSideEffect()
    {
        return skillReceivingSideEffect;
    }
    //スキルの効果範囲を返す
    public GameObject GetSkillArea()
    {
        return skillArea;
    }
    //デバッグ用
    public void Show()
    {
        Debug.Log("SkillType:" + GetSkillType());
        Debug.Log("skillname" + GetKanjiName() + "/" + GetHiraganaName());
    }
}

