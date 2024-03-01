using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleEnemyStatus : MonoBehaviour
{
    //�G�l�~�[�̃X�e�[�^�X�\��
    //�e�G�l�~�[�̃v���n�u�ɂ��̃X�N���v�g���A�^�b�`���ʂɊǗ�

    CharacterStatus enemystatus;
    Transform enemyPanelTransform;
    public enum Status 
    {
        HP,
        MP,
    }

    void Start()
    {
        DisplayStatus();
    }

    private void Update()
    {
        
    }
    public void DisplayStatus()
    {
        enemystatus = GetComponent<CharacterBattleScript>().GetCharacterStatus();
        enemyPanelTransform = transform.Find("StatusPanel");
        enemyPanelTransform.Find("HP/Slider").GetComponent<Slider>().value = (float)enemystatus.GetHp() / enemystatus.GetMaxHp();
    }
    public void UpdateEnemyStatus(CharacterStatus characterStatus, Status status, int destinationValue)
    {
        if (status == Status.HP)
        {
            enemyPanelTransform.Find("HP/Slider").GetComponent<Slider>().value = (float)destinationValue / enemystatus.GetMaxHp();
        }
    }
}
