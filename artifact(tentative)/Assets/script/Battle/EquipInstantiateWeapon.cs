using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipInstantiateWeapon : MonoBehaviour
{
    //戦闘開始時に武器を持たせる
    [SerializeField]
    private Transform equip;
    // Start is called before the first frame update
    void Start()
    {
        var characterStatus = (AllyStatus)GetComponent<CharacterBattleScript>().GetCharacterStatus();
        GameObject weaponIns;
        if (characterStatus.GetEquipWeapon() != null)
        {
            if (characterStatus.GetEquipWeapon().GetItemObject() != null)
            {
                weaponIns=Instantiate<GameObject>(characterStatus.GetEquipWeapon().GetItemObject(), equip.position, equip.rotation, equip);
                weaponIns.transform.localPosition = characterStatus.GetEquipWeapon().GetPreferPosition();
                weaponIns.transform.Rotate(characterStatus.GetEquipWeapon().GetPreferRotation());

            }
            else
            {
                Debug.LogWarning("装備武器のゲームオブジェクトが設定されていません");
            }
        }
    }
    public Transform GetEquipTransform()
    {
        return equip;
    }

}
