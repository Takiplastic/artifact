using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityChanCommandScript : MonoBehaviour
{
    //�@�R�}���h�pUI
    [SerializeField]
    private GameObject commandUI = null;
    
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
        //�@�R�}���hUI�̕\���E��\���̐؂�ւ�
        if (Input.GetButtonDown("Menu"))
        {
            //�@�R�}���hUI�̃I���E�I�t
            commandUI.SetActive(!commandUI.activeSelf);
        }
    }
    public void ExitCommand()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

}