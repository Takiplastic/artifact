using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{//�o�g���S�̂��Ǘ�����X�N���v�g(�^�[���A�Z�N�V�������Ǘ�)
    public enum CommandMode
    {
        SelectCommand,
        SelectDirectAttacker,
        SelectMagic,
        SelectMagicAttackTarget,
        SelectUseMagicOnAlliesTarget,
        SelectBurstFinish,
        SelectBurstFinishTarget,
        SelectItem,
        SelectRecoveryItemTarget
    }

    //�@�퓬�f�[�^
    [SerializeField]
    private BattleData battleData = null;
    //�@�L�����N�^�[�̃x�[�X�ʒu
    [SerializeField]
    private Transform battleBasePosition;
    //�G�p�[�e�B�[�̃��X�g(�G�p�[�e�B�[����p)
    [SerializeField]
    private EnemyPartyStatusList enemyPartyStatusList = null;
    //�S�L�����N�^�[
    private List<GameObject> allCharacterList = new List<GameObject>();
    //���ݐ퓬�ɎQ�����Ă���S�L�����N�^�[
    private List<GameObject> allCharacterInBattleList = new List<GameObject>();
    //���ݐ퓬�ɎQ�����Ă��閡���L�����N�^�[
    private List<GameObject> allyCharacterInBattleList = new List<GameObject>();
    //���ݐ퓬�ɎQ�����Ă���G�L�����N�^�[
    private List<GameObject> enemyCharacterInBattleList = new List<GameObject>();
    //���݂̍U���̏���
    private int currentAttackOrder;
    //���ݍU�������悤�Ƃ��Ă���l���I��
    private bool isChoosing;
    //�퓬���J�n���Ă��邩�ǂ���
    private bool isStartBattle;
    //�퓬�V�[���̍ŏ��̍U�����n�܂�܂ł̑ҋ@����
    [SerializeField]
    private float firstWaitingTime = 3f;
    //�퓬�V�[���̃L�����ڍs���̊Ԃ̎���
    [SerializeField]
    private float timeToNextCharacter = 1f;
    //�҂�����
    private float waitTime;
    //�퓬�V�[���̍ŏ��̍U�����n�܂�܂ł̌o�ߎ���
    private float elapsedTime;
    //�퓬���I���������ǂ���
    private bool battleIsOver;
    //���݂̃R�}���h
    private CommandMode currentCommand;
    //�����p�[�e�B�[�̃R�}���h�p�l��
    [SerializeField]
    private Transform commandPanel = null;
    //�퓬�p�L�����N�^�[�I���{�^���v���n�u
    [SerializeField]
    private GameObject battleCharacterButton = null;
    //SelectCharacterPanel
    [SerializeField]
    private Transform selectCharacterPanel = null;
    //���@��A�C�e���I���p�l��
    [SerializeField]
    private Transform magicOrItemPanel = null;
    //���@��A�C�e���I���p�l����Content
    private Transform magicOrItemPanelContent = null;
    //BattleItemPanelButton�v���n�u
    [SerializeField]
    private GameObject battleItemPanelButton = null;
    //BattleMagicPanelButton
    [SerializeField]
    private GameObject battleMagicPanelButton = null;
    //�Ō�ɑI�����Ă����Q�[���I�u�W�F�N�g���X�^�b�N
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();
    //MagicOrItemPanel�łǂ̔ԍ��̃{�^�������ɃX�N���[�����邩
    [SerializeField]
    private int scrollDownButtonNum = 8;
    //MagicOrItemPanel�łǂ̔ԍ��̃{�^�����牺�ɃX�N���[�����邩
    [SerializeField]
    private int scrollUpButtonNum = 10;
    //ScrollManager
    private ScrollManager scrollManager;
    //���b�Z�[�W�p�l���v���n�u
    [SerializeField]
    private GameObject messagePanel;
    //BattleUI
    [SerializeField]
    private Transform battleUI;
    //���b�Z�[�W�p�l���C���X�^���X
    private GameObject messagePanelIns;
    //�퓬���ʂ̃f�[�^
    [SerializeField]
    private BattleResult battleResult;
    //�J�����}�l�[�W���[
    [SerializeField]
    private CameraController cameraController;
    // Start is called before the first frame update
    void Start()
    {
        ShowMessage("�퓬�J�n");
        //�����ŋL�q,�G�\�p�[�e�B�[�������_���Ɍ���
        if (Random.Range(0, 1) > 0.5f)
        {
            battleData.SetEnemyPartyStatus(enemyPartyStatusList.GetPartyMembersList().Find(enemyPartyStatus => enemyPartyStatus.GetPartyName() == "Tree1"));
        }
        else
        {
            battleData.SetEnemyPartyStatus(enemyPartyStatusList.GetPartyMembersList().Find(enemyPartyStatus => enemyPartyStatus.GetPartyName() == "Tree2"));
        }
        //
        //�L�����N�^�[�C���X�^���X�̐e
        Transform charactersParent = new GameObject("Characters").transform;
        //�L�����N�^�[��z�u����Transform
        Transform characterTransfom;
        //�������O�̓G�������ꍇ�̏����Ɏg�����X�g
        List<string> enemyNameList = new List<string>();

        GameObject ins;
        CharacterBattleScript characterBattleScript;
        string characterName;

        magicOrItemPanelContent = magicOrItemPanel.Find("Mask/Content");
        scrollManager = magicOrItemPanelContent.GetComponent<ScrollManager>();

        //�����p�[�e�B�[�̃v���n�u���C���X�^���X��
        for(int i=0; i<battleData.GetAllyPartyStatus().GetAllyGameObject().Count; i++)
        {
            characterTransfom = battleBasePosition.Find("AllyPos" + i).transform;
            ins = Instantiate<GameObject>(battleData.GetAllyPartyStatus().GetAllyGameObject()[i], characterTransfom.position,characterTransfom.rotation,charactersParent);
            characterBattleScript = ins.GetComponent<CharacterBattleScript>();
            ins.name = characterBattleScript.GetCharacterStatus().GetCharacterName();
            if (characterBattleScript.GetCharacterStatus().GetHp() > 0)
            {
                allyCharacterInBattleList.Add(ins);
                allCharacterList.Add(ins);
            }
        }
        if (battleData.GetEnemyPartyStatus() == null)
        {
            Debug.LogError("�G�o�[�e�B�\�f�[�^���ۑ�����Ă��܂���");
        }
        //�G�o�[�e�B�\�̃v���n�u���C���X�^���X��
        for(int i=0; i<battleData.GetEnemyPartyStatus().GetEnemyGameObjectList().Count; i++)
        {
            characterTransfom = battleBasePosition.Find("EnemyPos" + i).transform;
            ins = Instantiate<GameObject>(battleData.GetEnemyPartyStatus().GetEnemyGameObjectList()[i], characterTransfom.position, characterTransfom.rotation, charactersParent);
            //���ɓ����G�����݂����當����t������
            characterName = ins.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetCharacterName();
            if (!enemyNameList.Contains(characterName))
            {
                ins.name = characterName + 'A';
            }
            else
            {
                ins.name = characterName + (char)('A' + enemyNameList.Count(enemyName => enemyName == characterName));
            }
            enemyNameList.Add(characterName);
            enemyCharacterInBattleList.Add(ins);
            allCharacterList.Add(ins);
        }
        //�L�����N�^�[���X�g���L�����N�^�[�̑f�����̍������ɕ��בւ�
        allCharacterList = allCharacterList.OrderByDescending(character => character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetAgility()).ToList<GameObject>();
        //���݂̐퓬
        allCharacterInBattleList = allCharacterList.ToList<GameObject>();
        //�m�F�̂��ߕ��בւ������X�g��\��
        foreach(var character in allCharacterInBattleList)
        {
            Debug.Log(character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetCharacterName() + ":" + character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetAgility());
        }
        //�퓬�O�̑҂����Ԃ�ݒ�
        waitTime = firstWaitingTime;
        //�����_���l�̃V�[�h�̐ݒ�
        Random.InitState((int)Time.time);
    }

    void Update()
    {
        //�퓬���I�����Ă����炱��ȍ~�������Ȃ�
        if (battleIsOver)
        {
            return;
        }
        //�@�I���������ꂽ���i�}�E�X��UI�O���N���b�N�����j�͌��݂̃��[�h�ɂ���Ė������I��������
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (currentCommand == CommandMode.SelectCommand)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.GetChild(1).gameObject);
            }
            else if (currentCommand == CommandMode.SelectDirectAttacker)
            {
                EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.SelectMagic)
            {
                scrollManager.Reset();
                EventSystem.current.SetSelectedGameObject(magicOrItemPanelContent.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.SelectMagicAttackTarget)
            {
                EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.SelectUseMagicOnAlliesTarget)
            {
                EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.SelectBurstFinish)
            {
                scrollManager.Reset();
                EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
            }else if(currentCommand == CommandMode.SelectBurstFinishTarget)
            {
                EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.SelectItem)
            {
                scrollManager.Reset();
                EventSystem.current.SetSelectedGameObject(magicOrItemPanelContent.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.SelectRecoveryItemTarget)
            {
                EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
            }
        }
        //�퓬�J�n
        if (isStartBattle)
        {
            //���݂̃L�����N�^�[�̍U�����I����Ă���
            if (!isChoosing)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime < waitTime)
                {//�҂����Ԓ�
                    return;
                }
                elapsedTime = 0f;
                isChoosing = true;

                //�L�����N�^�[�̍U���̑I���Ɉڂ�
                Debug.Log(currentAttackOrder);
                MakeAttackChoise(allCharacterInBattleList[currentAttackOrder]);
                //���̃L�����N�^�[�̃^�[���ɂ���
                currentAttackOrder++;
                //�S���U�����I�������ŏ�����(1�^�[���o��)
                if (currentAttackOrder >= allCharacterInBattleList.Count)
                {
                    currentAttackOrder = 0;
                }
            }
            else
            {//�s���I��
                //�L�����Z���{�^�����������Ƃ��̏���
                if (Input.GetButtonDown("Cancel"))
                {
                    if (currentCommand == CommandMode.SelectDirectAttacker)
                    {
                        //�L�����N�^�[�I���{�^��������΂��ׂč폜���s���I���ɖ߂�
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }

                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        commandPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //�U���͈͂��폜(�s���I�𒆂̃L�����̔ԍ���(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%�L������)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjList��������
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectCommand;
                    }
                    else if (currentCommand == CommandMode.SelectMagic)
                    {
                        // magicOrItemPanel�Ƀ{�^��������ΑS�č폜���s���I���ɖ߂�
                        for (int i = magicOrItemPanelContent.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(magicOrItemPanelContent.transform.GetChild(i).gameObject);
                        }
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
                        magicOrItemPanel.gameObject.SetActive(false);
                        commandPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        currentCommand = CommandMode.SelectCommand;
                    }
                    else if (currentCommand == CommandMode.SelectMagicAttackTarget)
                    {
                        // selectCharacterPanel�Ƀ{�^��������ΑS�č폜�����@�I���ɖ߂�
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //�U���͈͂��폜(�s���I�𒆂̃L�����̔ԍ���(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%�L������)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjList��������
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectMagic;
                    }
                    else if (currentCommand == CommandMode.SelectUseMagicOnAlliesTarget)
                    {
                        // selectCharacterPanel�Ƀ{�^��������ΑS�č폜�����@�I���ɖ߂�
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //�U���͈͂��폜(�s���I�𒆂̃L�����̔ԍ���(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%�L������)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjList��������
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectMagic;
                    }
                    else if (currentCommand == CommandMode.SelectBurstFinish)
                    {
                        // magicOrItemPanel�Ƀ{�^��������ΑS�č폜���s���I���ɖ߂�
                        for (int i = magicOrItemPanelContent.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(magicOrItemPanelContent.transform.GetChild(i).gameObject);
                        }
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
                        magicOrItemPanel.gameObject.SetActive(false);
                        commandPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        currentCommand = CommandMode.SelectCommand;
                    }
                    else if(currentCommand == CommandMode.SelectBurstFinishTarget)
                    {
                        // selectCharacterPanel�Ƀ{�^��������ΑS�č폜���A�C�e���I���ɖ߂�
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //�U���͈͂��폜(�s���I�𒆂̃L�����̔ԍ���(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%�L������)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjList��������
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectBurstFinish;
                    }
                    else if (currentCommand == CommandMode.SelectItem)
                    {
                        // magicOrItemPanel�Ƀ{�^��������ΑS�č폜���s���I���ɖ߂�
                        for (int i = magicOrItemPanelContent.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(magicOrItemPanelContent.transform.GetChild(i).gameObject);
                        }
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
                        magicOrItemPanel.gameObject.SetActive(false);
                        commandPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        currentCommand = CommandMode.SelectCommand;
                    }
                    else if (currentCommand == CommandMode.SelectRecoveryItemTarget)
                    {
                        // selectCharacterPanel�Ƀ{�^��������ΑS�č폜���A�C�e���I���ɖ߂�
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //�U���͈͂��폜(�s���I�𒆂̃L�����̔ԍ���(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%�L������)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjList��������
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectItem;
                    }
                    
                }
            }
        }else
        { 
            //�҂����Ԓ�
            Debug.Log("�o�ߎ���:" + elapsedTime);
            //�퓬�O�̑ҋ@
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waitTime)
            {   //���ڈȍ~�̓L�����Ԃ̑҂����Ԃ�ݒ�
                waitTime = timeToNextCharacter;
                //�ŏ��̃L�����N�^�[�̑҂����Ԃ�0�ɂ��邽�߂ɂ��炩���ߏ������N���A�����Ă���
                elapsedTime = timeToNextCharacter;
                isStartBattle = true;
            }
        }
    }

    //�L�����N�^�[�̍U���̑I������
    public void MakeAttackChoise(GameObject character)
    {
        CharacterStatus characterStatus = character.GetComponent<CharacterBattleScript>().GetCharacterStatus();
        //EnemyStatus�L���X�g�ł���ꍇ�͓G�̍U������
        if(characterStatus as EnemyStatus != null)
        {
            Debug.Log(character.gameObject.name + "�̍U��");
           StartCoroutine(EnemyAttack(character));
        }
        else
        {//�����̍U��
            Debug.Log(characterStatus.GetCharacterName() + "�̍U��");
            //�^�[���̏��߂Ƀo�[�X�g���[�h�Ɉڍs�ł��邩�`�F�b�N+�J�������L�����ɃZ�b�g
            character.GetComponent<CharacterBattleScript>().ChangeBurstMode(1);
            AllyAttack(character);
        }
    }
    //�����̍U������(�R�}���h�p�l������+�{�^���Ƀ��\�b�h���A�^�b�`)
    public void AllyAttack(GameObject character)
    {
        //�s������L�����N�^�[�̃}�[�J�[��\��
        character.transform.Find("Marker/Image2").gameObject.SetActive(true);
        currentCommand = CommandMode.SelectCommand;
        //�O�̃L�����̍s�������̍ۂɎg�����p�l���A�{�^���̍폜
        //�L�����N�^�[�I���{�^��������΂��ׂč폜
        for(int i = selectCharacterPanel.transform.childCount - 1;i>= 0; i--)
        {
            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
        }
        //���@��A�C�e���p�l���̎q�v�f��Content�Ƀ{�^��������΂��ׂč폜
        for(int i = magicOrItemPanelContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(magicOrItemPanelContent.transform.GetChild(i).gameObject);
        }
        
        commandPanel.GetComponent<CanvasGroup>().interactable = true;
        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;

        //�L�����N�^�[���K�[�h��Ԃł���΃K�[�h������
        if (character.GetComponent<Animator>().GetBool("Guard"))
        {
            character.GetComponent<CharacterBattleScript>().UnlockGuard();
        }

        //�L�����N�^�[�̖��O��\��
        commandPanel.Find("CharacterName/Text").GetComponent<Text>().text = character.name;

        var characterSkill = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
        //�����Ă���X�L���ɉ����ăR�}���h�{�^����\��
        if (characterSkill.Exists(skill => skill.GetSkillType() == Skill.Type.DirectAttack))
        {//�U��
            var directAttackButtonObj = commandPanel.Find("DirectAttack").gameObject;
            var directAttackButton = directAttackButtonObj.GetComponent<Button>();
            directAttackButton.onClick.RemoveAllListeners();
            directAttackButtonObj.GetComponent<Button>().onClick.AddListener(() => SelectDirectAttacker(character));
            directAttackButtonObj.SetActive(true);
        }
        else
        {
            commandPanel.Find("DirectAttack").gameObject.SetActive(false);
        }
        if(characterSkill.Exists(skill => skill.GetSkillType() == Skill.Type.Guard)){
            var guardButtonObj = commandPanel.Find("Guard").gameObject;
            var guardButton = guardButtonObj.GetComponent<Button>();
            guardButton.onClick.RemoveAllListeners();
            guardButton.onClick.AddListener(() => Guard(character));
            guardButtonObj.SetActive(true);
        }
        else
        {
            commandPanel.Find("Guard").gameObject.SetActive(false);
        }
        if (characterSkill.Exists(skill => skill.GetSkillType() == Skill.Type.Item))
        {
            var itemButtonObj = commandPanel.Find("Item").gameObject;
            var itemButton = itemButtonObj.GetComponent<Button>();
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => SelectItem(character));
            commandPanel.Find("Item").gameObject.SetActive(true);
        }
        else
        {
            commandPanel.Find("Item").gameObject.SetActive(false);
        }
        if (characterSkill.Exists(skill => skill.GetSkillType() == Skill.Type.MagicAttack)
            || characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.IncreaseAttackPowerMagic)
            || characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic) 
            || characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.IncreaseMagicPowerMagic)
            || characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.IncreaseMagicStrengthMagic)
            || characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic)
            || characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic)
            || characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.RecoveryMagic))
        {

            var magicButtonObj = commandPanel.Find("Magic").gameObject;
            var magicButton = magicButtonObj.GetComponent<Button>();
            magicButton.onClick.RemoveAllListeners();
            magicButton.onClick.AddListener(() => SelectMagic(character));

            magicButtonObj.SetActive(true);
        }
        else
        {
            commandPanel.Find("Magic").gameObject.SetActive(false);
        }
        if (characterSkill.Exists(skill => skill.GetSkillType() == Skill.Type.BurstFinish))
        {
            var specialButtonObj = commandPanel.Find("Special").gameObject;
            var specialButton = specialButtonObj.GetComponent<Button>();
            specialButton.onClick.RemoveAllListeners();
            specialButton.onClick.AddListener(() => SelectBurstFinish(character));
            if (character.GetComponent<CharacterBattleScript>().IsBurstMode())
            {
                specialButtonObj.SetActive(true);
            }
            else
            {//�X�y�V�����{�^���̓o�[�X�g���[�h�łȂ��Ǝg���Ȃ�
                specialButtonObj.SetActive(false);
            }
        }
        else
        {
            commandPanel.Find("Special").gameObject.SetActive(false);
        }
        if (characterSkill.Exists(skill => skill.GetSkillType() == Skill.Type.GetAway))
        {
            var getAwayButtonObj = commandPanel.Find("GetAway").gameObject;
            var getAwayButton = getAwayButtonObj.GetComponent<Button>();
            getAwayButton.onClick.RemoveAllListeners();
            getAwayButton.onClick.AddListener(() => GetAway(character));
            getAwayButtonObj.SetActive(true);
        }
        else
        {
            commandPanel.Find("GetAway").gameObject.SetActive(false);
        }
        EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(1).gameObject);
        commandPanel.gameObject.SetActive(true);
    }
    //���ڍU������̑I��(�X�L���͈͓��̓G���ׂĂɍU������ver)
    public void SelectDirectAttacker(GameObject attackCharacter)
    {
        currentCommand = CommandMode.SelectDirectAttacker;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        Transform skillareapos = attackCharacter.transform.Find("SkillAreaPos").transform;
        //�U������L������DirectAttack�X�L�����擾����
        var characterSkill = attackCharacter.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
        Skill directAttack = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack);
        //�Z�͈͂̍쐬
        GameObject ins = Instantiate<GameObject>(directAttack.GetSkillArea(), skillareapos.position, attackCharacter.transform.rotation, skillareapos);
        //�^�[�Q�b�g�̃^�O��enemy
        ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("enemy");
        //�G�L�����̃{�^�����쐬+�������璼�ڍU�������s����悤�ɐݒ�
        GameObject battleCharacterButtonIns;
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "�퓬�J�n";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => DirectAttackDemo(attackCharacter));

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }
    //���ڍU��
    public void DirectAttack(GameObject attackCharacter, GameObject attackTarget)
    {
        //�U������L������DirectAttack�X�L�����擾����
        var characterSkill = attackCharacter.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
        Skill directAttack = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack);
        attackCharacter.GetComponent<CharacterBattleScript>().ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, attackTarget, directAttack);
        commandPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //���ڍU��(�����U���Ώ�)
    public void DirectAttackDemo(GameObject attackCharacter)
    {
        //�U������L������DirectAttack�X�L�����擾����
        var characterBattleScript = attackCharacter.GetComponent<CharacterBattleScript>();
        var characterSkill = characterBattleScript.GetCharacterStatus().GetSkillList();
        Skill directAttack = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack);
        characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, directAttack);
        commandPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //�h��
    public void Guard(GameObject guardCharacter)
    {
        guardCharacter.GetComponent<CharacterBattleScript>().Guard(guardCharacter.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList().Find(skill=>skill.GetSkillType()==Skill.Type.Guard));
        commandPanel.gameObject.SetActive(false);
        //�J��������Ղɖ߂�
        cameraController.ResetCamera();
        ChangeNextChara();
    }
    //���@�X�L���̃{�^���𐶐�+�������Ƃ��̋@�\���Z�b�g
    public void SelectMagic(GameObject character)
    {
        currentCommand = CommandMode.SelectMagic;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleMagicPanelButtonIns;
        var skillList = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();

        //MagicOrItemPanel�̃X�N���[���l�̏�����
        scrollManager.Reset();
        int battleMagicPanelButtonNum = 0;

        foreach(var skill in skillList)
        {
            if(skill.GetSkillType() == Skill.Type.MagicAttack 
                || skill.GetSkillType() == Skill.Type.RecoveryMagic 
                || skill.GetSkillType() == Skill.Type.IncreaseAttackPowerMagic 
                || skill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic
                || skill.GetSkillType() == Skill.Type.IncreaseMagicPowerMagic
                || skill.GetSkillType() == Skill.Type.IncreaseMagicStrengthMagic
                || skill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic 
                || skill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic)
            {
                battleMagicPanelButtonIns = Instantiate<GameObject>(battleMagicPanelButton, magicOrItemPanelContent);
                battleMagicPanelButtonIns.transform.Find("MagicName").GetComponent<Text>().text = skill.GetKanjiName();
                battleMagicPanelButtonIns.transform.Find("AmountToUseMagicPoints").GetComponent<Text>().text = ((Magic)skill).GetAmountToUseMagicPoints().ToString();

                //�w�肵���ԍ��̃A�C�e���p�l���{�^���ɃA�C�e���X�N���[���p�̃X�N���v�g�����t����
                if (battleMagicPanelButtonNum != 0 &&(battleMagicPanelButtonNum%scrollDownButtonNum==0 || battleMagicPanelButtonNum%(scrollDownButtonNum+1)==0))
                {
                    //�A�C�e���X�N���[���X�N���v�g�����t���Đݒ�l�̃Z�b�g
                    battleMagicPanelButtonIns.AddComponent<ScrollDownScript>();
                }else if(battleMagicPanelButtonNum!=0 && (battleMagicPanelButtonNum % scrollUpButtonNum == 0 || battleMagicPanelButtonNum % (scrollUpButtonNum + 1) == 0))
                {
                    battleMagicPanelButtonIns.AddComponent<ScrollUpScript>();
                }

                //MP������Ȃ��Ƃ��̓{�^���������Ă������������@�̖��O���Â�����
                if (character.GetComponent<CharacterBattleScript>().GetMp() < ((Magic)skill).GetAmountToUseMagicPoints())
                {
                    battleMagicPanelButtonIns.transform.Find("MagicName").GetComponent<Text>().color = new Color(0.4f, 0.4f, 0.4f);
                }
                else
                {
                    battleMagicPanelButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectUseMagicTarget(character, skill));
                }
                //�{�^���̔ԍ��𑫂�
                battleMagicPanelButtonNum++;

                if (battleMagicPanelButtonNum == scrollUpButtonNum + 2)
                {
                    battleMagicPanelButtonNum = 2;
                }
            }
        }
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(magicOrItemPanelContent.GetChild(0).gameObject);
        magicOrItemPanel.gameObject.SetActive(true);
    }
    
    //���@���g������̑I��(�����Ώ�)
    public void SelectUseMagicTarget(GameObject attackCharacter,Skill skill)
    {
        currentCommand = CommandMode.SelectUseMagicOnAlliesTarget;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        Transform skillareapos = attackCharacter.transform.Find("SkillAreaPos").transform;
        //�Z�͈͂̍쐬
        GameObject ins=Instantiate<GameObject>(skill.GetSkillArea(), skillareapos.position, attackCharacter.transform.rotation, skillareapos);
        if (skill.GetSkillType() == Skill.Type.MagicAttack)
        {//�^�[�Q�b�g���G�̖��@
            ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("enemy");
        }
        else
        {//�^�[�Q�b�g�������̖��@
            ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("Player");
        }
        //�G�L�����̃{�^�����쐬+��������s�������s����悤�ɐݒ�
        GameObject battleCharacterButtonIns;
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "�퓬�J�n";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseMagicDemo(attackCharacter,skill));

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);

    }
    //���@���g��
    public void UseMagic(GameObject user,GameObject targetCharacter, Skill skill)
    {
        CharacterBattleScript.BattleState battleState = CharacterBattleScript.BattleState.Idle;
        //���@���g�������CharacterBattleScript���擾���Ă���
        var targetCharacterBattleScript = targetCharacter.GetComponent<CharacterBattleScript>();
        if (skill.GetSkillType() == Skill.Type.MagicAttack)
        {
            battleState = CharacterBattleScript.BattleState.MagicAttack;
        }
        else if (skill.GetSkillType() == Skill.Type.RecoveryMagic)
        {
            if (targetCharacterBattleScript.GetHp() == targetCharacterBattleScript.GetCharacterStatus().GetMaxMp())
            {
                ShowMessage(targetCharacter.name + "�͑S���ł�");
                Debug.Log(targetCharacter.name + "�͑S���ł�");
                return;
            }
            battleState = CharacterBattleScript.BattleState.Healing;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseAttackPowerMagic)
        {
            if (targetCharacterBattleScript.IsIncreasePower())
            {
                ShowMessage(targetCharacter.name + "�͊��ɍU���͂������Ă��܂�");
                Debug.Log(targetCharacter.name + "�͊��ɍU���͂������Ă��܂�");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseAttackPowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic)
        {
            if (targetCharacterBattleScript.IsIncreaseStrikingStrength())
            {
                ShowMessage("���ɖh��͂��グ�Ă��܂��B");
                Debug.Log("���ɖh��͂��グ�Ă��܂��B");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseDefencePowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseMagicPowerMagic)
        {
            if (targetCharacterBattleScript.IsIncreaseMagicPower())
            {
                ShowMessage("���ɖ��@�U���͂��グ�Ă��܂��B");
                Debug.Log("���ɖ��@�U���͂��グ�Ă��܂��B");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseMagicPowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseMagicStrengthMagic)
        {
            if (targetCharacterBattleScript.IsIncreaseMagicStrength())
            {
                ShowMessage("���ɖ��@�h��͂��グ�Ă��܂��B");
                Debug.Log("���ɖ��@�h��͂��グ�Ă��܂��B");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseMagicStrengthMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic)
        {
            if (!targetCharacterBattleScript.IsNumbness())
            {
                ShowMessage(targetCharacter.name + "��Ⴢ��Ԃł͂���܂���B");
                Debug.Log(targetCharacter.name + "��Ⴢ��Ԃł͂���܂���B");
                return;
            }
            battleState = CharacterBattleScript.BattleState.NumbnessRecoveryMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic)
        {
            if (!targetCharacterBattleScript.IsPoison())
            {
                ShowMessage(targetCharacter.name + "�͓ŏ�Ԃł͂���܂���B");
                Debug.Log(targetCharacter.name + "�͓ŏ�Ԃł͂���܂���B");
                return;
            }
            battleState = CharacterBattleScript.BattleState.PoisonnouRecoveryMagic;
        }
        user.GetComponent<CharacterBattleScript>().ChooseAttackOptions(battleState, targetCharacter, skill);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //���@(�����U���Ώ�)
    public void UseMagicDemo(GameObject user,Skill skill)
    {
        CharacterBattleScript.BattleState battleState = CharacterBattleScript.BattleState.Idle;
        if (skill.GetSkillType() == Skill.Type.MagicAttack)
        {
            battleState = CharacterBattleScript.BattleState.MagicAttack;
        }
        else if (skill.GetSkillType() == Skill.Type.RecoveryMagic)
        {
            battleState = CharacterBattleScript.BattleState.Healing;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseAttackPowerMagic)
        {

            battleState = CharacterBattleScript.BattleState.IncreaseAttackPowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic)
        {

            battleState = CharacterBattleScript.BattleState.IncreaseDefencePowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseMagicPowerMagic)
        {

            battleState = CharacterBattleScript.BattleState.IncreaseMagicPowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseMagicStrengthMagic)
        {

            battleState = CharacterBattleScript.BattleState.IncreaseMagicStrengthMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic)
        {

            battleState = CharacterBattleScript.BattleState.NumbnessRecoveryMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic)
        {

            battleState = CharacterBattleScript.BattleState.PoisonnouRecoveryMagic;
        }
        user.GetComponent<CharacterBattleScript>().ChooseAttackOptions(battleState, skill);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //�o�[�X�g�t�B�j�b�V���X�̃{�^���𐶐�+�������Ƃ��̋@�\���Z�b�g
    public void SelectBurstFinish(GameObject character)
    {
        currentCommand = CommandMode.SelectBurstFinish;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleSpecialPanelButtonIns;
        var skillList = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();

        //MagicOrItemPanel�̃X�N���[���l�̏�����
        scrollManager.Reset();
        int battleSpecialPanelButtonNum = 0;

        foreach (var skill in skillList)
        {
            if (skill.GetSkillType() == Skill.Type.BurstFinish)
            {
                battleSpecialPanelButtonIns = Instantiate<GameObject>(battleMagicPanelButton, magicOrItemPanelContent);
                battleSpecialPanelButtonIns.transform.Find("MagicName").GetComponent<Text>().text = skill.GetKanjiName();
                //battleSpecialPanelButtonIns.transform.Find("AmountToUseMagicPoints").GetComponent<Text>().text = ((Magic)skill).GetAmountToUseMagicPoints().ToString();

                //�w�肵���ԍ��̃A�C�e���p�l���{�^���ɃA�C�e���X�N���[���p�̃X�N���v�g�����t����
                if (battleSpecialPanelButtonNum != 0 && (battleSpecialPanelButtonNum % scrollDownButtonNum == 0 || battleSpecialPanelButtonNum % (scrollDownButtonNum + 1) == 0))
                {
                    //�A�C�e���X�N���[���X�N���v�g�����t���Đݒ�l�̃Z�b�g
                    battleSpecialPanelButtonIns.AddComponent<ScrollDownScript>();
                }
                else if (battleSpecialPanelButtonNum != 0 && (battleSpecialPanelButtonNum % scrollUpButtonNum == 0 || battleSpecialPanelButtonNum % (scrollUpButtonNum + 1) == 0))
                {
                    battleSpecialPanelButtonIns.AddComponent<ScrollUpScript>();
                }
                //�{�^����������������
                battleSpecialPanelButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectUseBurstFinishTarget(character, skill));
                //�{�^���̔ԍ��𑫂�
                battleSpecialPanelButtonNum++;

                if (battleSpecialPanelButtonNum == scrollUpButtonNum + 2)
                {
                    battleSpecialPanelButtonNum = 2;
                }
            }
        }
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(magicOrItemPanelContent.GetChild(0).gameObject);
        magicOrItemPanel.gameObject.SetActive(true);
    }
    //�o�[�X�g�t�B�j�b�V�����g������̑I��
    public void SelectUseBurstFinishTarget(GameObject user, Skill skill)
    {
        currentCommand = CommandMode.SelectBurstFinishTarget;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleCharacterButtonIns;

        Transform skillareapos = user.transform.Find("SkillAreaPos").transform;
        //�X�L���͈͂̍쐬
        GameObject ins = Instantiate<GameObject>(skill.GetSkillArea(), skillareapos.position, user.transform.rotation, skillareapos);
        //�^�[�Q�b�g�̃^�O��enemy
        ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("enemy");
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "�퓬�J�n";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseBurstFinish(user, skill));
        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }
    //�o�[�X�g�t�B�j�b�V�����g��
    public void UseBurstFinish(GameObject user, Skill skill)
    {
        CharacterBattleScript.BattleState battleState = CharacterBattleScript.BattleState.Idle;
        battleState = CharacterBattleScript.BattleState.BurstFinish;
        user.GetComponent<CharacterBattleScript>().ChooseAttackOptions(battleState, skill);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //�g�p����A�C�e���̑I��
    public void SelectItem(GameObject character)
    {
        var itemDictionary = ((AllyStatus)character.GetComponent<CharacterBattleScript>().GetCharacterStatus()).GetItemDictionary();

        //MagicOrItemPanel�̃X�N���[���l�̏�����
        scrollManager.Reset();
        var battleItemPanelButtonNum = 0;
        GameObject battleItemPanelButtonIns;

        foreach (var item in itemDictionary.Keys)
        {
            if (item.GetItemType() == Item.Type.HPRecovery
                || item.GetItemType() == Item.Type.MPRecovery
                || item.GetItemType() == Item.Type.NumbnessRecovery
                || item.GetItemType() == Item.Type.PoisonRecovery
                )
            {
                battleItemPanelButtonIns = Instantiate<GameObject>(battleItemPanelButton, magicOrItemPanelContent);
                //�A�C�e���{�^���̖��O�ύX
                battleItemPanelButtonIns.transform.Find("ItemName").GetComponent<Text>().text = item.GetKanjiName();
                //�A�C�e���̐���\��
                battleItemPanelButtonIns.transform.Find("Num").GetComponent<Text>().text = ((AllyStatus)character.GetComponent<CharacterBattleScript>().GetCharacterStatus()).GetItemNum(item).ToString();
                battleItemPanelButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectItemTarget(character, item));

                //�@�w�肵���ԍ��̃A�C�e���p�l���{�^���ɃA�C�e���X�N���[���p�X�N���v�g�����t����
                if (battleItemPanelButtonNum != 0
                    && (battleItemPanelButtonNum % scrollDownButtonNum == 0
                    || battleItemPanelButtonNum % (scrollDownButtonNum + 1) == 0)
                    )
                {
                    //�@�A�C�e���X�N���[���X�N���v�g�̎��t���Đݒ�l�̃Z�b�g
                    battleItemPanelButtonIns.AddComponent<ScrollDownScript>();
                }
                else if (battleItemPanelButtonNum != 0
                  && (battleItemPanelButtonNum % scrollUpButtonNum == 0
                  || battleItemPanelButtonNum % (scrollUpButtonNum + 1) == 0)
                  )
                {
                    battleItemPanelButtonIns.AddComponent<ScrollUpScript>();
                }
                //�@�{�^���ԍ��𑫂�
                battleItemPanelButtonNum++;

                if (battleItemPanelButtonNum == scrollUpButtonNum + 2)
                {
                    battleItemPanelButtonNum = 2;
                }
            }
        }

        if (magicOrItemPanelContent.childCount > 0)
        {
            currentCommand = CommandMode.SelectItem;
            commandPanel.GetComponent<CanvasGroup>().interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
            EventSystem.current.SetSelectedGameObject(magicOrItemPanelContent.GetChild(0).gameObject);
            magicOrItemPanel.gameObject.SetActive(true);
        }
        else
        {
            ShowMessage("�g����A�C�e��������܂���B");
            Debug.Log("�g����A�C�e��������܂���B");
        }
    }

    //�A�C�e�����g�p���鑊���I��(�����L�����N�^�[�ɑ΂��ăA�C�e�����g�p����ꍇ�̂�)
    public void SelectItemTarget(GameObject user, Item item)
    {
        currentCommand = CommandMode.SelectRecoveryItemTarget;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        Transform skillareapos = user.transform.Find("SkillAreaPos").transform;
        //�A�C�e�����ʔ͈͂̍쐬
        GameObject ins = Instantiate<GameObject>(item.GetItemArea(), skillareapos.position, user.transform.rotation, skillareapos);
        //�^�[�Q�b�g�̃^�O��Player
        ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("Player");
        GameObject battleCharacterButtonIns;
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "�퓬�J�n";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItem(user, item));
        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }
    //�A�C�e���g�p
    public void UseItem(GameObject user, Item item)
    {
        var userCharacterBattleScript = user.GetComponent<CharacterBattleScript>();
        var skill = userCharacterBattleScript.GetCharacterStatus().GetSkillList().Find(skills => skills.GetSkillType() == Skill.Type.Item);

        CharacterBattleScript.BattleState battleState = CharacterBattleScript.BattleState.Idle;

        if (item.GetItemType() == Item.Type.HPRecovery)
        {
            battleState = CharacterBattleScript.BattleState.UseHPRecoveryItem;
        }
        else if (item.GetItemType() == Item.Type.MPRecovery)
        {
            battleState = CharacterBattleScript.BattleState.UseMPRecoveryItem;
        }
        else if (item.GetItemType() == Item.Type.NumbnessRecovery)
        {
            battleState = CharacterBattleScript.BattleState.UseNumbnessRecoveryItem;
        }
        else if (item.GetItemType() == Item.Type.PoisonRecovery)
        {
            battleState = CharacterBattleScript.BattleState.UsePoisonRecoveryItem;
        }
        userCharacterBattleScript.ChooseAttackOptions(battleState, skill, item);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }

    //�@������
    public void GetAway(GameObject character)
    {

        //�A�C�R�����\��
        character.transform.Find("Marker/Image2").gameObject.SetActive(false);
        var randomValue = Random.value;
        if (0f <= randomValue && randomValue <= 0.2f)
        {
            ShowMessage("������̂ɐ��������B");
            Debug.Log("������̂ɐ��������B");
            battleIsOver = true;
            commandPanel.gameObject.SetActive(false);
            //�@�퓬�I��
            battleResult.InitialProcessingOfRanAwayResult();
        }
        else
        {
            ShowMessage("������̂Ɏ��s�����B");
            Debug.Log("������̂Ɏ��s�����B");
            commandPanel.gameObject.SetActive(false);
            ChangeNextChara();
        }
    }
    //�G�̈ړ�����
    IEnumerator EnemyAutoMove(GameObject character,Skill skill)
    {
        
        Transform skillareapos = character.transform.Find("SkillAreaPos").transform;
        //�Z�͈͂̍쐬
        Debug.Log(skill.GetSkillType());
        GameObject ins = Instantiate<GameObject>(skill.GetSkillArea(), skillareapos.position, character.transform.rotation, skillareapos);
        GameObject closesttarget = null;
        Vector3 v= new Vector3(0.0f,0.0f,0.0f);
        float distance = 1000.0f;
        if (skill.GetSkillType()== Skill.Type.DirectAttack || skill.GetSkillType() == Skill.Type.MagicAttack)
        {//�^�[�Q�b�g�̓v���C���[
            ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("Player");
            foreach (var child in allyCharacterInBattleList)
            {
                Vector3 vector = new Vector3(child.transform.position.x - character.transform.position.x, 0, child.transform.position.z - character.transform.position.z);
                if (distance > vector.magnitude)
                {
                    distance = vector.magnitude;
                    closesttarget = child;
                    v = vector;
                }
            }
        }
        else
        {//�^�[�Q�b�g�͓G
            ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("enemy");
            foreach (var child in enemyCharacterInBattleList)
            {
                if (child == character)
                    continue;
                Vector3 vector = new Vector3(child.transform.position.x - character.transform.position.x, 0, child.transform.position.z - character.transform.position.z);
                if (distance > vector.magnitude)
                {
                    distance = vector.magnitude;
                    closesttarget = child;
                    v = vector.normalized;
                }
            }
        }
        Debug.Log("closest" + closesttarget.name);
        //��ԋ߂��^�[�Q�b�g�̌���������
        character.transform.LookAt(closesttarget.transform);
        //�X�N���v�g�̎擾
        CharacterBattleScript characterBattleScript = character.GetComponent<CharacterBattleScript>();
        Rigidbody rb= character.GetComponent<Rigidbody>();

        //�^�[�Q�b�g���X�g����łȂ��Ȃ�܂ŃL�����𓮂���
        yield return StartCoroutine(movingwait(characterBattleScript, rb, v));
        //�������I�������X�L���͈͍͂폜
        Destroy(ins);
    }
    //���b�����ƂɈړ�
    IEnumerator movingwait(CharacterBattleScript characterBattleScript,Rigidbody rb, Vector3 v)
    {
        List<GameObject> objList = new List<GameObject>();
        int i =0;
        while(objList.Count == 0 || i > 1000)
        {
            yield return new WaitForSeconds(0.02f);
            rb.position += 0.1f * v;
            objList = characterBattleScript.GettargetObjList();
            i++;
        }
        if (i > 1000)
        {
            Debug.LogError("i>1000");
        }
    }
    //�G�̍U������
    public IEnumerator EnemyAttack(GameObject character)
    {
        CharacterBattleScript characterBattleScript = character.GetComponent<CharacterBattleScript>();
        CharacterStatus characterStatus = characterBattleScript.GetCharacterStatus();

        if (characterStatus.GetSkillList().Count <= 0)
        {
            Debug.Log("Skill 0");
           yield return null;
        }
        //�G���K�[�h��Ԃł���΃K�[�h������
        if (character.GetComponent<Animator>().GetBool("Guard"))
        {
            character.GetComponent<CharacterBattleScript>().UnlockGuard();
        }

        //�G�̍s���A���S���Y��
        int randomValue = (int)(Random.value * characterStatus.GetSkillList().Count);
        var nowSkill = characterStatus.GetSkillList()[0];

        //������܂��̓K�[�h�̎��͈ړ����s��Ȃ�
        if (!(nowSkill.GetSkillType() == Skill.Type.GetAway || nowSkill.GetSkillType() == Skill.Type.Guard))
        {
            Coroutine coroutine = StartCoroutine(EnemyAutoMove(character, nowSkill));
            yield return coroutine;
        }
        

        //�X�L���̃^�C�v�ʂɏ������L�q����( GetAway,PoisonnouRecoveryMagic,NumbnessRecoveryMagic����`)
        if (nowSkill.GetSkillType() == Skill.Type.DirectAttack)
        {
            characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack,nowSkill);
            Debug.Log(character.name + "��" + nowSkill.GetKanjiName() + "���s����");
        }else if (nowSkill.GetSkillType() == Skill.Type.MagicAttack)
        {
            if (characterBattleScript.GetMp() >= ((Magic)nowSkill).GetAmountToUseMagicPoints()){
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.MagicAttack,  nowSkill);
                Debug.Log(character.name + "��" + nowSkill.GetKanjiName() + "���s����");
            }
            else
            {
                ShowMessage("MP������Ȃ�!");
                Debug.Log("MP������Ȃ�!");
                //MP������Ȃ��ꍇ�͒��ڍU�����s��
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, characterStatus.GetSkillList().Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack));
                Debug.Log(character.name + "�͍U�����s����");
            }
        }else if(nowSkill.GetSkillType() == Skill.Type.Guard)
        {
            characterBattleScript.Guard(nowSkill);
            //Guard�A�j����bool�Ȃ̂ŃA�j���[�V���J�ڂ������炷���Ɏ��̃L�����N�^�[�Ɉڍs������
            ChangeNextChara();
            ShowMessage(character.name + "��" + nowSkill.GetKanjiName() + "���s����");
            Debug.Log(character.name + "��" + nowSkill.GetKanjiName() + "���s����");
        }else if (nowSkill.GetSkillType() == Skill.Type.RecoveryMagic)
        {//�񕜑���̌���
            if (characterBattleScript.GetMp() >= ((Magic)nowSkill).GetAmountToUseMagicPoints())
            {
                //�񕜑����CharacterBattleScript
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.Healing, nowSkill);
                Debug.Log(character.name + "��" + nowSkill.GetKanjiName() + "���s����");
            }
            else
            {
                ShowMessage("MP������Ȃ�!");
                Debug.Log("MP������Ȃ�!");
                Debug.Log(character.name + "�͉����ł��Ȃ�����");
            }
        }
        else if(nowSkill.GetSkillType()==Skill.Type.IncreaseAttackPowerMagic 
            || nowSkill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic 
            || nowSkill.GetSkillType() == Skill.Type.IncreaseMagicPowerMagic 
            || nowSkill.GetSkillType() == Skill.Type.IncreaseMagicStrengthMagic
            || nowSkill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic
            || nowSkill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic)
        {
            if (characterBattleScript.GetMp() >= ((Magic)nowSkill).GetAmountToUseMagicPoints())
            {
                //�o�t�����CharacterBattleScript
                if (nowSkill.GetSkillType() == Skill.Type.IncreaseAttackPowerMagic)
                {
                    characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.IncreaseAttackPowerMagic, nowSkill);
                }
                else if(nowSkill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic)
                {
                    characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.IncreaseDefencePowerMagic, nowSkill);
                }
                else if(nowSkill.GetSkillType() == Skill.Type.IncreaseMagicPowerMagic)
                {
                    characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.IncreaseMagicPowerMagic, nowSkill);
                }
                else if(nowSkill.GetSkillType() == Skill.Type.IncreaseMagicStrengthMagic)
                {
                    characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.IncreaseMagicStrengthMagic, nowSkill);
                }
                else if(nowSkill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic)
                {
                    characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.PoisonnouRecoveryMagic, nowSkill);
                }
                else if(nowSkill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic)
                {
                    characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.NumbnessRecoveryMagic, nowSkill);
                }
                    Debug.Log(character.name + "��" + nowSkill.GetKanjiName() + "���s����");
            }
            else
            {
                ShowMessage("MP������Ȃ�!");
                Debug.Log("MP������Ȃ�!");
                Debug.Log(character.name + "�͉����ł��Ȃ�����");
            }
        }
        else
        {
            Debug.Log("����`�X�L���^�C�v:" + nowSkill.GetSkillType());
        }
    }
    //���̃L�����N�^�[�Ɉڍs
    public void ChangeNextChara()
    {
        isChoosing = false;
    }
    //���񂾃L�����N�^�[��S�L�������X�g����폜
    public void DeleteAllCharacterInBattleList(GameObject deleteObj)
    {
        var deleteObjNum = allCharacterInBattleList.IndexOf(deleteObj);
        allCharacterInBattleList.Remove(deleteObj);
        if (deleteObjNum < currentAttackOrder)
        {
            currentAttackOrder--;
        }
        //�S���U�����I�������ŏ�����
        if (currentAttackOrder >= allCharacterInBattleList.Count)
        {
            currentAttackOrder = 0;
        }
    }
    //�����L���������񂾂Ƃ��ɖ����L�������X�g����폜
    public void DeleteAllyCharacterInBattleList(GameObject deleteObj)
    {
        allyCharacterInBattleList.Remove(deleteObj);
        if (allyCharacterInBattleList.Count == 0)
        {
            ShowMessage("�������S��");
            Debug.Log("�������S��");
            battleIsOver = true;
            CharacterBattleScript characterBattleScript;
            foreach (var character in allCharacterList)
            {
                //�@�����L�����N�^�[�̐퓬�ő�������HP��MP��ʏ�̃X�e�[�^�X�ɔ��f������
                characterBattleScript = character.GetComponent<CharacterBattleScript>();
                if (characterBattleScript.GetCharacterStatus() as AllyStatus != null)
                {
                    characterBattleScript.GetCharacterStatus().SetHp(characterBattleScript.GetHp());
                    characterBattleScript.GetCharacterStatus().SetMp(characterBattleScript.GetMp());
                    characterBattleScript.GetCharacterStatus().SetNumbness(characterBattleScript.IsNumbness());
                    characterBattleScript.GetCharacterStatus().SetPoisonState(characterBattleScript.IsPoison());
                }
            }
            //�@�s�펞�̌��ʕ\��
            battleResult.InitialProcessingOfDefeatResult();
        }
    }
    //�G�L���������񂾂Ƃ��ɓG�L�������X�g����폜
    public void DeleteEnemyCharacterInBattleList(GameObject deleteObj)
    {
        enemyCharacterInBattleList.Remove(deleteObj);
        if (enemyCharacterInBattleList.Count == 0)
        {
            ShowMessage("�G���S��");
            Debug.Log("�G���S��");
            battleIsOver = true;
            CharacterBattleScript characterBattleScript;
            foreach(var character in allCharacterList)
            {
                //�����L�����N�^�[�̐擪�ő�������HP��MP��ʏ�̃X�e�[�^�X�ɔ��f������
                characterBattleScript = character.GetComponent<CharacterBattleScript>();
                if(characterBattleScript.GetCharacterStatus() as AllyStatus != null)
                {
                    characterBattleScript.GetCharacterStatus().SetHp(characterBattleScript.GetHp());
                    characterBattleScript.GetCharacterStatus().SetMp(characterBattleScript.GetMp());
                    characterBattleScript.GetCharacterStatus().SetNumbness(characterBattleScript.IsNumbness());
                    characterBattleScript.GetCharacterStatus().SetPoisonState(characterBattleScript.IsPoison());
                }
            }
            //�������̌��ʕ\��
            battleResult.InitialProcessingOfVictoryResult(allCharacterList, allyCharacterInBattleList);
        }
    }

    //���b�Z�[�W�\��
    public void ShowMessage(string message)
    {
        if (messagePanelIns != null)
        {//�Â����b�Z�[�W�p�l�����폜
            Destroy(messagePanelIns);
        }
        messagePanelIns = Instantiate<GameObject>(messagePanel, battleUI);
        messagePanelIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = message;
    }
}
