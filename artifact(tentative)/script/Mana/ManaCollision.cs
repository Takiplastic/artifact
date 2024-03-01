using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaCollision : MonoBehaviour
{
    private BattleManager battleManager;
    private BattleStatusScript battleStatusScript;
    [SerializeField]
    private AudioClip audioClip;
    //マナのデータ
    [SerializeField]
    Mana mana=null;
    //効果ポイント表示スクリプト
    private EffectNumericalDisplayScript effectNumericalDisplayScript;
    // Start is called before the first frame update
    void Start()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleStatusScript = GameObject.Find("StatusPanel").GetComponent<BattleStatusScript>();
        effectNumericalDisplayScript = battleManager.GetComponent<EffectNumericalDisplayScript>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {//マナの種類ごとに処理を記述
            CharacterBattleScript characterBattleScript = other.gameObject.GetComponent<CharacterBattleScript>();
            CharacterStatus characterStatus = characterBattleScript.GetCharacterStatus();
            if (mana.GetManaType() == Mana.Type.Damage)
            {
                AudioSource.PlayClipAtPoint(audioClip, other.transform.position);
                int damage = characterStatus.GetMaxHp() * mana.GetAmount() / 100;
                characterBattleScript.SetHp(characterBattleScript.GetHp() - damage);
                characterBattleScript.SetBurst(mana.GetBurstamount());
                battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.HP, characterBattleScript.GetHp());
                battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.Burstgage, characterBattleScript.GetBurst());
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, other.gameObject.transform.Find("CenterPos").transform, damage);
                Destroy(this.gameObject);
                characterBattleScript.ChangeBurstMode(1);
            }
            else if(mana.GetManaType() == Mana.Type.HealingHp)
            {
                AudioSource.PlayClipAtPoint(audioClip,other.transform.position);
                int recoverypoint = characterStatus.GetMaxHp() * mana.GetAmount() / 100;
                characterBattleScript.SetHp(characterBattleScript.GetHp() + recoverypoint);
                characterBattleScript.SetBurst(mana.GetBurstamount());
                battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.HP, characterBattleScript.GetHp());
                battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.Burstgage, characterBattleScript.GetBurst());
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing, other.gameObject.transform.Find("CenterPos").transform, recoverypoint);
                Destroy(this.gameObject);
                characterBattleScript.ChangeBurstMode(1);
            }
            else if (mana.GetManaType() == Mana.Type.HealingMp)
            {
                AudioSource.PlayClipAtPoint(audioClip, other.transform.position);
                int recoverypoint = characterStatus.GetMaxMp() * mana.GetAmount() / 100;
                characterBattleScript.SetMp(characterBattleScript.GetMp() + recoverypoint);
                characterBattleScript.SetBurst(mana.GetBurstamount());
                battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.MP, characterBattleScript.GetMp());
                battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.Burstgage, characterBattleScript.GetBurst());
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing, other.gameObject.transform.Find("CenterPos").transform, recoverypoint);
                Destroy(this.gameObject);
                characterBattleScript.ChangeBurstMode(1);
            }
        }
    }
}
