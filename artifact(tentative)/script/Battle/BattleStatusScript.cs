using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatusScript : MonoBehaviour
{
    //キャラステータスの表示を行う
    public enum Status { 
        HP,
        MP,
        Burstgage,
    }
    //パーティーステータス
    [SerializeField]
    private PartyStatus partyStatus = null;
    //キャラクターステータス表示transformリスト
    private Dictionary<CharacterStatus, Transform> characterStatusDictionary = new Dictionary<CharacterStatus, Transform>();

    void Start()
    {
        DisplayStatus();
    }
    //ステータスデータの表示
    public void DisplayStatus()
    {
        CharacterStatus member;
        Transform characterPanelTransform;
        for (int i = 0; i < 3; i++)
        {
            characterPanelTransform = transform.Find("CharacterPanel" + i);
            if (i < partyStatus.GetAllyStatus().Count)
            {
                member = partyStatus.GetAllyStatus()[i];
                member.Show();
                characterPanelTransform.Find("column1/CharacterName").GetComponent<Text>().text = member.GetCharacterName();
                characterPanelTransform.Find("column1/low2/BurstgageSlider").GetComponent<Slider>().value = (float)member.GetBurst() / member.GetMaxBurst();
                characterPanelTransform.Find("column1/low2/BurstgageSlider/Fill Area/BurstText").GetComponent<Text>().text = member.GetBurst().ToString();
                characterPanelTransform.Find("column2/low1/HPSlider").GetComponent<Slider>().value = (float)member.GetHp() / member.GetMaxHp();
                characterPanelTransform.Find("column2/low1/HPSlider/Fill Area/HPText").GetComponent<Text>().text = member.GetHp().ToString();
                characterPanelTransform.Find("column2/low2/MPSlider").GetComponent<Slider>().value = (float)member.GetMp() / member.GetMaxMp();
                characterPanelTransform.Find("column2/low2/MPSlider/Fill Area/MPText").GetComponent<Text>().text = member.GetMp().ToString();
                characterStatusDictionary.Add(member, characterPanelTransform);
            }

        }
    }
    //ステータスデータの更新
    public void UpdateStatus(CharacterStatus characterStatus, Status status, int destinationValue)
    {
        if (status == Status.HP)
        {
            Debug.Log(characterStatusDictionary[characterStatus].name);
            characterStatusDictionary[characterStatus].Find("column2/low1/HPSlider").GetComponent<Slider>().value = (float)destinationValue / characterStatus.GetMaxHp();
            characterStatusDictionary[characterStatus].Find("column2/low1/HPSlider/Fill Area/HPText").GetComponent<Text>().text = destinationValue.ToString();
        }
        else if (status == Status.MP)
        {
            Debug.Log(characterStatusDictionary[characterStatus].name);
            characterStatusDictionary[characterStatus].Find("column2/low2/MPSlider").GetComponent<Slider>().value = (float)destinationValue / characterStatus.GetMaxMp();
            characterStatusDictionary[characterStatus].Find("column2/low2/MPSlider/Fill Area/MPText").GetComponent<Text>().text = destinationValue.ToString();
        }else if (status == Status.Burstgage)
        {
            Debug.Log(characterStatusDictionary[characterStatus].name);
            characterStatusDictionary[characterStatus].Find("column1/low2/BurstgageSlider").GetComponent<Slider>().value = (float)destinationValue / characterStatus.GetMaxBurst();
            characterStatusDictionary[characterStatus].Find("column1/low2/BurstgageSlider/Fill Area/BurstText").GetComponent<Text>().text = destinationValue.ToString();
            //バーストゲージの値が100以上ならバーストの色を赤に変更
            if (destinationValue >= 100)
            {
                characterStatusDictionary[characterStatus].Find("column1/low2/BurstgageSlider/Fill Area/BurstText").GetComponent<Text>().color = Color.red;
            }
            else
            {
                characterStatusDictionary[characterStatus].Find("column1/low2/BurstgageSlider/Fill Area/BurstText").GetComponent<Text>().color = Color.black;
            }
        }
    }
}
