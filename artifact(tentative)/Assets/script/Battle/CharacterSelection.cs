using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour, ISelectHandler,IDeselectHandler,ISubmitHandler,IPointerDownHandler
{
    private GameObject characterMarker;
    // Start is called before the first frame update
    void Start()
    {
        characterMarker = GameObject.Find("Characters" + transform.Find("Text (Legacy)").GetComponent<Text>().text).transform.Find("Marker/Image").gameObject;
        Debug.Log(characterMarker.name);
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            Debug.Log("setactive");
            characterMarker.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        //charactermarkerがnullでなければマーカーを非表示
        if(characterMarker != null)
        {
            characterMarker.SetActive(false);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        characterMarker.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (characterMarker != null)
        {
            characterMarker.SetActive(false);
        }        
    }

    public void OnSubmit(BaseEventData eventData)
    {
        characterMarker.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        characterMarker.SetActive(false);
    }
}
