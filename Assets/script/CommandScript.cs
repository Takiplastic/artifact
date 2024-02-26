 using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class CommandScript : MonoBehaviour
{

    public enum CommandMode
    {
        CommandPanel,
        StatusPanelSelectCharacter,
        StatusPanel,
    }

    private CommandMode currentCommand;
    //�@���j�e�B�����R�}���h�X�N���v�g
    private UnityChanCommandScript unityChanCommandScript;

    //�@�ŏ��ɑI������Button��Transform
    private GameObject firstSelectButton;

    //�@�R�}���h�p�l��
    private GameObject commandPanel;
    //�@�X�e�[�^�X�\���p�l��
    private GameObject statusPanel;
    //�@�L�����N�^�[�I���p�l��
    private GameObject selectCharacterPanel;

    //�@�R�}���h�p�l����CanvasGroup
    private CanvasGroup commandPanelCanvasGroup;
    //�@�L�����N�^�[�I���p�l����CanvasGroup
    private CanvasGroup selectCharacterPanelCanvasGroup;

    //�@�L�����N�^�[��
    private Text characterNameText;
    //�@�X�e�[�^�X�^�C�g���e�L�X�g
    private Text statusTitleText;
    //�@�X�e�[�^�X�p�����[�^�e�L�X�g1
    private Text statusParam1Text;
    //�@�X�e�[�^�X�p�����[�^�e�L�X�g2
    private Text statusParam2Text;
    //�@�p�[�e�B�[�X�e�[�^�X
    [SerializeField]
    private PartyStatus partyStatus = null;

    //�@�L�����N�^�[�I���̃{�^���̃v���n�u
    [SerializeField]
    private GameObject characterPanelButtonPrefab = null;

    //�@�Ō�ɑI�����Ă����Q�[���I�u�W�F�N�g���X�^�b�N
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();
   
    void Awake()
    {
        //�@�R�}���h��ʂ��J�����������Ă���UnityChanCommandScript���擾
        unityChanCommandScript = GameObject.FindWithTag("Player").GetComponent<UnityChanCommandScript>();
        //�@���݂̃R�}���h��������
        currentCommand = CommandMode.CommandPanel;
        //�@�K�w��H���Ă��擾
        firstSelectButton = transform.Find("CommandPanel/StatusButton").gameObject;
        //�@�p�l���n
        commandPanel = transform.Find("CommandPanel").gameObject;
        statusPanel = transform.Find("StatusPanel").gameObject;
        selectCharacterPanel = transform.Find("SelectCharacterPanel").gameObject;
        //�@CanvasGroup
        commandPanelCanvasGroup = commandPanel.GetComponent<CanvasGroup>();
        selectCharacterPanelCanvasGroup = selectCharacterPanel.GetComponent<CanvasGroup>();
        //�@�X�e�[�^�X�p�e�L�X�g
        characterNameText = statusPanel.transform.Find("CharacterNamePanel/Text").GetComponent<Text>();
        statusTitleText = statusPanel.transform.Find("StatusParamPanel/Title").GetComponent<Text>();
        statusParam1Text = statusPanel.transform.Find("StatusParamPanel/Param1").GetComponent<Text>();
        statusParam2Text = statusPanel.transform.Find("StatusParamPanel/Param2").GetComponent<Text>();
    }
    private void OnEnable()
    {
        //�@���݂̃R�}���h�̏�����
        currentCommand = CommandMode.CommandPanel;
        //�@�R�}���h���j���[�\�����ɑ��̃p�l���͔�\���ɂ���
        statusPanel.SetActive(false);
        selectCharacterPanel.SetActive(false);

        // �L�����N�^�[�I���{�^��������ΑS�č폜
        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
        }

        selectedGameObjectStack.Clear();

        commandPanelCanvasGroup.interactable = true;
        selectCharacterPanelCanvasGroup.interactable = false;
        EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }
    private void Update()
    {

        //�@�L�����Z���{�^�������������̏���
        if (Input.GetButtonDown("Cancel"))
        {
            //�@�R�}���h�I����ʎ�
            if (currentCommand == CommandMode.CommandPanel)
            {
                unityChanCommandScript.ExitCommand();
                gameObject.SetActive(false);
                //�@�X�e�[�^�X�L�����N�^�[�I���܂��̓X�e�[�^�X�\����
            }
            else if (currentCommand == CommandMode.StatusPanelSelectCharacter
              || currentCommand == CommandMode.StatusPanel
              )
            {
                selectCharacterPanelCanvasGroup.interactable = false;
                selectCharacterPanel.SetActive(false);
                statusPanel.SetActive(false);
                //�@�L�����N�^�[�I���p�l���̎q�v�f�̃{�^�����폜
                for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                }
                //�@�O�̃p�l���őI�����Ă����Q�[���I�u�W�F�N�g��I��
                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                commandPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.CommandPanel;
            }
        }
    }
    //�@�I�������R�}���h�ŏ�������
    public void SelectCommand(string command)
    {
        if (command == "Status")
        {
            currentCommand = CommandMode.StatusPanelSelectCharacter;
            //�@UI�̃I���E�I�t��I���A�C�R���̐ݒ�
            commandPanelCanvasGroup.interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            GameObject characterButtonIns;

            //�@�p�[�e�B�[�����o�[���̃{�^�����쐬
            foreach (var member in partyStatus.GetAllyStatus())
            {
                characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, selectCharacterPanel.transform);
                characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
                characterButtonIns.GetComponent<Button>().onClick.AddListener(() => ShowStatus(member));
            }
        }
        //�@�K�w����ԍŌ�ɕ��בւ�
        selectCharacterPanel.transform.SetAsLastSibling();
        selectCharacterPanel.SetActive(true);
        selectCharacterPanelCanvasGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.transform.GetChild(0).gameObject);

    }
    //�@�L�����N�^�[�̃X�e�[�^�X�\��
    public void ShowStatus(AllyStatus allyStatus)
    {
        currentCommand = CommandMode.StatusPanel;
        statusPanel.SetActive(true);
        //�@�L�����N�^�[�̖��O��\��
        characterNameText.text = allyStatus.GetCharacterName();

        //�@�^�C�g���̕\��
        var text = "���x��\n";
        text += "HP\n";
        text += "MP\n";
        text += "�o���l\n";
        text += "��Ԉُ�\n";
        text += "��\n";
        text += "�f����\n";
        text += "�ł��ꋭ��\n";
        text += "���@��\n";
        text += "��������\n";
        text += "�����Z\n";
        text += "�U����\n";
        text += "�h���\n";
        statusTitleText.text = text;

        //�@HP��MP��Division�L���̕\��
        text = "\n";
        text += allyStatus.GetHp() + "\n";
        text += allyStatus.GetMp() + "\n";
        statusParam1Text.text = text;

        //�@�X�e�[�^�X�p�����[�^�̕\��
        text = allyStatus.GetLevel() + "\n";
        text += allyStatus.GetMaxHp() + "\n";
        text += allyStatus.GetMaxMp() + "\n";
        text += allyStatus.GetEarnedExperience() + "\n";
        if (!allyStatus.IsPoisonState() && !allyStatus.IsNumbnessState())
        {
            text += "����";
        }
        else
        {
            if (allyStatus.IsPoisonState())
            {
                text += "��";
                if (allyStatus.IsNumbnessState())
                {
                    text += "�AჂ�";
                }
            }
            else
            {
                if (allyStatus.IsNumbnessState())
                {
                    text += "Ⴢ�";
                }
            }
        }

        text += "\n";
        text += allyStatus.GetPower() + "\n";
        text += allyStatus.GetAgility() + "\n";
        text += allyStatus.GetStrikingStrength() + "\n";
        text += allyStatus.GetMagicPower() + "\n";
        text += allyStatus?.GetEquipWeapon()?.GetKanjiName() ?? "";
        text += "\n";
        text += allyStatus.GetEquipArmor()?.GetKanjiName() ?? "";
        text += "\n";
        text += allyStatus.GetPower() + (allyStatus.GetEquipWeapon()?.GetAmount() ?? 0) + "\n";
        text += allyStatus.GetStrikingStrength() + (allyStatus.GetEquipArmor()?.GetAmount() ?? 0) + "\n";
        statusParam2Text.text = text;
    }

}
