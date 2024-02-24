using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
[CreateAssetMenu(fileName ="Skill",menuName ="CreateSkill")]//�G�f�B�^��ŃX�L���̃f�[�^���쐬�ł���悤�ɂ���
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
    //�g�p�҂̃G�t�F�N�g
    [SerializeField]
    private GameObject skillUserEffect = null;
    //���@���󂯂鑤�̃G�t�F�N�g
    [SerializeField]
    private GameObject skillReceivingSideEffect = null;
    //�g�p�҃G�t�F�N�g��\������ꏊ
    [SerializeField]
    private PosType userPosType = PosType.Center;
    //�^�[�Q�b�g���̃G�t�F�N�g��\������ꏊ
    [SerializeField]
    private PosType targetPosType = PosType.Center;
    //�X�L���̌��ʔ͈�
    [SerializeField]
    private GameObject skillArea = null;
    //�X�L���̎�ނ�Ԃ�
    public Type GetSkillType()
    {
        return skillType;
    }
    //�g�p�҃G�t�F�N�g��\������ꏊ��Ԃ�
    public PosType GetUserPosType()
    {
        return userPosType;
    }
    //�^�[�Q�b�g�G�t�F�N�g��\������ꏊ��Ԃ�
    public PosType GetTargetPosType()
    {
        return targetPosType;
    }
    //�X�L���̖��O��Ԃ�
    public string GetKanjiName()
    {
        return kanjiName;
    }
    //�@�X�L���̕������̖��O��Ԃ�
    public string GetHiraganaName()
    {
        return hiraganaName;
    }
    //�@�X�L������Ԃ�
    public string GetInformation()
    {
        return information;
    }
    //�@�g�p�҂̃G�t�F�N�g��Ԃ�
    public GameObject GetSkillUserEffect()
    {
        return skillUserEffect;
    }
    //�@�X�L�����󂯂鑤�̃G�t�F�N�g��Ԃ�
    public GameObject GetSkillReceivingSideEffect()
    {
        return skillReceivingSideEffect;
    }
    //�X�L���̌��ʔ͈͂�Ԃ�
    public GameObject GetSkillArea()
    {
        return skillArea;
    }
    //�f�o�b�O�p
    public void Show()
    {
        Debug.Log("SkillType:" + GetSkillType());
        Debug.Log("skillname" + GetKanjiName() + "/" + GetHiraganaName());
    }
}

