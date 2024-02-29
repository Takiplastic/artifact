using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityChanCommandScript : MonoBehaviour
{
    //　コマンド用UI
    [SerializeField]
    private GameObject commandUI = null;
    
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
        //　コマンドUIの表示・非表示の切り替え
        if (Input.GetButtonDown("Menu"))
        {
            //　コマンドUIのオン・オフ
            commandUI.SetActive(!commandUI.activeSelf);
        }
    }
    public void ExitCommand()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

}