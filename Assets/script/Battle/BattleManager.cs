using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{//バトル全体を管理するスクリプト(ターン、セクションを管理)
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

    //　戦闘データ
    [SerializeField]
    private BattleData battleData = null;
    //　キャラクターのベース位置
    [SerializeField]
    private Transform battleBasePosition;
    //敵パーティーのリスト(敵パーティー決定用)
    [SerializeField]
    private EnemyPartyStatusList enemyPartyStatusList = null;
    //全キャラクター
    private List<GameObject> allCharacterList = new List<GameObject>();
    //現在戦闘に参加している全キャラクター
    private List<GameObject> allCharacterInBattleList = new List<GameObject>();
    //現在戦闘に参加している味方キャラクター
    private List<GameObject> allyCharacterInBattleList = new List<GameObject>();
    //現在戦闘に参加している敵キャラクター
    private List<GameObject> enemyCharacterInBattleList = new List<GameObject>();
    //現在の攻撃の順番
    private int currentAttackOrder;
    //現在攻撃をしようとしている人が選択中
    private bool isChoosing;
    //戦闘が開始しているかどうか
    private bool isStartBattle;
    //戦闘シーンの最初の攻撃が始まるまでの待機時間
    [SerializeField]
    private float firstWaitingTime = 3f;
    //戦闘シーンのキャラ移行時の間の時間
    [SerializeField]
    private float timeToNextCharacter = 1f;
    //待ち時間
    private float waitTime;
    //戦闘シーンの最初の攻撃が始まるまでの経過時間
    private float elapsedTime;
    //戦闘が終了したかどうか
    private bool battleIsOver;
    //現在のコマンド
    private CommandMode currentCommand;
    //味方パーティーのコマンドパネル
    [SerializeField]
    private Transform commandPanel = null;
    //戦闘用キャラクター選択ボタンプレハブ
    [SerializeField]
    private GameObject battleCharacterButton = null;
    //SelectCharacterPanel
    [SerializeField]
    private Transform selectCharacterPanel = null;
    //魔法やアイテム選択パネル
    [SerializeField]
    private Transform magicOrItemPanel = null;
    //魔法やアイテム選択パネルのContent
    private Transform magicOrItemPanelContent = null;
    //BattleItemPanelButtonプレハブ
    [SerializeField]
    private GameObject battleItemPanelButton = null;
    //BattleMagicPanelButton
    [SerializeField]
    private GameObject battleMagicPanelButton = null;
    //最後に選択していたゲームオブジェクトをスタック
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();
    //MagicOrItemPanelでどの番号のボタンから上にスクロールするか
    [SerializeField]
    private int scrollDownButtonNum = 8;
    //MagicOrItemPanelでどの番号のボタンから下にスクロールするか
    [SerializeField]
    private int scrollUpButtonNum = 10;
    //ScrollManager
    private ScrollManager scrollManager;
    //メッセージパネルプレハブ
    [SerializeField]
    private GameObject messagePanel;
    //BattleUI
    [SerializeField]
    private Transform battleUI;
    //メッセージパネルインスタンス
    private GameObject messagePanelIns;
    //戦闘結果のデータ
    [SerializeField]
    private BattleResult battleResult;
    //カメラマネージャー
    [SerializeField]
    private CameraController cameraController;
    // Start is called before the first frame update
    void Start()
    {
        ShowMessage("戦闘開始");
        //自分で記述,敵―パーティーをランダムに決定
        if (Random.Range(0, 1) > 0.5f)
        {
            battleData.SetEnemyPartyStatus(enemyPartyStatusList.GetPartyMembersList().Find(enemyPartyStatus => enemyPartyStatus.GetPartyName() == "Tree1"));
        }
        else
        {
            battleData.SetEnemyPartyStatus(enemyPartyStatusList.GetPartyMembersList().Find(enemyPartyStatus => enemyPartyStatus.GetPartyName() == "Tree2"));
        }
        //
        //キャラクターインスタンスの親
        Transform charactersParent = new GameObject("Characters").transform;
        //キャラクターを配置するTransform
        Transform characterTransfom;
        //同じ名前の敵がいた場合の処理に使うリスト
        List<string> enemyNameList = new List<string>();

        GameObject ins;
        CharacterBattleScript characterBattleScript;
        string characterName;

        magicOrItemPanelContent = magicOrItemPanel.Find("Mask/Content");
        scrollManager = magicOrItemPanelContent.GetComponent<ScrollManager>();

        //味方パーティーのプレハブをインスタンス化
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
            Debug.LogError("敵バーティ―データが保存されていません");
        }
        //敵バーティ―のプレハブをインスタンス化
        for(int i=0; i<battleData.GetEnemyPartyStatus().GetEnemyGameObjectList().Count; i++)
        {
            characterTransfom = battleBasePosition.Find("EnemyPos" + i).transform;
            ins = Instantiate<GameObject>(battleData.GetEnemyPartyStatus().GetEnemyGameObjectList()[i], characterTransfom.position, characterTransfom.rotation, charactersParent);
            //既に同じ敵が存在したら文字を付加する
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
        //キャラクターリストをキャラクターの素早さの高い順に並べ替え
        allCharacterList = allCharacterList.OrderByDescending(character => character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetAgility()).ToList<GameObject>();
        //現在の戦闘
        allCharacterInBattleList = allCharacterList.ToList<GameObject>();
        //確認のため並べ替えたリストを表示
        foreach(var character in allCharacterInBattleList)
        {
            Debug.Log(character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetCharacterName() + ":" + character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetAgility());
        }
        //戦闘前の待ち時間を設定
        waitTime = firstWaitingTime;
        //ランダム値のシードの設定
        Random.InitState((int)Time.time);
    }

    void Update()
    {
        //戦闘が終了していたらこれ以降何もしない
        if (battleIsOver)
        {
            return;
        }
        //　選択解除された時（マウスでUI外をクリックした）は現在のモードによって無理やり選択させる
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
        //戦闘開始
        if (isStartBattle)
        {
            //現在のキャラクターの攻撃が終わっている
            if (!isChoosing)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime < waitTime)
                {//待ち時間中
                    return;
                }
                elapsedTime = 0f;
                isChoosing = true;

                //キャラクターの攻撃の選択に移る
                Debug.Log(currentAttackOrder);
                MakeAttackChoise(allCharacterInBattleList[currentAttackOrder]);
                //次のキャラクターのターンにする
                currentAttackOrder++;
                //全員攻撃が終わったら最初から(1ターン経過)
                if (currentAttackOrder >= allCharacterInBattleList.Count)
                {
                    currentAttackOrder = 0;
                }
            }
            else
            {//行動選択中
                //キャンセルボタンを押したときの処理
                if (Input.GetButtonDown("Cancel"))
                {
                    if (currentCommand == CommandMode.SelectDirectAttacker)
                    {
                        //キャラクター選択ボタンがあればすべて削除→行動選択に戻る
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }

                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        commandPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //攻撃範囲を削除(行動選択中のキャラの番号は(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%キャラ数)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjListを初期化
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectCommand;
                    }
                    else if (currentCommand == CommandMode.SelectMagic)
                    {
                        // magicOrItemPanelにボタンがあれば全て削除→行動選択に戻る
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
                        // selectCharacterPanelにボタンがあれば全て削除→魔法選択に戻る
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //攻撃範囲を削除(行動選択中のキャラの番号は(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%キャラ数)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjListを初期化
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectMagic;
                    }
                    else if (currentCommand == CommandMode.SelectUseMagicOnAlliesTarget)
                    {
                        // selectCharacterPanelにボタンがあれば全て削除→魔法選択に戻る
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //攻撃範囲を削除(行動選択中のキャラの番号は(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%キャラ数)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjListを初期化
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectMagic;
                    }
                    else if (currentCommand == CommandMode.SelectBurstFinish)
                    {
                        // magicOrItemPanelにボタンがあれば全て削除→行動選択に戻る
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
                        // selectCharacterPanelにボタンがあれば全て削除→アイテム選択に戻る
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //攻撃範囲を削除(行動選択中のキャラの番号は(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%キャラ数)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjListを初期化
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectBurstFinish;
                    }
                    else if (currentCommand == CommandMode.SelectItem)
                    {
                        // magicOrItemPanelにボタンがあれば全て削除→行動選択に戻る
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
                        // selectCharacterPanelにボタンがあれば全て削除→アイテム選択に戻る
                        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                        {
                            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                        }
                        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
                        selectCharacterPanel.gameObject.SetActive(false);
                        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                        //攻撃範囲を削除(行動選択中のキャラの番号は(allCharacterInBattleList.Count+currentAttackOrder+currentAttackOrder-1)%キャラ数)
                        Transform skillareapos = allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].transform.Find("SkillAreaPos");
                        Destroy(skillareapos.GetChild(0).gameObject);
                        //targetObjListを初期化
                        allCharacterInBattleList[(allCharacterInBattleList.Count + currentAttackOrder + currentAttackOrder - 1) % allCharacterInBattleList.Count].GetComponent<CharacterBattleScript>().GettargetObjList().Clear();
                        currentCommand = CommandMode.SelectItem;
                    }
                    
                }
            }
        }else
        { 
            //待ち時間中
            Debug.Log("経過時間:" + elapsedTime);
            //戦闘前の待機
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waitTime)
            {   //二回目以降はキャラ間の待ち時間を設定
                waitTime = timeToNextCharacter;
                //最初のキャラクターの待ち時間は0にするためにあらかじめ条件をクリアさせておく
                elapsedTime = timeToNextCharacter;
                isStartBattle = true;
            }
        }
    }

    //キャラクターの攻撃の選択処理
    public void MakeAttackChoise(GameObject character)
    {
        CharacterStatus characterStatus = character.GetComponent<CharacterBattleScript>().GetCharacterStatus();
        //EnemyStatusキャストできる場合は敵の攻撃処理
        if(characterStatus as EnemyStatus != null)
        {
            Debug.Log(character.gameObject.name + "の攻撃");
           StartCoroutine(EnemyAttack(character));
        }
        else
        {//味方の攻撃
            Debug.Log(characterStatus.GetCharacterName() + "の攻撃");
            //ターンの初めにバーストモードに移行できるかチェック+カメラをキャラにセット
            character.GetComponent<CharacterBattleScript>().ChangeBurstMode(1);
            AllyAttack(character);
        }
    }
    //味方の攻撃処理(コマンドパネル生成+ボタンにメソッドをアタッチ)
    public void AllyAttack(GameObject character)
    {
        //行動するキャラクターのマーカーを表示
        character.transform.Find("Marker/Image2").gameObject.SetActive(true);
        currentCommand = CommandMode.SelectCommand;
        //前のキャラの行動処理の際に使ったパネル、ボタンの削除
        //キャラクター選択ボタンがあればすべて削除
        for(int i = selectCharacterPanel.transform.childCount - 1;i>= 0; i--)
        {
            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
        }
        //魔法やアイテムパネルの子要素のContentにボタンがあればすべて削除
        for(int i = magicOrItemPanelContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(magicOrItemPanelContent.transform.GetChild(i).gameObject);
        }
        
        commandPanel.GetComponent<CanvasGroup>().interactable = true;
        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;

        //キャラクターがガード状態であればガードを解く
        if (character.GetComponent<Animator>().GetBool("Guard"))
        {
            character.GetComponent<CharacterBattleScript>().UnlockGuard();
        }

        //キャラクターの名前を表示
        commandPanel.Find("CharacterName/Text").GetComponent<Text>().text = character.name;

        var characterSkill = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
        //持っているスキルに応じてコマンドボタンを表示
        if (characterSkill.Exists(skill => skill.GetSkillType() == Skill.Type.DirectAttack))
        {//攻撃
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
            {//スペシャルボタンはバーストモードでないと使えない
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
    //直接攻撃相手の選択(スキル範囲内の敵すべてに攻撃するver)
    public void SelectDirectAttacker(GameObject attackCharacter)
    {
        currentCommand = CommandMode.SelectDirectAttacker;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        Transform skillareapos = attackCharacter.transform.Find("SkillAreaPos").transform;
        //攻撃するキャラのDirectAttackスキルを取得する
        var characterSkill = attackCharacter.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
        Skill directAttack = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack);
        //技範囲の作成
        GameObject ins = Instantiate<GameObject>(directAttack.GetSkillArea(), skillareapos.position, attackCharacter.transform.rotation, skillareapos);
        //ターゲットのタグはenemy
        ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("enemy");
        //敵キャラのボタンを作成+押したら直接攻撃を実行するように設定
        GameObject battleCharacterButtonIns;
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "戦闘開始";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => DirectAttackDemo(attackCharacter));

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }
    //直接攻撃
    public void DirectAttack(GameObject attackCharacter, GameObject attackTarget)
    {
        //攻撃するキャラのDirectAttackスキルを取得する
        var characterSkill = attackCharacter.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
        Skill directAttack = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack);
        attackCharacter.GetComponent<CharacterBattleScript>().ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, attackTarget, directAttack);
        commandPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //直接攻撃(複数攻撃対象)
    public void DirectAttackDemo(GameObject attackCharacter)
    {
        //攻撃するキャラのDirectAttackスキルを取得する
        var characterBattleScript = attackCharacter.GetComponent<CharacterBattleScript>();
        var characterSkill = characterBattleScript.GetCharacterStatus().GetSkillList();
        Skill directAttack = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack);
        characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, directAttack);
        commandPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //防御
    public void Guard(GameObject guardCharacter)
    {
        guardCharacter.GetComponent<CharacterBattleScript>().Guard(guardCharacter.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList().Find(skill=>skill.GetSkillType()==Skill.Type.Guard));
        commandPanel.gameObject.SetActive(false);
        //カメラを俯瞰に戻す
        cameraController.ResetCamera();
        ChangeNextChara();
    }
    //魔法スキルのボタンを生成+押したときの機能をセット
    public void SelectMagic(GameObject character)
    {
        currentCommand = CommandMode.SelectMagic;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleMagicPanelButtonIns;
        var skillList = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();

        //MagicOrItemPanelのスクロール値の初期化
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

                //指定した番号のアイテムパネルボタンにアイテムスクロール用のスクリプトを取り付ける
                if (battleMagicPanelButtonNum != 0 &&(battleMagicPanelButtonNum%scrollDownButtonNum==0 || battleMagicPanelButtonNum%(scrollDownButtonNum+1)==0))
                {
                    //アイテムスクロールスクリプトを取り付けて設定値のセット
                    battleMagicPanelButtonIns.AddComponent<ScrollDownScript>();
                }else if(battleMagicPanelButtonNum!=0 && (battleMagicPanelButtonNum % scrollUpButtonNum == 0 || battleMagicPanelButtonNum % (scrollUpButtonNum + 1) == 0))
                {
                    battleMagicPanelButtonIns.AddComponent<ScrollUpScript>();
                }

                //MPが足りないときはボタンを押しても何もせず魔法の名前を暗くする
                if (character.GetComponent<CharacterBattleScript>().GetMp() < ((Magic)skill).GetAmountToUseMagicPoints())
                {
                    battleMagicPanelButtonIns.transform.Find("MagicName").GetComponent<Text>().color = new Color(0.4f, 0.4f, 0.4f);
                }
                else
                {
                    battleMagicPanelButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectUseMagicTarget(character, skill));
                }
                //ボタンの番号を足す
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
    
    //魔法を使う相手の選択(複数対象)
    public void SelectUseMagicTarget(GameObject attackCharacter,Skill skill)
    {
        currentCommand = CommandMode.SelectUseMagicOnAlliesTarget;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        Transform skillareapos = attackCharacter.transform.Find("SkillAreaPos").transform;
        //技範囲の作成
        GameObject ins=Instantiate<GameObject>(skill.GetSkillArea(), skillareapos.position, attackCharacter.transform.rotation, skillareapos);
        if (skill.GetSkillType() == Skill.Type.MagicAttack)
        {//ターゲットが敵の魔法
            ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("enemy");
        }
        else
        {//ターゲットが味方の魔法
            ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("Player");
        }
        //敵キャラのボタンを作成+押したら行動を実行するように設定
        GameObject battleCharacterButtonIns;
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "戦闘開始";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseMagicDemo(attackCharacter,skill));

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);

    }
    //魔法を使う
    public void UseMagic(GameObject user,GameObject targetCharacter, Skill skill)
    {
        CharacterBattleScript.BattleState battleState = CharacterBattleScript.BattleState.Idle;
        //魔法を使う相手のCharacterBattleScriptを取得しておく
        var targetCharacterBattleScript = targetCharacter.GetComponent<CharacterBattleScript>();
        if (skill.GetSkillType() == Skill.Type.MagicAttack)
        {
            battleState = CharacterBattleScript.BattleState.MagicAttack;
        }
        else if (skill.GetSkillType() == Skill.Type.RecoveryMagic)
        {
            if (targetCharacterBattleScript.GetHp() == targetCharacterBattleScript.GetCharacterStatus().GetMaxMp())
            {
                ShowMessage(targetCharacter.name + "は全快です");
                Debug.Log(targetCharacter.name + "は全快です");
                return;
            }
            battleState = CharacterBattleScript.BattleState.Healing;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseAttackPowerMagic)
        {
            if (targetCharacterBattleScript.IsIncreasePower())
            {
                ShowMessage(targetCharacter.name + "は既に攻撃力をあげています");
                Debug.Log(targetCharacter.name + "は既に攻撃力をあげています");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseAttackPowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic)
        {
            if (targetCharacterBattleScript.IsIncreaseStrikingStrength())
            {
                ShowMessage("既に防御力を上げています。");
                Debug.Log("既に防御力を上げています。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseDefencePowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseMagicPowerMagic)
        {
            if (targetCharacterBattleScript.IsIncreaseMagicPower())
            {
                ShowMessage("既に魔法攻撃力を上げています。");
                Debug.Log("既に魔法攻撃力を上げています。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseMagicPowerMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.IncreaseMagicStrengthMagic)
        {
            if (targetCharacterBattleScript.IsIncreaseMagicStrength())
            {
                ShowMessage("既に魔法防御力を上げています。");
                Debug.Log("既に魔法防御力を上げています。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.IncreaseMagicStrengthMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic)
        {
            if (!targetCharacterBattleScript.IsNumbness())
            {
                ShowMessage(targetCharacter.name + "は痺れ状態ではありません。");
                Debug.Log(targetCharacter.name + "は痺れ状態ではありません。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.NumbnessRecoveryMagic;
        }
        else if (skill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic)
        {
            if (!targetCharacterBattleScript.IsPoison())
            {
                ShowMessage(targetCharacter.name + "は毒状態ではありません。");
                Debug.Log(targetCharacter.name + "は毒状態ではありません。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.PoisonnouRecoveryMagic;
        }
        user.GetComponent<CharacterBattleScript>().ChooseAttackOptions(battleState, targetCharacter, skill);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //魔法(複数攻撃対象)
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
    //バーストフィニッシュスのボタンを生成+押したときの機能をセット
    public void SelectBurstFinish(GameObject character)
    {
        currentCommand = CommandMode.SelectBurstFinish;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleSpecialPanelButtonIns;
        var skillList = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();

        //MagicOrItemPanelのスクロール値の初期化
        scrollManager.Reset();
        int battleSpecialPanelButtonNum = 0;

        foreach (var skill in skillList)
        {
            if (skill.GetSkillType() == Skill.Type.BurstFinish)
            {
                battleSpecialPanelButtonIns = Instantiate<GameObject>(battleMagicPanelButton, magicOrItemPanelContent);
                battleSpecialPanelButtonIns.transform.Find("MagicName").GetComponent<Text>().text = skill.GetKanjiName();
                //battleSpecialPanelButtonIns.transform.Find("AmountToUseMagicPoints").GetComponent<Text>().text = ((Magic)skill).GetAmountToUseMagicPoints().ToString();

                //指定した番号のアイテムパネルボタンにアイテムスクロール用のスクリプトを取り付ける
                if (battleSpecialPanelButtonNum != 0 && (battleSpecialPanelButtonNum % scrollDownButtonNum == 0 || battleSpecialPanelButtonNum % (scrollDownButtonNum + 1) == 0))
                {
                    //アイテムスクロールスクリプトを取り付けて設定値のセット
                    battleSpecialPanelButtonIns.AddComponent<ScrollDownScript>();
                }
                else if (battleSpecialPanelButtonNum != 0 && (battleSpecialPanelButtonNum % scrollUpButtonNum == 0 || battleSpecialPanelButtonNum % (scrollUpButtonNum + 1) == 0))
                {
                    battleSpecialPanelButtonIns.AddComponent<ScrollUpScript>();
                }
                //ボタン処理を実装する
                battleSpecialPanelButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectUseBurstFinishTarget(character, skill));
                //ボタンの番号を足す
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
    //バーストフィニッシュを使う相手の選択
    public void SelectUseBurstFinishTarget(GameObject user, Skill skill)
    {
        currentCommand = CommandMode.SelectBurstFinishTarget;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleCharacterButtonIns;

        Transform skillareapos = user.transform.Find("SkillAreaPos").transform;
        //スキル範囲の作成
        GameObject ins = Instantiate<GameObject>(skill.GetSkillArea(), skillareapos.position, user.transform.rotation, skillareapos);
        //ターゲットのタグはenemy
        ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("enemy");
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "戦闘開始";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseBurstFinish(user, skill));
        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }
    //バーストフィニッシュを使う
    public void UseBurstFinish(GameObject user, Skill skill)
    {
        CharacterBattleScript.BattleState battleState = CharacterBattleScript.BattleState.Idle;
        battleState = CharacterBattleScript.BattleState.BurstFinish;
        user.GetComponent<CharacterBattleScript>().ChooseAttackOptions(battleState, skill);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        selectCharacterPanel.gameObject.SetActive(false);
    }
    //使用するアイテムの選択
    public void SelectItem(GameObject character)
    {
        var itemDictionary = ((AllyStatus)character.GetComponent<CharacterBattleScript>().GetCharacterStatus()).GetItemDictionary();

        //MagicOrItemPanelのスクロール値の初期化
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
                //アイテムボタンの名前変更
                battleItemPanelButtonIns.transform.Find("ItemName").GetComponent<Text>().text = item.GetKanjiName();
                //アイテムの数を表示
                battleItemPanelButtonIns.transform.Find("Num").GetComponent<Text>().text = ((AllyStatus)character.GetComponent<CharacterBattleScript>().GetCharacterStatus()).GetItemNum(item).ToString();
                battleItemPanelButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectItemTarget(character, item));

                //　指定した番号のアイテムパネルボタンにアイテムスクロール用スクリプトを取り付ける
                if (battleItemPanelButtonNum != 0
                    && (battleItemPanelButtonNum % scrollDownButtonNum == 0
                    || battleItemPanelButtonNum % (scrollDownButtonNum + 1) == 0)
                    )
                {
                    //　アイテムスクロールスクリプトの取り付けて設定値のセット
                    battleItemPanelButtonIns.AddComponent<ScrollDownScript>();
                }
                else if (battleItemPanelButtonNum != 0
                  && (battleItemPanelButtonNum % scrollUpButtonNum == 0
                  || battleItemPanelButtonNum % (scrollUpButtonNum + 1) == 0)
                  )
                {
                    battleItemPanelButtonIns.AddComponent<ScrollUpScript>();
                }
                //　ボタン番号を足す
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
            ShowMessage("使えるアイテムがありません。");
            Debug.Log("使えるアイテムがありません。");
        }
    }

    //アイテムを使用する相手を選択(味方キャラクターに対してアイテムを使用する場合のみ)
    public void SelectItemTarget(GameObject user, Item item)
    {
        currentCommand = CommandMode.SelectRecoveryItemTarget;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        Transform skillareapos = user.transform.Find("SkillAreaPos").transform;
        //アイテム効果範囲の作成
        GameObject ins = Instantiate<GameObject>(item.GetItemArea(), skillareapos.position, user.transform.rotation, skillareapos);
        //ターゲットのタグはPlayer
        ins.transform.GetChild(0).GetComponent<BattleRegistTarget>().SetSkillTargetTag("Player");
        GameObject battleCharacterButtonIns;
        battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
        battleCharacterButtonIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = "戦闘開始";
        battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItem(user, item));
        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }
    //アイテム使用
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

    //　逃げる
    public void GetAway(GameObject character)
    {

        //アイコンを非表示
        character.transform.Find("Marker/Image2").gameObject.SetActive(false);
        var randomValue = Random.value;
        if (0f <= randomValue && randomValue <= 0.2f)
        {
            ShowMessage("逃げるのに成功した。");
            Debug.Log("逃げるのに成功した。");
            battleIsOver = true;
            commandPanel.gameObject.SetActive(false);
            //　戦闘終了
            battleResult.InitialProcessingOfRanAwayResult();
        }
        else
        {
            ShowMessage("逃げるのに失敗した。");
            Debug.Log("逃げるのに失敗した。");
            commandPanel.gameObject.SetActive(false);
            ChangeNextChara();
        }
    }
    //敵の移動処理
    IEnumerator EnemyAutoMove(GameObject character,Skill skill)
    {
        
        Transform skillareapos = character.transform.Find("SkillAreaPos").transform;
        //技範囲の作成
        Debug.Log(skill.GetSkillType());
        GameObject ins = Instantiate<GameObject>(skill.GetSkillArea(), skillareapos.position, character.transform.rotation, skillareapos);
        GameObject closesttarget = null;
        Vector3 v= new Vector3(0.0f,0.0f,0.0f);
        float distance = 1000.0f;
        if (skill.GetSkillType()== Skill.Type.DirectAttack || skill.GetSkillType() == Skill.Type.MagicAttack)
        {//ターゲットはプレイヤー
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
        {//ターゲットは敵
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
        //一番近いターゲットの向きを向く
        character.transform.LookAt(closesttarget.transform);
        //スクリプトの取得
        CharacterBattleScript characterBattleScript = character.GetComponent<CharacterBattleScript>();
        Rigidbody rb= character.GetComponent<Rigidbody>();

        //ターゲットリストが空でなくなるまでキャラを動かす
        yield return StartCoroutine(movingwait(characterBattleScript, rb, v));
        //動かし終わったらスキル範囲は削除
        Destroy(ins);
    }
    //一定秒数ごとに移動
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
    //敵の攻撃処理
    public IEnumerator EnemyAttack(GameObject character)
    {
        CharacterBattleScript characterBattleScript = character.GetComponent<CharacterBattleScript>();
        CharacterStatus characterStatus = characterBattleScript.GetCharacterStatus();

        if (characterStatus.GetSkillList().Count <= 0)
        {
            Debug.Log("Skill 0");
           yield return null;
        }
        //敵がガード状態であればガードを解く
        if (character.GetComponent<Animator>().GetBool("Guard"))
        {
            character.GetComponent<CharacterBattleScript>().UnlockGuard();
        }

        //敵の行動アルゴリズム
        int randomValue = (int)(Random.value * characterStatus.GetSkillList().Count);
        var nowSkill = characterStatus.GetSkillList()[0];

        //逃げるまたはガードの時は移動を行わない
        if (!(nowSkill.GetSkillType() == Skill.Type.GetAway || nowSkill.GetSkillType() == Skill.Type.Guard))
        {
            Coroutine coroutine = StartCoroutine(EnemyAutoMove(character, nowSkill));
            yield return coroutine;
        }
        

        //スキルのタイプ別に処理を記述する( GetAway,PoisonnouRecoveryMagic,NumbnessRecoveryMagic未定義)
        if (nowSkill.GetSkillType() == Skill.Type.DirectAttack)
        {
            characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack,nowSkill);
            Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
        }else if (nowSkill.GetSkillType() == Skill.Type.MagicAttack)
        {
            if (characterBattleScript.GetMp() >= ((Magic)nowSkill).GetAmountToUseMagicPoints()){
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.MagicAttack,  nowSkill);
                Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            }
            else
            {
                ShowMessage("MPが足りない!");
                Debug.Log("MPが足りない!");
                //MPが足りない場合は直接攻撃を行う
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, characterStatus.GetSkillList().Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack));
                Debug.Log(character.name + "は攻撃を行った");
            }
        }else if(nowSkill.GetSkillType() == Skill.Type.Guard)
        {
            characterBattleScript.Guard(nowSkill);
            //Guardアニメはboolなのでアニメーショ遷移させたらすぐに次のキャラクターに移行させる
            ChangeNextChara();
            ShowMessage(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
        }else if (nowSkill.GetSkillType() == Skill.Type.RecoveryMagic)
        {//回復相手の決定
            if (characterBattleScript.GetMp() >= ((Magic)nowSkill).GetAmountToUseMagicPoints())
            {
                //回復相手のCharacterBattleScript
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.Healing, nowSkill);
                Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            }
            else
            {
                ShowMessage("MPが足りない!");
                Debug.Log("MPが足りない!");
                Debug.Log(character.name + "は何もできなかった");
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
                //バフ相手のCharacterBattleScript
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
                    Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            }
            else
            {
                ShowMessage("MPが足りない!");
                Debug.Log("MPが足りない!");
                Debug.Log(character.name + "は何もできなかった");
            }
        }
        else
        {
            Debug.Log("未定義スキルタイプ:" + nowSkill.GetSkillType());
        }
    }
    //次のキャラクターに移行
    public void ChangeNextChara()
    {
        isChoosing = false;
    }
    //死んだキャラクターを全キャラリストから削除
    public void DeleteAllCharacterInBattleList(GameObject deleteObj)
    {
        var deleteObjNum = allCharacterInBattleList.IndexOf(deleteObj);
        allCharacterInBattleList.Remove(deleteObj);
        if (deleteObjNum < currentAttackOrder)
        {
            currentAttackOrder--;
        }
        //全員攻撃が終わったら最初から
        if (currentAttackOrder >= allCharacterInBattleList.Count)
        {
            currentAttackOrder = 0;
        }
    }
    //味方キャラが死んだときに味方キャラリストから削除
    public void DeleteAllyCharacterInBattleList(GameObject deleteObj)
    {
        allyCharacterInBattleList.Remove(deleteObj);
        if (allyCharacterInBattleList.Count == 0)
        {
            ShowMessage("味方が全滅");
            Debug.Log("味方が全滅");
            battleIsOver = true;
            CharacterBattleScript characterBattleScript;
            foreach (var character in allCharacterList)
            {
                //　味方キャラクターの戦闘で増減したHPとMPを通常のステータスに反映させる
                characterBattleScript = character.GetComponent<CharacterBattleScript>();
                if (characterBattleScript.GetCharacterStatus() as AllyStatus != null)
                {
                    characterBattleScript.GetCharacterStatus().SetHp(characterBattleScript.GetHp());
                    characterBattleScript.GetCharacterStatus().SetMp(characterBattleScript.GetMp());
                    characterBattleScript.GetCharacterStatus().SetNumbness(characterBattleScript.IsNumbness());
                    characterBattleScript.GetCharacterStatus().SetPoisonState(characterBattleScript.IsPoison());
                }
            }
            //　敗戦時の結果表示
            battleResult.InitialProcessingOfDefeatResult();
        }
    }
    //敵キャラが死んだときに敵キャラリストから削除
    public void DeleteEnemyCharacterInBattleList(GameObject deleteObj)
    {
        enemyCharacterInBattleList.Remove(deleteObj);
        if (enemyCharacterInBattleList.Count == 0)
        {
            ShowMessage("敵が全滅");
            Debug.Log("敵が全滅");
            battleIsOver = true;
            CharacterBattleScript characterBattleScript;
            foreach(var character in allCharacterList)
            {
                //味方キャラクターの先頭で増減したHPとMPを通常のステータスに反映させる
                characterBattleScript = character.GetComponent<CharacterBattleScript>();
                if(characterBattleScript.GetCharacterStatus() as AllyStatus != null)
                {
                    characterBattleScript.GetCharacterStatus().SetHp(characterBattleScript.GetHp());
                    characterBattleScript.GetCharacterStatus().SetMp(characterBattleScript.GetMp());
                    characterBattleScript.GetCharacterStatus().SetNumbness(characterBattleScript.IsNumbness());
                    characterBattleScript.GetCharacterStatus().SetPoisonState(characterBattleScript.IsPoison());
                }
            }
            //勝利時の結果表示
            battleResult.InitialProcessingOfVictoryResult(allCharacterList, allyCharacterInBattleList);
        }
    }

    //メッセージ表示
    public void ShowMessage(string message)
    {
        if (messagePanelIns != null)
        {//古いメッセージパネルを削除
            Destroy(messagePanelIns);
        }
        messagePanelIns = Instantiate<GameObject>(messagePanel, battleUI);
        messagePanelIns.transform.Find("Text (Legacy)").GetComponent<Text>().text = message;
    }
}
