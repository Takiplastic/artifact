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

    //�@�{�^�����I�����ꂽ���Ɏ��s
    public void OnSelect(BaseEventData eventData)
    {
        scrollManager.ScrollUp(transform);

        ScrollManager.PreSelectedButton = gameObject;
    }
    //�@�{�^�����I���������ꂽ���Ɏ��s
    public void OnDeselect(BaseEventData eventData)
    {

    }
}