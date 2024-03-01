using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRegistTarget : MonoBehaviour
{

    private GameObject charaobj;
    private CharacterBattleScript characterBattleScript;
    private string skilltargettag;
    void Start()
    {
        charaobj = this.gameObject.transform.parent.parent.parent.gameObject;
        characterBattleScript = charaobj.GetComponent<CharacterBattleScript>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == skilltargettag)
        {
            characterBattleScript.SettargetObjList(other.gameObject);
            if (skilltargettag == "enemy")
            {//敵の場合はマーカーを表示
                other.transform.Find("Marker/Image").gameObject.SetActive(true);
            }
        }
        
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == skilltargettag)
        {
            characterBattleScript.DeletetargetObjList(other.gameObject);
            if (skilltargettag == "enemy")
            {//敵の場合はマーカーを非表示
                other.transform.Find("Marker/Image").gameObject.SetActive(false);
            }
        }
            
    }
    public void OnDestroy()
    {
        if(charaobj.tag=="Player")
        {//味方のスキルエリアが消失したらターゲットのマーカーも消す
            Debug.Log(characterBattleScript.GettargetObjList().Count);
            List<GameObject> targetList = characterBattleScript.GettargetObjList();
            foreach(var child in targetList)
            {
                child.transform.Find("Marker/Image").gameObject.SetActive(false);
                Debug.Log("DeleteImage");
            }
        }
        //targetObjListを初期化
        characterBattleScript.GettargetObjList().Clear();
    }
    public void SetSkillTargetTag(string str)
    {
        skilltargettag = str;
    }
}
