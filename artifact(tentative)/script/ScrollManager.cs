using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    //アイテムボタン表示用コンテンツ
    private Transform content;
    //スクロール中かどうか
    private bool changeScrollValue;
    //スクロールの目的の値
    private float destinationValue;
    //スクロールスピード
    [SerializeField]
    private float scrollSpeed = 1000f;
    //一回でスクロールする値
    [SerializeField]
    private float scrollValue = 415f;
    //アイテム一覧のスクロールのデフォルト値
    private Vector3 defaultScrollValue;

    //前に選択していたボタン
    public static GameObject PreSelectedButton { get; set; }

    private void Awake()
    {
        content = transform;
        defaultScrollValue = content.transform.position;
    }

    private void Update()
    {
        if (!changeScrollValue)
        {
            return;
        }

        //徐々に目的の値に変化させる
        content.transform.localPosition = new Vector3(content.transform.localPosition.x, Mathf.MoveTowards(content.transform.localPosition.y, destinationValue, scrollSpeed * Time.deltaTime), content.transform.localPosition.z);

        // ある程度移動したら目的地に設定
        if (Mathf.Abs(content.transform.localPosition.y - destinationValue) < 0.2f)
        {
            changeScrollValue = false;
            content.transform.localPosition = new Vector3(0f, destinationValue, 0f);
        }
    }

    //下にスクロール
    public void ScrollDown(Transform button)
    {
        if (changeScrollValue)
        {
            changeScrollValue = false;
            content.transform.localPosition = new Vector3(content.transform.localPosition.x, destinationValue, content.transform.localPosition.z);
        }

        if (ScrollManager.PreSelectedButton != null && button.position.y > ScrollManager.PreSelectedButton.transform.position.y)
        {
            destinationValue = content.transform.localPosition.y - scrollValue;
            changeScrollValue = true;
        }
    }
    //　上にスクロール
    public void ScrollUp(Transform button)
    {
        if (changeScrollValue)
        {
            content.transform.localPosition = new Vector3(content.transform.localPosition.x, destinationValue, content.transform.localPosition.z);
            changeScrollValue = false;
        }

        if (ScrollManager.PreSelectedButton != null
            && button.position.y < ScrollManager.PreSelectedButton.transform.position.y)
        {
            destinationValue = content.transform.localPosition.y + scrollValue;
            changeScrollValue = true;
        }
    }

    public void Reset()
    {
        PreSelectedButton = null;
        transform.position = defaultScrollValue;
    }
}
