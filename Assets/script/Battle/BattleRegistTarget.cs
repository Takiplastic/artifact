using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRegistTarget : MonoBehaviour
{

    Å@private CharacterBattleScript characterBattleScript;
    private string skilltargettag;
    void Start()
    {
        characterBattleScript = this.gameObject.transform.parent.parent.parent.GetComponent<CharacterBattleScript>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == skilltargettag)
            characterBattleScript.SettargetObjList(other.gameObject);
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == skilltargettag)
            characterBattleScript.DeletetargetObjList(other.gameObject);
    }
    public void SetSkillTargetTag(string str)
    {
        skilltargettag = str;
    }
}
