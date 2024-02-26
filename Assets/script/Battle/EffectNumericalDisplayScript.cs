using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectNumericalDisplayScript : MonoBehaviour
{
    public enum NumberType
    {
        Damage,
        Healing
    }

    //ダメージポイント表示用プレハブ
    [SerializeField]
    private GameObject damagePointText;
    //回復ポイント表示用プレハブ
    [SerializeField]
    private GameObject healingPointText;
    //ポイントの表示オフセット値
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.8f, -0.5f);
    //ポイントの表示座標乱数1
    [SerializeField]
    private float min=-1;
    //ポイントの表示座標乱数2
    [SerializeField]
    private float max = 1;
    //第1引数:ダメージか回復か,第2引数:対象キャラ,第3引数:表示する文字
    public void InstantiatePointText(NumberType numberType, Transform target, int point)
    {
        Debug.Log("SetText");
        float rand1 = Random.Range(min, max); float rand2 = Random.Range(min, max); float rand3 = Random.Range(min, max);
        Vector3 randv = new Vector3(rand1, rand2, rand3);
        var rot = Quaternion.LookRotation(target.position - Camera.main.transform.position);
        if (numberType == NumberType.Damage)
        {
            var pointTextIns = Instantiate<GameObject>(damagePointText, target.position + offset+randv, rot);
            pointTextIns.GetComponent<TextMeshPro>().text = point.ToString();
            Destroy(pointTextIns, 3f);
        }else if (numberType == NumberType.Healing)
        {
            var pointTextIns = Instantiate<GameObject>(healingPointText, target.position + offset+randv, rot);
            pointTextIns.GetComponent<TextMeshPro>().text = point.ToString();
            Destroy(pointTextIns, 3f);
        }
    }
}
