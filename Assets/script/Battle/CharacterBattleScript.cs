using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//各キャラクターに付ける,戦闘中の動作,パラメータの管理
public class CharacterBattleScript : MonoBehaviour
{
   //戦闘中のキャラクターの状態
   public enum BattleState
    {
        Idle,
        DirectAttack,
        MagicAttack,
        Healing,
        UseHPRecoveryItem,
        UseMPRecoveryItem,
        UseNumbnessRecoveryItem,
        UsePoisonRecoveryItem,
        IncreaseAttackPowerMagic,
        IncreaseDefencePowerMagic,
        IncreaseMagicPowerMagic,
        IncreaseMagicStrengthMagic,
        NumbnessRecoveryMagic,
        PoisonnouRecoveryMagic,
        BurstFinish,
        Damage,
        Guard,
        Dead,
    }
    
    

    private BattleManager battleManager;
    private BattleStatusScript battleStatusScript;
    [SerializeField]
    private CharacterStatus characterStatus = null;
    private Animator animator;
    private BattleState battleState;

    //もとのステータスからコピー
    //HP
    private int hp = 0;
    //MP
    private int mp = 0;
    //Burst
    private int burst = 0;
    //バーストモード時のステ倍率
    [SerializeField]
    private float burstbuff=1.0f;
    //補助の素早さ
    private int auxiliaryAgility = 0;
    //補助の攻撃力
    private float auxiliaryPower = 1.0f;
    //補助の防御力
    private float auxiliaryStrikingStrength = 1.0f;
    //補助の魔法攻撃力
    private float auxiliaryMagicPower = 1.0f;
    //補助の魔法防御力
    private float auxiliaryMagicStrength = 1.0f;
    //痺れ状態か
    private bool isNumbness;
    //毒状態か
    private bool isPoison;
    //バーストモードか
    private bool isBurstMode;
    //今選択したスキル
    private Skill currentSkill;
    //今のターゲット
    private GameObject currentTarget;
    //今使用したアイテム
    private Item currentItem;
    //ターゲットのCharacterBattleScript
    private CharacterBattleScript targetCharacterBattleScript;
    //ターゲットのCharacterStatus
    private CharacterStatus targetCharacterStatus;

    //ターゲットのオブジェクトリスト(試用)
    [SerializeField]
    private List<GameObject> targetObjList= new List<GameObject>();
    //ターゲットのCharacterBattleScriptリスト(試用)
    private List<CharacterBattleScript> targetCharacterBattleScriptList=new List<CharacterBattleScript>();
    //ターゲットのCharacterStatusリスト(試用)
    private List<CharacterStatus> targetCharacterStatusList=new List<CharacterStatus>();
    

    //攻撃選択後のアニメーションが終了したかどうか
    private bool isDoneAnimation;
    //キャラクターが死んでいるかどうか
    private bool isDead;

    //物理攻撃力アップしているかどうか
    private bool isIncreasePower;
    //物理攻撃力アップ倍率
    private float increasePowerPoint=1.3f;
    //物理攻撃力アップしているターン
    private int numOfTurnsIncreasePower = 3;
    //物理攻撃力アップしてからのターン
    private int numOfTurnsSinceIncreasePower = 0;
    //物理防御力アップしているかどうか
    private bool isIncreaseStrikingStrength;
    //物理防御力アップ倍率
    private float increaseStrikingStrengthPoint=1.3f;
    //物理防御力アップしているターン
    private int numOfTurnsIncreaseStrikingStrength = 3;
    //物理防御力アップしてからのターン
    private int numOfTurnsSinceIncreaseStrikingStrength = 0;
    //魔法攻撃力アップしているかどうか
    private bool isIncreaseMagicPower;
    //魔法攻撃力アップ倍率
    private float increaseMagicPowerPoint=1.3f;
    //魔法攻撃力アップしているターン
    private int numOfTurnsIncreaseMagicPower = 3;
    //魔法攻撃力アップしてからのターン
    private int numOfTurnsSinceIncreaseMagicPower = 0;
    //魔法防御力アップしているかどうか
    private bool isIncreaseMagicStrength;
    //魔法防御力アップ倍率
    private float increaseMagicStrengthPoint=1.3f;
    //魔法防御力アップしているターン
    private int numOfTurnsIncreaseMagicStrength = 3;
    //魔法防御力アップしてからのターン
    private int numOfTurnsSinceIncreaseMagicStrength = 0;

    //効果ポイント表示スクリプト
    private EffectNumericalDisplayScript effectNumericalDisplayScript;
    private void Start()
    {
        animator = GetComponent<Animator>();
        //元データから設定
        hp = characterStatus.GetHp();
        mp = characterStatus.GetMp();
        burst = characterStatus.GetBurst();
        isNumbness = characterStatus.IsNumbnessState();
        isPoison = characterStatus.IsPoisonState();
        isBurstMode = false;
        //状態の設定
        battleState = BattleState.Idle;
        //コンポーネントの取得
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleStatusScript = GameObject.Find("BattleUI/StatusPanel").GetComponent<BattleStatusScript>();
        effectNumericalDisplayScript = battleManager.GetComponent<EffectNumericalDisplayScript>();
        //既に死んでいる場合は倒れている状態にする
        if (characterStatus.GetHp() <= 0)
        {
            //CrossFade:直接遷移
            animator.CrossFade("Dead", 0f, 0, 1f);
            isDead = true;
        }
    }
    private void Update()
    {
        
        //既に死んでいたら何もしない
        if (isDead)
        {
            return;
        }
        //自分のターンでなければ何もしない
        if (battleState == BattleState.Idle)
        {
            return;
        }
        //行動中のキャラが味方キャラの場合は移動できないようにスクリプトをオフにする+カメラを俯瞰に戻す
        if (characterStatus as AllyStatus != null)
        {
            GameObject.Find("CameraManager").GetComponent<CameraController>().ResetCamera(this.gameObject);
        }
        //アニメーションが終わっていなければ何もしない
        if (!isDoneAnimation)
        {
            return;
        }

        //選択したアニメーションによって処理を分ける
        if (battleState == BattleState.DirectAttack)
        {
            ShowEffectOnTheTarget();
            DirectAttack();
            //自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        else if (battleState == BattleState.MagicAttack)
        {
            ShowEffectOnTheTarget();
            MagicAttack();
            //自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();

        }
        else if (battleState == BattleState.Healing || battleState==BattleState.NumbnessRecoveryMagic || battleState == BattleState.PoisonnouRecoveryMagic)
        {
            ShowEffectOnTheTarget();
            UseMagic();
            //自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        else if (IsBuffState(battleState))
        {
            ShowEffectOnTheTarget();
            UseMagic();
            //自身の攻撃力をアップした場合はターン数をカウントしない
            if (targetObjList.Contains(this.gameObject))
            {
                switch(battleState){
                    case BattleState.IncreaseAttackPowerMagic:
                        CheckIncreaseStrikingStrength();
                        CheckIncreaseMagicPower();
                        CheckIncreaseMagicStrength();
                        break;
                    case BattleState.IncreaseDefencePowerMagic:
                        CheckIncreaseAttackPower();
                        CheckIncreaseMagicPower();
                        CheckIncreaseMagicStrength();
                        break;
                    case BattleState.IncreaseMagicPowerMagic:
                        CheckIncreaseAttackPower();
                        CheckIncreaseStrikingStrength();
                        CheckIncreaseMagicStrength();
                        break;
                    case BattleState.IncreaseMagicStrengthMagic:
                        CheckIncreaseAttackPower();
                        CheckIncreaseStrikingStrength();
                        CheckIncreaseMagicPower();
                        break;
                    default:
                        Debug.LogError(battleState + "は未定義のバトルステートです");
                        break;
                }
            }
            else
            {
                CheckIncreaseAttackPower();
                CheckIncreaseStrikingStrength();
                CheckIncreaseMagicPower();
                CheckIncreaseMagicStrength();
            }
        }
        else if (battleState == BattleState.BurstFinish)
        {//バーストフィニッシュを使う
            ShowEffectOnTheTarget();
            BurstFinish();
            //自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        else if(IsItemState(battleState))
        {
            //ShowEffectOnTheTarget();//Itemデータにエフェクトを登録した場合
            UseItem();
            //自分のターンが来たので上がったパラメータのチェック
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        Debug.Log("ターン終了");
        //ターゲットのリセット
        currentTarget = null;
        currentSkill = null;
        currentItem = null;
        targetCharacterBattleScript = null;
        targetCharacterStatus = null;
        battleState = BattleState.Idle;
        //ターゲットのキャラスクリプトおよびキャラステータスのリストを初期化
        targetObjList.Clear();
        targetCharacterBattleScriptList.Clear();
        targetCharacterStatusList.Clear();
        //自身の選択が終了したら次のキャラクターにする
        battleManager.ChangeNextChara();
        isDoneAnimation = false;
    }

    public CharacterStatus GetCharacterStatus()
    {
        return characterStatus;
    }

    public void SetHp(int hp)
    {
        this.hp = Mathf.Max(0, Mathf.Min(characterStatus.GetMaxHp(), hp));

        if (this.hp <= 0)
        {
            Dead();
        }
    }

    public int GetHp()
    {
        return hp;
    }

    public void SetMp(int mp)
    {
        this.mp = Mathf.Max(0, Mathf.Min(characterStatus.GetMaxMp(), mp));
    }

    public int GetMp()
    {
        return mp;
    }
    //バーストゲージを増加させるメソッド
    public void SetBurst(int burst)
    {
        if (isBurstMode)
        {
            if (burst > 10)
                this.burst += 10;
            else
                this.burst += burst;
        }
        else
        {
            if (this.burst < 100)
            {
                this.burst += burst;
                if (this.burst >= 100)
                    this.burst = 100;
            }
            else
            {
                this.burst += burst;
            }
        }

        if (this.burst > 120)
        {
            ChangeBurstMode(0);
            this.SetHp(0);
            this.Dead();
        }

    }
    //バーストモードへの遷移またはバーストモードからの遷移を行うメソッド(valueが0ならモード終了,valueが1ならモード開始の判定)
    public void ChangeBurstMode(int value)
    {
        if (value == 1)
        {
            //1.5秒待ってからカメラをキャラの後ろに移動する
            CameraController cameraController = GameObject.Find("CameraManager").GetComponent<CameraController>();
            if (!isBurstMode)
            {//バーストモードでない時
                //バーストゲージが100以上でバーストモードオン
                if (burst >= 100)
                {
                    //1.5秒待ってからカメラをキャラの後ろに移動する
                    StartCoroutine(burstmodewait(1.5f, cameraController));
                    //バーストエフェクトオン
                    BurstModeAnimation();
                    //バースト倍率を2倍に設定
                    SetBurstBuff(2);
                    //バーストモードトリガーをtrueに
                    SetIsBurstMode(true);
                }
                else
                {//バーストアニメーションを表示しないなら0.5f秒だけ待ってカメラを移動
                    StartCoroutine(burstmodewait(0.5f, cameraController));
                }
            }
            else
            {
                //バーストアニメーションを表示しないなら0.5f秒だけ待ってカメラを移動
                StartCoroutine(burstmodewait(0.5f, cameraController));
            }
        }
        else
        {
            //エフェクトオフ
            GameObject buttompos = this.transform.Find("ButtomPos").gameObject;
            foreach(Transform child in buttompos.transform)
            {
                Debug.Log("childname" + child.name);
                if (child.name.Contains("BurstMode"))
                    GameObject.Destroy(child.gameObject);
            }
            //バースト倍率を1倍に戻す
            SetBurstBuff(1);
            //バーストゲージを0に戻す
            burst=0;
            battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.Burstgage, burst);
            //バーストモードトリガーをfalseに
            SetIsBurstMode(false);
        }
    }
    public int GetBurst()
    {
        return burst;
    }
    
    public bool IsBurstMode()
    {
        return isBurstMode;
    }
    public void SetIsBurstMode(bool isBurstMode)
    {
        this.isBurstMode = isBurstMode;
    }
    public float GetBurstBuff()
    {
        return burstbuff;
    }
    public void SetBurstBuff(float burstbuff)
    {
        this.burstbuff = burstbuff;
    }
   
    public bool IsDoneAnimation()
    {
        return isDoneAnimation;
    }

    public int GetAuxiliaryAgility()
    {
        return auxiliaryAgility;
    }

    public float GetAuxiliaryPower()
    {
        return auxiliaryPower;
    }

    public float GetAuxiliaryStrikingStrength()
    {
        return auxiliaryStrikingStrength;
    }

    //　補正の素早さを設定
    public void SetAuxiliaryAgility(int value)
    {
        auxiliaryAgility = value;
    }

    //　補正の力を設定
    public void SetAuxiliaryPower(float value)
    {
        auxiliaryPower = value;
    }

    //　補正の打たれ強さを設定
    public void SetAuxiliaryStrikingStrength(float value)
    {
        auxiliaryStrikingStrength = value;
    }

    //補正の魔法攻撃力の取得および設定
    public void SetAuxiliaryMagicPower(float value)
    {
        auxiliaryMagicPower = value;
    }
    public float GetAuxiliaryMagicPower()
    {
        return auxiliaryMagicPower;
    }
    //補正の魔法防御力の取得および補正
    public void SetAuxiliaryMagicStrength(float value)
    {
        auxiliaryMagicStrength = value;
    }
    public float GetAuxiliaryMagicStrength()
    {
        return auxiliaryMagicStrength;
    }
    public bool IsNumbness()
    {
        return isNumbness;
    }

    public bool IsPoison()
    {
        return isPoison;
    }

    public void SetNumbness(bool isNumbness)
    {
        this.isNumbness = isNumbness;
    }

    public void SetPoison(bool isPoison)
    {
        this.isPoison = isPoison;
    }

    public bool IsIncreasePower()
    {
        return isIncreasePower;
    }

    public void SetIsIncreasePower(bool isIncreasePower)
    {
        this.isIncreasePower = isIncreasePower;
    }

    public bool IsIncreaseStrikingStrength()
    {
        return isIncreaseStrikingStrength;
    }

    public void SetIsIncreaseStrikingStrength(bool isIncreaseStrikingStrength)
    {
        this.isIncreaseStrikingStrength = isIncreaseStrikingStrength;
    }

    public bool IsIncreaseMagicPower()
    {
        return isIncreaseMagicPower;
    }
    public void SetIsIncreaseMagicPower(bool isIncreaseMagicPower)
    {
        this.isIncreaseMagicPower = isIncreaseMagicPower;
    }

    public bool IsIncreaseMagicStrength()
    {
        return isIncreaseMagicStrength;
    }

    public void SetIsIncreaseMagicStrength(bool isIncreaseMagicStrength)
    {
        this.isIncreaseMagicStrength = isIncreaseMagicStrength;
    }
    public void SetBattleState(BattleState state)
    {
        this.battleState = state;
    }

    public void SetIsDoneAnimation()
    {
        isDoneAnimation = true;
    }
    
    public void SettargetObjList(GameObject obj)
    {
        if (!targetObjList.Contains(obj))
        {
            targetObjList.Add(obj);
            Debug.Log("added:" + obj.name);
        }
        foreach(GameObject targetobj in targetObjList)
        {
            Debug.Log("targetObjList:" + targetobj.name);
        }
    }
    public void DeletetargetObjList(GameObject obj)
    {
        if (targetObjList.Contains(obj))
        {
            targetObjList.Remove(obj);
            Debug.Log("Deleted:" + obj.name);
        }
        foreach (GameObject targetobj in targetObjList)
        {
            Debug.Log("targetObjList:" + targetobj.name);
        }
    }

    public List<GameObject> GettargetObjList()
    {
        return targetObjList;
    }

    //選択肢から選んだモードを実行(アニメーション,UI更新)
    public void ChooseAttackOptions(BattleState selectOption,GameObject target,Skill skill=null,Item item = null)
    {
        //行動するキャラクターが味方ならマーカーを表示
        if(characterStatus as AllyStatus != null)
        {
            transform.Find("Marker/Image2").gameObject.SetActive(false);
        }
        //スキルやターゲットの情報をセット
        currentTarget = target;
        currentSkill = skill;
        targetCharacterBattleScript = target.GetComponent<CharacterBattleScript>();
        targetCharacterStatus = targetCharacterBattleScript.GetCharacterStatus();
                
        //選択したキャラクターの状態を設定
        battleState = selectOption;
        if(selectOption == BattleState.DirectAttack)
        {
            animator.SetTrigger("DirectAttack");
            battleManager.ShowMessage(gameObject.name + "は" + currentTarget.name + "に" + currentSkill.GetKanjiName() + "を行った");
        }
        else if(IsMagicState(selectOption)){
            animator.SetTrigger("MagicAttack");
            battleManager.ShowMessage(gameObject.name + "は" + currentTarget.name + "に" + currentSkill.GetKanjiName() + "を使った");
            //魔法使用者のMPを減らす
            SetMp(GetMp() - ((Magic)skill).GetAmountToUseMagicPoints());
            //使用者が味方キャラクターであればStatusPanelの更新
            if(GetCharacterStatus() as AllyStatus != null)
            {
                battleStatusScript.UpdateStatus(GetCharacterStatus(), BattleStatusScript.Status.MP, GetMp());
            }
            //使用者のスキルエフェクトを表示(魔法陣は足元の座標を取得)
            Instantiate(((Magic)skill).GetSkillUserEffect(),transform.Find("ButtomPos").transform.position, ((Magic)skill).GetSkillUserEffect().transform.rotation);
        }
        else if(IsItemState(selectOption))
        {
            currentItem = item;
            animator.SetTrigger("UseItem");
            battleManager.ShowMessage(gameObject.name + "は" + currentTarget.name + "に" + currentSkill.GetKanjiName() + "を行った");
        }
        else if (selectOption == BattleState.BurstFinish)
        {
            animator.SetTrigger("BurstFinish");
            battleManager.ShowMessage(gameObject.name + "は" + currentTarget.name + "に" + currentSkill.GetKanjiName() + "を行った");
            //使用者のスキルエフェクトを表示(剣のオーラの場合)
            Transform equipweapon = gameObject.GetComponent<EquipInstantiateWeapon>().GetEquipTransform();
            Instantiate(skill.GetSkillUserEffect(), equipweapon.position,equipweapon.rotation,equipweapon);
        }

    }

    //選択肢から選んだモードを実行(アニメーション,UI更新)
    public void ChooseAttackOptions(BattleState selectOption, Skill skill = null, Item item = null)
    {
        //行動するキャラクターが味方ならマーカーを表示
        if (characterStatus as AllyStatus != null)
        {
            transform.Find("Marker/Image2").gameObject.SetActive(false);
        }
        //スキルやターゲットの情報をセット
        currentSkill = skill;
        string objnames=null;
        foreach(GameObject obj in targetObjList)
        {
            objnames += obj.name + ", ";
            targetCharacterBattleScriptList.Add(obj.GetComponent<CharacterBattleScript>());
            targetCharacterStatusList.Add(targetCharacterBattleScriptList[targetCharacterBattleScriptList.Count - 1].GetCharacterStatus());
        }

        //選択したキャラクターの状態を設定
        battleState = selectOption;
        //アニメーションに移る前に技範囲を削除
        Destroy(this.gameObject.transform.Find("SkillAreaPos").GetChild(0).gameObject);
        if (selectOption == BattleState.DirectAttack)
        {
            animator.SetTrigger("DirectAttack");
            battleManager.ShowMessage(gameObject.name + "は" + objnames + "に" + currentSkill.GetKanjiName() + "を行った");
        }
        else if (IsMagicState(selectOption))
        {
            animator.SetTrigger("MagicAttack");
            battleManager.ShowMessage(gameObject.name + "は" + objnames + "に" + currentSkill.GetKanjiName() + "を行った");
            //魔法使用者のMPを減らす
            SetMp(GetMp() - ((Magic)skill).GetAmountToUseMagicPoints());
            //使用者が味方キャラクターであればStatusPanelの更新
            if (GetCharacterStatus() as AllyStatus != null)
            {
                battleStatusScript.UpdateStatus(GetCharacterStatus(), BattleStatusScript.Status.MP, GetMp());
            }
            //使用者のスキルエフェクトを表示(魔法陣は足元の座標を取得)
            Instantiate(((Magic)skill).GetSkillUserEffect(), transform.Find("ButtomPos").transform.position, ((Magic)skill).GetSkillUserEffect().transform.rotation);
        }
        else if (battleState == BattleState.BurstFinish)
        {
            animator.SetTrigger("BurstFinish");
            battleManager.ShowMessage(gameObject.name + "は" + objnames + "に" + currentSkill.GetKanjiName() + "を行った");
            //使用者のスキルエフェクトを表示(剣のオーラの場合)
            Transform equipweapon = gameObject.GetComponent<EquipInstantiateWeapon>().GetEquipTransform();
            Instantiate(skill.GetSkillUserEffect(), equipweapon.position, equipweapon.rotation, equipweapon);
        }
        else if (IsItemState(selectOption))
        {
            currentItem = item;
            animator.SetTrigger("UseItem");
            battleManager.ShowMessage(gameObject.name + "は" + objnames + "に" + currentSkill.GetKanjiName() + "を使った");
        }
    }
    //ターゲットエフェクトの表示(魔法攻撃or回復orバフ)
    public void ShowEffectOnTheTarget()
    {
        //EffectPosによってターゲットのどこにエフェクトを表示するか変更する.
        if (currentSkill.GetTargetPosType() == Skill.PosType.Center)
        {
            foreach(var obj in targetObjList)
            {
                Instantiate<GameObject>(currentSkill.GetSkillReceivingSideEffect(), obj.transform.Find("CenterPos").transform.position, currentSkill.GetSkillReceivingSideEffect().transform.rotation);
            }
        }
        
        else if (currentSkill.GetTargetPosType() == Skill.PosType.Buttom)
        {
            foreach (var obj in targetObjList)
            {
                Instantiate<GameObject>(currentSkill.GetSkillReceivingSideEffect(), obj.transform.Find("ButtomPos").transform.position, currentSkill.GetSkillReceivingSideEffect().transform.rotation);
            }
        }
     
    }
    //物理攻撃を受けた際の処理(ターゲット複数ver)
    public void DirectAttack()
    {
        for(int i=0; i<targetObjList.Count; i++)
        { 
            var targetAnimator = targetObjList[i].GetComponent<Animator>();
            targetAnimator.SetTrigger("Damage");
            
            var damage = 0;
            var burstup = 0;
            if(characterStatus as AllyStatus != null)
            {//味方の攻撃
                //攻撃相手の通常の防御力*補助倍率
                var targetDefencePower = targetCharacterStatusList[i].GetStrikingStrength() * targetCharacterBattleScriptList[i].GetAuxiliaryStrikingStrength();
                //物理ダメージ=((通常の攻撃力+武器値)*補助倍率*バーストモード倍率)-防御力)/2
                damage = (int)Mathf.Max(0, ((characterStatus.GetPower() + ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) * auxiliaryPower * burstbuff - targetDefencePower) / 2);
                battleManager.ShowMessage(targetObjList[i].name + "は" + damage + "のダメージを受けた");
                //敵のステータスのHPをセット
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
            }
            else
            {//敵の攻撃
                //攻撃相手の(通常の防御力+防具値)*補助倍率
                var targetDefencePower = (targetCharacterStatusList[i].GetStrikingStrength()+ (((AllyStatus)targetCharacterStatusList[i]).GetEquipArmor()?.GetAmount() ?? 0)) * targetCharacterBattleScriptList[i].GetAuxiliaryStrikingStrength();
                //物理ダメージ=((通常の攻撃力*補助倍率*バーストモード倍率)-防御力)/2
                damage = (int)Mathf.Max(0, (characterStatus.GetPower() * auxiliaryPower * burstbuff - targetDefencePower) / 2);
                battleManager.ShowMessage(targetObjList[i].name + "は" + damage + "のダメージを受けた");
                //バーストゲージ増加量=受けたダメージ/敵の最大HP*100
                burstup = (int)(((float)damage / (float)targetCharacterStatusList[i].GetMaxHp()) * 100);
                //味方のステータスのHPをセット
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
                //相手のステータスのバーストゲージをセット(既にバースト状態なら10%)
                targetCharacterBattleScriptList[i].SetBurst(burstup);
                //味方のステータスをUIに反映
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.Burstgage, targetCharacterBattleScriptList[i].GetBurst());
            }
            Debug.Log(gameObject.name + "は" + targetCharacterStatusList[i].GetCharacterName() + "に" + currentSkill.GetKanjiName() + "をして" + damage + "ダメージを与えた。");
            effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, targetObjList[i].transform, damage);
        }  
    }
    //魔法攻撃を受けた際の処理(データ更新)
    /*
    public void MagicAttack()
    {
        var targetAnimator = currentTarget.GetComponent<Animator>();
        targetAnimator.SetTrigger("Damage");
        var damage = 0;
        //ダメージ処理
        if (targetCharacterStatus as AllyStatus != null)
        {//味方への魔法攻撃
            var castedTargetStatus = (AllyStatus)targetCharacterBattleScript.GetCharacterStatus();
            //魔法防御=(防御力(魔法防御力)+防具値)*補助倍率*バーストモード倍率
            var targetDefencePower = (castedTargetStatus.GetMagicStrength() + (castedTargetStatus.GetEquipArmor()?.GetAmount() ?? 0))*targetCharacterBattleScript.GetAuxiliaryMagicStrength()*targetCharacterBattleScript.GetBurstBuff();
            damage = (int)Mathf.Max(0, (((Magic)currentSkill).GetMagicPower()*auxiliaryMagicPower - targetDefencePower)/2);
            battleManager.ShowMessage(currentTarget.name + "は" + damage + "のダメージを受けた");
            ////　相手のステータスのHPをセット
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
            //　ステータスUIを更新
            battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.HP, targetCharacterBattleScript.GetHp());
        }
        else if (targetCharacterStatus as EnemyStatus != null)
        {//敵への魔法攻撃
            var castedTargetStatus = (EnemyStatus)targetCharacterBattleScript.GetCharacterStatus();
            var targetDefencePower = castedTargetStatus.GetStrikingStrength()*targetCharacterBattleScript.GetAuxiliaryMagicStrength();
            damage = (int)Mathf.Max(0, ((((Magic)currentSkill).GetMagicPower()+ ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) *auxiliaryMagicPower*burstbuff- targetDefencePower)/2);
            battleManager.ShowMessage(currentTarget.name + "は" + damage + "のダメージを受けた");
            //　相手のステータスのHPをセット
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
        }
        else
        {
            Debug.LogError("魔法攻撃でターゲットが設定されていない");
        }

        Debug.Log(gameObject.name + "は" + currentTarget.name + "に" + currentSkill.GetKanjiName() + "をして" + damage + "ダメージを与えた。");
        effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, currentTarget.transform, damage);
    }*/
    public void MagicAttack()
    {
        for (int i = 0; i < targetObjList.Count; i++)
        {
            var targetAnimator = targetObjList[i].GetComponent<Animator>();
            targetAnimator.SetTrigger("Damage");
            int damage = 0;
            int burstup = 0;
            if(characterStatus as AllyStatus!= null)
            {
                var targetDefencePower = targetCharacterStatusList[i].GetStrikingStrength() * targetCharacterBattleScriptList[i].GetAuxiliaryMagicStrength();
                damage = (int)Mathf.Max(0, ((((Magic)currentSkill).GetMagicPower() + ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) * auxiliaryMagicPower * burstbuff - targetDefencePower) / 2);
                battleManager.ShowMessage(targetObjList[i].name + "は" + damage + "のダメージを受けた");
                //　相手のステータスのHPをセット
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
            }else
            {
                var targetDefencePower = (targetCharacterStatusList[i].GetStrikingStrength()+((AllyStatus)targetCharacterStatusList[i]).GetEquipArmor()?.GetAmount()??0) * targetCharacterBattleScriptList[i].GetAuxiliaryMagicStrength();
                damage = (int)Mathf.Max(0, ((((Magic)currentSkill).GetMagicPower() + ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) * auxiliaryMagicPower * burstbuff - targetDefencePower) / 2);
                battleManager.ShowMessage(targetObjList[i].name + "は" + damage + "のダメージを受けた");
                //バーストゲージ増加量=受けたダメージ/味方の最大HP*100
                burstup = (int)(((float)damage / (float)targetCharacterStatusList[i].GetMaxHp()) * 100);
                //味方のステータスのHPをセット
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
                //味方のステータスのバーストゲージをセット(既にバースト状態なら10%)
                targetCharacterBattleScriptList[i].SetBurst(burstup);
                //味方のステータスをUIに反映
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.Burstgage, targetCharacterBattleScriptList[i].GetBurst());
            }
            
            Debug.Log(gameObject.name + "は" + targetCharacterStatusList[i].GetCharacterName() + "に" + currentSkill.GetKanjiName() + "をして" + damage + "ダメージを与えた。");
            effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, targetObjList[i].transform, damage);
        }
    }
    //魔法攻撃以外の魔法を受けたとき(状態異常,攻撃力,防御力アップはUIに反映しない)
    /*
    public void UseMagic()
    {
        //バフ,ヒール系はターゲットのモーションなし＋効果音のみ
        var magicType = ((Magic)currentSkill).GetSkillType();
        if(magicType == Skill.Type.RecoveryMagic)
        {//回復量=スキルの威力+キャラの魔法攻撃力
            var recoveryPoint = ((Magic)currentSkill).GetMagicPower() + characterStatus.GetMagicPower();
            if(targetCharacterStatus as AllyStatus != null)
            {
                targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() + recoveryPoint);
                battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.HP, targetCharacterBattleScript.GetHp());
            }
            else
            {
                targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() + recoveryPoint);
            }
            Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "を" + recoveryPoint + "回復した。");
            battleManager.ShowMessage(currentTarget.name + "を" + recoveryPoint + "回復した");
            effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing, currentTarget.transform, recoveryPoint);
        }
        else if (magicType == Skill.Type.IncreaseAttackPowerMagic)
        {//物理攻撃力アップ倍率=1.3f
            increasePowerPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryPower(increasePowerPoint);
            targetCharacterBattleScript.SetIsIncreasePower(true);
            Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の力を" + increasePowerPoint + "増やした。");
            battleManager.ShowMessage(currentTarget.name + "の物理攻撃力を" + increasePowerPoint + "増やした");
        }
        else if (magicType == Skill.Type.IncreaseDefencePowerMagic)
        {   //物理防御力アップ倍率=1.3f
            increaseStrikingStrengthPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryStrikingStrength(increaseStrikingStrengthPoint);
            targetCharacterBattleScript.SetIsIncreaseStrikingStrength(true);
            Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の打たれ強さを" + increaseStrikingStrengthPoint + "増やした。");
            battleManager.ShowMessage(currentTarget.name + "の物理防御力を" +increaseStrikingStrengthPoint + "増やした");
        }
        else if (magicType == Skill.Type.IncreaseMagicPowerMagic)
        {
            //魔法攻撃力アップ倍率=1.3f
            increaseMagicPowerPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryMagicPower(increaseMagicPowerPoint);
            targetCharacterBattleScript.SetIsIncreaseMagicPower(true);
            Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の打たれ強さを" + increaseMagicPowerPoint + "増やした。");
            battleManager.ShowMessage(currentTarget.name + "の魔法攻撃力を" + increaseStrikingStrengthPoint + "増やした");
        }
        else if (magicType == Skill.Type.IncreaseMagicStrengthMagic)
        {
            //魔法防御力アップ倍率=1.3f
            increaseMagicStrengthPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryStrikingStrength(increaseMagicStrengthPoint);
            targetCharacterBattleScript.SetIsIncreaseMagicStrength(true);
            Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の打たれ強さを" + increaseMagicStrengthPoint + "増やした。");
            battleManager.ShowMessage(currentTarget.name + "の魔法防御力を" + increaseStrikingStrengthPoint + "増やした");
        }
        else if (magicType == Skill.Type.NumbnessRecoveryMagic)
        {
            targetCharacterStatus.SetNumbness(false);
            Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の痺れを消した");
            battleManager.ShowMessage(currentTarget.name + "の痺れを消した");
        }
        else if (magicType == Skill.Type.PoisonnouRecoveryMagic)
        {
            targetCharacterStatus.SetPoisonState(false);
            Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + currentTarget.name + "の毒を消した");
            battleManager.ShowMessage(currentTarget.name + "の毒を消した");
        }
    }
    */
    public void UseMagic()
    {
        //バフ,ヒール系はターゲットのモーションなし＋効果音のみ
        var magicType = ((Magic)currentSkill).GetSkillType();
        if (magicType == Skill.Type.RecoveryMagic)
        {//回復量=スキルの威力+キャラの魔法攻撃力
            var recoveryPoint = ((Magic)currentSkill).GetMagicPower() + characterStatus.GetMagicPower();
            for (int i = 0; i < targetObjList.Count; i++)
            {
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() + recoveryPoint);
                if(characterStatus as AllyStatus != null)
                {//味方の回復の時はUIにステータスを反映
                    battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                }
                Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "を" + recoveryPoint + "回復した。");
                battleManager.ShowMessage(targetObjList[i].name + "を" + recoveryPoint + "回復した");
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing, targetObjList[i].transform, recoveryPoint);
            }
        }
        else if (magicType == Skill.Type.IncreaseAttackPowerMagic)
        {//物理攻撃力アップ倍率=1.3f
            increasePowerPoint = 1.3f;
            for(int i=0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreasePower())
                {//物理攻撃力が上昇していない
                    targetCharacterBattleScriptList[i].SetAuxiliaryPower(increasePowerPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreasePower(true);
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の物理攻撃力を増やした。");
                    battleManager.ShowMessage(targetObjList[i].name + "の物理攻撃力を" + increasePowerPoint + "増やした");
                }
                else
                {
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の物理攻撃力を増やそうとした");
                    battleManager.ShowMessage(targetObjList[i].name + "の物理攻撃力は既に上がっている");
                }
            }
            
        }
        else if (magicType == Skill.Type.IncreaseDefencePowerMagic)
        {   //物理防御力アップ倍率=1.3f
            increaseStrikingStrengthPoint = 1.3f;
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreaseStrikingStrength())
                {//物理防御力が上昇していない
                    targetCharacterBattleScriptList[i].SetAuxiliaryStrikingStrength(increaseStrikingStrengthPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreaseStrikingStrength(true);
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の物理防御力を増やした。");
                    battleManager.ShowMessage(targetObjList[i].name + "の物理防御力を" + increaseStrikingStrengthPoint + "増やした");
                }
                else
                {
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の物理防御力を増やそうとした");
                    battleManager.ShowMessage(targetObjList[i].name + "の物理防御力は既に上がっている");
                }
            }
        }
        else if (magicType == Skill.Type.IncreaseMagicPowerMagic)
        {
            //魔法攻撃力アップ倍率=1.3f
            increaseMagicPowerPoint = 1.3f;
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreaseMagicPower())
                {//魔法攻撃力が上昇していない
                    targetCharacterBattleScriptList[i].SetAuxiliaryMagicPower(increaseMagicPowerPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreaseMagicPower(true);
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の魔法攻撃力を増やした。");
                    battleManager.ShowMessage(targetObjList[i].name + "の魔法攻撃力を" + increaseMagicPowerPoint + "増やした");
                }
                else
                {
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の魔法攻撃力を増やそうとした");
                    battleManager.ShowMessage(targetObjList[i].name + "の魔法攻撃力は既に上がっている");
                }
            }
        }
        else if (magicType == Skill.Type.IncreaseMagicStrengthMagic)
        {
            //魔法防御力アップ倍率=1.3f
            increaseMagicStrengthPoint = 1.3f;
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreaseMagicStrength())
                {//魔法防御力が上昇していない
                    targetCharacterBattleScriptList[i].SetAuxiliaryMagicStrength(increaseMagicStrengthPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreaseMagicStrength(true);
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の魔法防御力を増やした。");
                    battleManager.ShowMessage(targetObjList[i].name + "の魔法防御力を" +increaseMagicStrengthPoint + "増やした");
                }
                else
                {
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の魔法防御力を増やそうとした");
                    battleManager.ShowMessage(targetObjList[i].name + "の魔法防御力は既に上がっている");
                }
            }
        }
        else if (magicType == Skill.Type.NumbnessRecoveryMagic)
        {
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (targetCharacterBattleScriptList[i].IsNumbness())
                {//麻痺である
                    targetCharacterBattleScriptList[i].SetNumbness(false);
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "のしびれを消した");
                    battleManager.ShowMessage(targetObjList[i].name + "のしびれを消した" );
                }
                else
                {
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "のしびれを消そうとした");
                    battleManager.ShowMessage(targetObjList[i].name + "はしびれじょうたいではない");
                }
            }
        }
        else if (magicType == Skill.Type.PoisonnouRecoveryMagic)
        {
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (targetCharacterBattleScriptList[i].IsNumbness())
                {//毒である
                    targetCharacterBattleScriptList[i].SetPoison(false);
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の毒を消した");
                    battleManager.ShowMessage(targetObjList[i].name + "の毒を消した");
                }
                else
                {
                    Debug.Log(gameObject.name + "は" + ((Magic)currentSkill).GetKanjiName() + "を使って" + targetCharacterStatusList[i].GetCharacterName() + "の毒を消そうとした");
                    battleManager.ShowMessage(targetObjList[i].name + "は毒状態ではない");
                }
            }
        }
        
    }
    //アイテムを使ったときの処理(アイテムを使うのは味方だけと仮定)
    public void UseItem()
    {
        //キャラクターのアイテム数を減らす
        ((AllyStatus)characterStatus).SetItemNum(currentItem, ((AllyStatus)characterStatus).GetItemNum(currentItem) - 1);

        if(currentItem.GetItemType() == Item.Type.HPRecovery)
        {
            var recoveryPoint = currentItem.GetAmount();
            for(int i=0; i < targetObjList.Count; i++)
            {
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() + recoveryPoint);
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                Debug.Log(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" + targetObjList[i].name + "のHPを" + recoveryPoint + "回復した。");
                battleManager.ShowMessage(targetObjList[i].name + "のHPを" + recoveryPoint + "回復した");
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing,targetObjList[i].transform, recoveryPoint);
            }
        }
        else if(currentItem.GetItemType() == Item.Type.MPRecovery)
        {
            var recoveryPoint = currentItem.GetAmount();
            for(int i=0; i<targetObjList.Count; i++)
            {
                targetCharacterBattleScriptList[i].SetMp(targetCharacterBattleScriptList[i].GetMp() + recoveryPoint);
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.MP, targetCharacterBattleScriptList[i].GetMp());
                Debug.Log(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" + targetObjList[i].name + "のMPを" + recoveryPoint + "回復した。");
                battleManager.ShowMessage(targetObjList[i].name + "のMPを" + recoveryPoint + "回復した");
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing, targetObjList[i].transform, recoveryPoint);
            }
        }
        else if (currentItem.GetItemType() == Item.Type.NumbnessRecovery)
        {
            for(int i=0; i<targetObjList.Count; i++)
            {
                if (targetCharacterStatusList[i].IsNumbnessState())
                {
                    targetCharacterStatusList[i].SetNumbness(false);
                    Debug.Log(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" +targetObjList[i].name + "の痺れを消した。");
                    battleManager.ShowMessage(targetObjList[i].name + "の痺れを消した");
                }
                else
                {
                    battleManager.ShowMessage(targetObjList[i].name + "は痺れ状態ではありません");
                }
            }
            
        }
        else if (currentItem.GetItemType() == Item.Type.PoisonRecovery)
        {
            for (int i = 0; i < targetObjList.Count; i++)
            {
                if (targetCharacterStatusList[i].IsPoisonState())
                {
                    targetCharacterStatusList[i].SetPoisonState(false);
                    Debug.Log(gameObject.name + "は" + currentItem.GetKanjiName() + "を使って" + targetObjList[i].name + "の毒を消した。");
                    battleManager.ShowMessage(targetObjList[i].name + "の毒を消した");
                }
                else
                {
                    battleManager.ShowMessage(targetObjList[i].name + "は毒状態ではありません");
                }
            }
        }
        //アイテム数が0になったkらItemDictionaryからそのアイテムを削除
        if (((AllyStatus)characterStatus).GetItemNum(currentItem) == 0)
        {
            ((AllyStatus)characterStatus).GetItemDictionary().Remove(currentItem);
        }
    }
    //バーストフィニッシュを受けた際の処理(データ更新),味方だけが使うと仮定
    public void BurstFinish()
    {
        for(int i = 0; i < targetObjList.Count; i++)
        {
            var targetAnimator = targetObjList[i].GetComponent<Animator>();
            targetAnimator.SetTrigger("Damage");
            var totaldamage = 0;
            for (int j = 0; j < 3; j++)
            {
                //バーストフィニッシュは3回敵に攻撃するとする(スキルにその情報を持たせるのもアリ)
                var damage = 0;
                //攻撃相手の通常の防御力*補助倍率
                var targetDefencePower = targetCharacterStatusList[i].GetStrikingStrength() * targetCharacterBattleScriptList[i].GetAuxiliaryStrikingStrength();
                //物理ダメージ=((通常の攻撃力+武器値)*補助倍率*バーストモード倍率)-防御力)/2
                damage = (int)Mathf.Max(0, ((characterStatus.GetPower() + ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) * auxiliaryPower * burstbuff - targetDefencePower) / 2);
                totaldamage += damage;
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, targetObjList[i].transform, damage);
            }
            //敵のステータスのHPをセット
            targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - totaldamage);
            Debug.Log(gameObject.name + "は" + targetObjList[i].name + "に" + currentSkill.GetKanjiName() + "をして" + totaldamage + "ダメージを与えた。");
            battleManager.ShowMessage(targetObjList[i].name + "は" + totaldamage + "のダメージを受けた");
        }
        //バーストモード終了
        ChangeBurstMode(0);
    }
    //防御(エフェクトを表示するためガードスキルのデータを引数として取得する)
    public void Guard(Skill skill)
    {
        //行動するキャラクターが味方ならマーカーを非表示に
        if (characterStatus as AllyStatus != null)
        {
            transform.Find("Marker/Image2").gameObject.SetActive(false);
        }
        //自分のターンが来たので上がったパラメータのチェック
        CheckIncreaseAttackPower();
        CheckIncreaseStrikingStrength();
        CheckIncreaseMagicStrength();
        CheckIncreaseMagicStrength();
        animator.SetBool("Guard", true);
        //ガードエフェクトを表示(キャラの真ん中から少し前にエフェクト表示)
        Instantiate(skill.GetSkillUserEffect(), transform.Find("CenterPos").transform.position+(transform.rotation*Vector3.forward.normalized), skill.GetSkillUserEffect().transform.rotation);
        //ガード中は防御力1.3倍
        SetAuxiliaryStrikingStrength(GetAuxiliaryStrikingStrength()*1.3f);
        SetAuxiliaryMagicStrength(GetAuxiliaryMagicStrength() * 1.3f);
        battleManager.ShowMessage(gameObject.name + "は防御を行った");
    }
    //防御を解除
    public void UnlockGuard()
    {
        animator.SetBool("Guard", false);
        SetAuxiliaryStrikingStrength(GetAuxiliaryStrikingStrength()/1.3f);
        SetAuxiliaryMagicStrength(GetAuxiliaryMagicStrength() / 1.3f);
    }
    //死んだときに実行する処理
    public void Dead()
    {
        animator.SetTrigger("Dead");
        Debug.Log(gameObject.name + "は倒れた");
        battleManager.ShowMessage(gameObject.name + "は倒れた");
        battleManager.DeleteAllCharacterInBattleList(this.gameObject);
        if(GetCharacterStatus() as AllyStatus != null)
        {
            battleStatusScript.UpdateStatus(GetCharacterStatus(), BattleStatusScript.Status.HP, GetHp());
            battleManager.DeleteAllyCharacterInBattleList(this.gameObject);
        }else if(GetCharacterStatus() as EnemyStatus != null)
        {
            battleManager.DeleteEnemyCharacterInBattleList(this.gameObject);
            this.gameObject.SetActive(false);
        }

        isDead = true;
    }
    //物理攻撃補正をチェック
    public void CheckIncreaseAttackPower()
    {
        //自分のターンが来たときに攻撃力が上がっていたら経過ターン数をふやす
        if (IsIncreasePower())
        {
            numOfTurnsSinceIncreasePower++;
            if (numOfTurnsSinceIncreasePower >= numOfTurnsIncreasePower)
            {
                numOfTurnsSinceIncreasePower = 0;
                SetAuxiliaryPower(1);
                SetIsIncreasePower(false);
                Debug.Log(gameObject.name + "の攻撃力が元に戻った");
               battleManager.ShowMessage(gameObject.name + "の攻撃力が元に戻った");
            }
        }
    }
    //防御補正をチェック
    public void CheckIncreaseStrikingStrength()
    {
        if (IsIncreaseStrikingStrength())
        {
            numOfTurnsSinceIncreaseStrikingStrength++;
            if (numOfTurnsSinceIncreaseStrikingStrength >= numOfTurnsIncreaseStrikingStrength)
            {
                numOfTurnsSinceIncreaseStrikingStrength = 0;
                SetAuxiliaryStrikingStrength(1);
                SetIsIncreaseStrikingStrength(false);
                Debug.Log(gameObject.name + "の防御力が元に戻った");
                battleManager.ShowMessage(gameObject.name + "の防御力が元に戻った");
            }
        }
    }

    //魔法攻撃補正をチェック
    public void CheckIncreaseMagicPower()
    {
        //自分のターンが来たときに攻撃力が上がっていたら経過ターン数をふやす
        if (IsIncreaseMagicPower())
        {
            numOfTurnsSinceIncreaseMagicPower++;
            if (numOfTurnsSinceIncreaseMagicPower >= numOfTurnsIncreaseMagicPower)
            {
                numOfTurnsSinceIncreaseMagicPower = 0;
                SetAuxiliaryMagicPower(1);
                SetIsIncreaseMagicPower(false);
                Debug.Log(gameObject.name + "の魔法攻撃力が元に戻った");
                battleManager.ShowMessage(gameObject.name + "の魔法攻撃力が元に戻った");
            }
        }
    }
    //魔法防御補正をチェック
    public void CheckIncreaseMagicStrength()
    {
        if (IsIncreaseMagicStrength())
        {
            numOfTurnsSinceIncreaseMagicStrength++;
            if (numOfTurnsSinceIncreaseMagicStrength >= numOfTurnsIncreaseMagicStrength)
            {
                numOfTurnsSinceIncreaseMagicStrength = 0;
                SetAuxiliaryMagicStrength(1);
                SetIsIncreaseMagicStrength(false);
                Debug.Log(gameObject.name + "の魔法防御力が元に戻った");
                battleManager.ShowMessage(gameObject.name + "の魔法防御力が元に戻った");
            }
        }
    }
    //BattleStateが魔法系のいずれかならtrueを返す
    public bool IsMagicState(BattleState battleState)
    {
        if (battleState == BattleState.MagicAttack || battleState == BattleState.Healing 
            || battleState == BattleState.IncreaseAttackPowerMagic || battleState == BattleState.IncreaseDefencePowerMagic 
            || battleState==BattleState.IncreaseMagicPowerMagic || battleState == BattleState.IncreaseMagicStrengthMagic
            || battleState == BattleState.NumbnessRecoveryMagic || battleState == BattleState.PoisonnouRecoveryMagic 
             )
            return true;
        else
            return false;
    }
    //BattleStateが魔法系かつバフ系のいずれかならtrueを返す
    public bool IsBuffState(BattleState battleState)
    {
        if (battleState == BattleState.IncreaseAttackPowerMagic
            || battleState == BattleState.IncreaseDefencePowerMagic
            || battleState == BattleState.IncreaseMagicPowerMagic
            || battleState == BattleState.IncreaseMagicStrengthMagic)
            return true;
        else
            return false;
    }
    //BattleStateがアイテム系のいずれかならtrueを返す
    public bool IsItemState(BattleState battleState)
    {
        if (battleState == BattleState.UseHPRecoveryItem
            || battleState == BattleState.UseMPRecoveryItem
            || battleState == BattleState.UseNumbnessRecoveryItem
            || battleState == BattleState.UsePoisonRecoveryItem)
            return true;
        else
            return false;
    }
    //バーストモード遷移時のアニメーション
    public void BurstModeAnimation()
    {
        CameraController cameraController = GameObject.Find("CameraManager").GetComponent<CameraController>();
        cameraController.SetCameraToCharaFront(this.gameObject);
        animator.SetTrigger("BurstMode");
        GameObject ins=Instantiate<GameObject>(characterStatus.GetBurstModeEffect(), this.transform.Find("ButtomPos").transform.position, characterStatus.GetBurstModeEffect().transform.rotation, this.transform.Find("ButtomPos").transform);
        ins.name = "BurstModeEffect";
    }
    //バーストモードアニメーションが終わる+1秒まで待つコルーチン
    IEnumerator burstmodewait(float value,CameraController cameraController)
    {
        yield return new WaitForSeconds(value);
        cameraController.SetCameraToCharaBack(this.gameObject);
    }
}
