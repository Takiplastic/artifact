using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollUpScript : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    private ScrollManager scrollManager;

    void Start()
    {
        scrollManager = GetComponentInParent<ScrollManager>();
    }

    //　ボタンが選択された時に実行
    public void OnSelect(BaseEventData eventData)
    {
        scrollManager.ScrollUp(transform);

        ScrollManager.PreSelectedButton = gameObject;
    }
    //　ボタンが選択解除された時に実行
    public void OnDeselect(BaseEventData eventData)
    {

    }
}