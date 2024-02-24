using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�e�L�����N�^�[�ɕt����,�퓬���̓���,�p�����[�^�̊Ǘ�
public class CharacterBattleScript : MonoBehaviour
{
   //�퓬���̃L�����N�^�[�̏��
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

    //���Ƃ̃X�e�[�^�X����R�s�[
    //HP
    private int hp = 0;
    //MP
    private int mp = 0;
    //Burst
    private int burst = 0;
    //�o�[�X�g���[�h���̃X�e�{��
    [SerializeField]
    private float burstbuff=1.0f;
    //�⏕�̑f����
    private int auxiliaryAgility = 0;
    //�⏕�̍U����
    private float auxiliaryPower = 1.0f;
    //�⏕�̖h���
    private float auxiliaryStrikingStrength = 1.0f;
    //�⏕�̖��@�U����
    private float auxiliaryMagicPower = 1.0f;
    //�⏕�̖��@�h���
    private float auxiliaryMagicStrength = 1.0f;
    //Ⴢ��Ԃ�
    private bool isNumbness;
    //�ŏ�Ԃ�
    private bool isPoison;
    //�o�[�X�g���[�h��
    private bool isBurstMode;
    //���I�������X�L��
    private Skill currentSkill;
    //���̃^�[�Q�b�g
    private GameObject currentTarget;
    //���g�p�����A�C�e��
    private Item currentItem;
    //�^�[�Q�b�g��CharacterBattleScript
    private CharacterBattleScript targetCharacterBattleScript;
    //�^�[�Q�b�g��CharacterStatus
    private CharacterStatus targetCharacterStatus;

    //�^�[�Q�b�g�̃I�u�W�F�N�g���X�g(���p)
    [SerializeField]
    private List<GameObject> targetObjList= new List<GameObject>();
    //�^�[�Q�b�g��CharacterBattleScript���X�g(���p)
    private List<CharacterBattleScript> targetCharacterBattleScriptList=new List<CharacterBattleScript>();
    //�^�[�Q�b�g��CharacterStatus���X�g(���p)
    private List<CharacterStatus> targetCharacterStatusList=new List<CharacterStatus>();
    

    //�U���I����̃A�j���[�V�������I���������ǂ���
    private bool isDoneAnimation;
    //�L�����N�^�[������ł��邩�ǂ���
    private bool isDead;

    //�����U���̓A�b�v���Ă��邩�ǂ���
    private bool isIncreasePower;
    //�����U���̓A�b�v�{��
    private float increasePowerPoint=1.3f;
    //�����U���̓A�b�v���Ă���^�[��
    private int numOfTurnsIncreasePower = 3;
    //�����U���̓A�b�v���Ă���̃^�[��
    private int numOfTurnsSinceIncreasePower = 0;
    //�����h��̓A�b�v���Ă��邩�ǂ���
    private bool isIncreaseStrikingStrength;
    //�����h��̓A�b�v�{��
    private float increaseStrikingStrengthPoint=1.3f;
    //�����h��̓A�b�v���Ă���^�[��
    private int numOfTurnsIncreaseStrikingStrength = 3;
    //�����h��̓A�b�v���Ă���̃^�[��
    private int numOfTurnsSinceIncreaseStrikingStrength = 0;
    //���@�U���̓A�b�v���Ă��邩�ǂ���
    private bool isIncreaseMagicPower;
    //���@�U���̓A�b�v�{��
    private float increaseMagicPowerPoint=1.3f;
    //���@�U���̓A�b�v���Ă���^�[��
    private int numOfTurnsIncreaseMagicPower = 3;
    //���@�U���̓A�b�v���Ă���̃^�[��
    private int numOfTurnsSinceIncreaseMagicPower = 0;
    //���@�h��̓A�b�v���Ă��邩�ǂ���
    private bool isIncreaseMagicStrength;
    //���@�h��̓A�b�v�{��
    private float increaseMagicStrengthPoint=1.3f;
    //���@�h��̓A�b�v���Ă���^�[��
    private int numOfTurnsIncreaseMagicStrength = 3;
    //���@�h��̓A�b�v���Ă���̃^�[��
    private int numOfTurnsSinceIncreaseMagicStrength = 0;

    //���ʃ|�C���g�\���X�N���v�g
    private EffectNumericalDisplayScript effectNumericalDisplayScript;
    private void Start()
    {
        animator = GetComponent<Animator>();
        //���f�[�^����ݒ�
        hp = characterStatus.GetHp();
        mp = characterStatus.GetMp();
        burst = characterStatus.GetBurst();
        isNumbness = characterStatus.IsNumbnessState();
        isPoison = characterStatus.IsPoisonState();
        isBurstMode = false;
        //��Ԃ̐ݒ�
        battleState = BattleState.Idle;
        //�R���|�[�l���g�̎擾
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleStatusScript = GameObject.Find("BattleUI/StatusPanel").GetComponent<BattleStatusScript>();
        effectNumericalDisplayScript = battleManager.GetComponent<EffectNumericalDisplayScript>();
        //���Ɏ���ł���ꍇ�͓|��Ă����Ԃɂ���
        if (characterStatus.GetHp() <= 0)
        {
            //CrossFade:���ڑJ��
            animator.CrossFade("Dead", 0f, 0, 1f);
            isDead = true;
        }
    }
    private void Update()
    {
        
        //���Ɏ���ł����牽�����Ȃ�
        if (isDead)
        {
            return;
        }
        //�����̃^�[���łȂ���Ή������Ȃ�
        if (battleState == BattleState.Idle)
        {
            return;
        }
        //�s�����̃L�����������L�����̏ꍇ�͈ړ��ł��Ȃ��悤�ɃX�N���v�g���I�t�ɂ���+�J��������Ղɖ߂�
        if (characterStatus as AllyStatus != null)
        {
            GameObject.Find("CameraManager").GetComponent<CameraController>().ResetCamera(this.gameObject);
        }
        //�A�j���[�V�������I����Ă��Ȃ���Ή������Ȃ�
        if (!isDoneAnimation)
        {
            return;
        }

        //�I�������A�j���[�V�����ɂ���ď����𕪂���
        if (battleState == BattleState.DirectAttack)
        {
            ShowEffectOnTheTarget();
            DirectAttack();
            //�����̃^�[���������̂ŏオ�����p�����[�^�̃`�F�b�N
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        else if (battleState == BattleState.MagicAttack)
        {
            ShowEffectOnTheTarget();
            MagicAttack();
            //�����̃^�[���������̂ŏオ�����p�����[�^�̃`�F�b�N
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();

        }
        else if (battleState == BattleState.Healing || battleState==BattleState.NumbnessRecoveryMagic || battleState == BattleState.PoisonnouRecoveryMagic)
        {
            ShowEffectOnTheTarget();
            UseMagic();
            //�����̃^�[���������̂ŏオ�����p�����[�^�̃`�F�b�N
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        else if (IsBuffState(battleState))
        {
            ShowEffectOnTheTarget();
            UseMagic();
            //���g�̍U���͂��A�b�v�����ꍇ�̓^�[�������J�E���g���Ȃ�
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
                        Debug.LogError(battleState + "�͖���`�̃o�g���X�e�[�g�ł�");
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
        {//�o�[�X�g�t�B�j�b�V�����g��
            ShowEffectOnTheTarget();
            BurstFinish();
            //�����̃^�[���������̂ŏオ�����p�����[�^�̃`�F�b�N
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        else if(IsItemState(battleState))
        {
            //ShowEffectOnTheTarget();//Item�f�[�^�ɃG�t�F�N�g��o�^�����ꍇ
            UseItem();
            //�����̃^�[���������̂ŏオ�����p�����[�^�̃`�F�b�N
            CheckIncreaseAttackPower();
            CheckIncreaseStrikingStrength();
            CheckIncreaseMagicPower();
            CheckIncreaseMagicStrength();
        }
        Debug.Log("�^�[���I��");
        //�^�[�Q�b�g�̃��Z�b�g
        currentTarget = null;
        currentSkill = null;
        currentItem = null;
        targetCharacterBattleScript = null;
        targetCharacterStatus = null;
        battleState = BattleState.Idle;
        //�^�[�Q�b�g�̃L�����X�N���v�g����уL�����X�e�[�^�X�̃��X�g��������
        targetObjList.Clear();
        targetCharacterBattleScriptList.Clear();
        targetCharacterStatusList.Clear();
        //���g�̑I�����I�������玟�̃L�����N�^�[�ɂ���
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
    //�o�[�X�g�Q�[�W�𑝉������郁�\�b�h
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
    //�o�[�X�g���[�h�ւ̑J�ڂ܂��̓o�[�X�g���[�h����̑J�ڂ��s�����\�b�h(value��0�Ȃ烂�[�h�I��,value��1�Ȃ烂�[�h�J�n�̔���)
    public void ChangeBurstMode(int value)
    {
        if (value == 1)
        {
            //1.5�b�҂��Ă���J�������L�����̌��Ɉړ�����
            CameraController cameraController = GameObject.Find("CameraManager").GetComponent<CameraController>();
            if (!isBurstMode)
            {//�o�[�X�g���[�h�łȂ���
                //�o�[�X�g�Q�[�W��100�ȏ�Ńo�[�X�g���[�h�I��
                if (burst >= 100)
                {
                    //1.5�b�҂��Ă���J�������L�����̌��Ɉړ�����
                    StartCoroutine(burstmodewait(1.5f, cameraController));
                    //�o�[�X�g�G�t�F�N�g�I��
                    BurstModeAnimation();
                    //�o�[�X�g�{����2�{�ɐݒ�
                    SetBurstBuff(2);
                    //�o�[�X�g���[�h�g���K�[��true��
                    SetIsBurstMode(true);
                }
                else
                {//�o�[�X�g�A�j���[�V������\�����Ȃ��Ȃ�0.5f�b�����҂��ăJ�������ړ�
                    StartCoroutine(burstmodewait(0.5f, cameraController));
                }
            }
            else
            {
                //�o�[�X�g�A�j���[�V������\�����Ȃ��Ȃ�0.5f�b�����҂��ăJ�������ړ�
                StartCoroutine(burstmodewait(0.5f, cameraController));
            }
        }
        else
        {
            //�G�t�F�N�g�I�t
            GameObject buttompos = this.transform.Find("ButtomPos").gameObject;
            foreach(Transform child in buttompos.transform)
            {
                Debug.Log("childname" + child.name);
                if (child.name.Contains("BurstMode"))
                    GameObject.Destroy(child.gameObject);
            }
            //�o�[�X�g�{����1�{�ɖ߂�
            SetBurstBuff(1);
            //�o�[�X�g�Q�[�W��0�ɖ߂�
            burst=0;
            battleStatusScript.UpdateStatus(characterStatus, BattleStatusScript.Status.Burstgage, burst);
            //�o�[�X�g���[�h�g���K�[��false��
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

    //�@�␳�̑f������ݒ�
    public void SetAuxiliaryAgility(int value)
    {
        auxiliaryAgility = value;
    }

    //�@�␳�̗͂�ݒ�
    public void SetAuxiliaryPower(float value)
    {
        auxiliaryPower = value;
    }

    //�@�␳�̑ł��ꋭ����ݒ�
    public void SetAuxiliaryStrikingStrength(float value)
    {
        auxiliaryStrikingStrength = value;
    }

    //�␳�̖��@�U���͂̎擾����ѐݒ�
    public void SetAuxiliaryMagicPower(float value)
    {
        auxiliaryMagicPower = value;
    }
    public float GetAuxiliaryMagicPower()
    {
        return auxiliaryMagicPower;
    }
    //�␳�̖��@�h��͂̎擾����ѕ␳
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

    //�I��������I�񂾃��[�h�����s(�A�j���[�V����,UI�X�V)
    public void ChooseAttackOptions(BattleState selectOption,GameObject target,Skill skill=null,Item item = null)
    {
        //�s������L�����N�^�[�������Ȃ�}�[�J�[��\��
        if(characterStatus as AllyStatus != null)
        {
            transform.Find("Marker/Image2").gameObject.SetActive(false);
        }
        //�X�L����^�[�Q�b�g�̏����Z�b�g
        currentTarget = target;
        currentSkill = skill;
        targetCharacterBattleScript = target.GetComponent<CharacterBattleScript>();
        targetCharacterStatus = targetCharacterBattleScript.GetCharacterStatus();
                
        //�I�������L�����N�^�[�̏�Ԃ�ݒ�
        battleState = selectOption;
        if(selectOption == BattleState.DirectAttack)
        {
            animator.SetTrigger("DirectAttack");
            battleManager.ShowMessage(gameObject.name + "��" + currentTarget.name + "��" + currentSkill.GetKanjiName() + "���s����");
        }
        else if(IsMagicState(selectOption)){
            animator.SetTrigger("MagicAttack");
            battleManager.ShowMessage(gameObject.name + "��" + currentTarget.name + "��" + currentSkill.GetKanjiName() + "���g����");
            //���@�g�p�҂�MP�����炷
            SetMp(GetMp() - ((Magic)skill).GetAmountToUseMagicPoints());
            //�g�p�҂������L�����N�^�[�ł����StatusPanel�̍X�V
            if(GetCharacterStatus() as AllyStatus != null)
            {
                battleStatusScript.UpdateStatus(GetCharacterStatus(), BattleStatusScript.Status.MP, GetMp());
            }
            //�g�p�҂̃X�L���G�t�F�N�g��\��(���@�w�͑����̍��W���擾)
            Instantiate(((Magic)skill).GetSkillUserEffect(),transform.Find("ButtomPos").transform.position, ((Magic)skill).GetSkillUserEffect().transform.rotation);
        }
        else if(IsItemState(selectOption))
        {
            currentItem = item;
            animator.SetTrigger("UseItem");
            battleManager.ShowMessage(gameObject.name + "��" + currentTarget.name + "��" + currentSkill.GetKanjiName() + "���s����");
        }
        else if (selectOption == BattleState.BurstFinish)
        {
            animator.SetTrigger("BurstFinish");
            battleManager.ShowMessage(gameObject.name + "��" + currentTarget.name + "��" + currentSkill.GetKanjiName() + "���s����");
            //�g�p�҂̃X�L���G�t�F�N�g��\��(���̃I�[���̏ꍇ)
            Transform equipweapon = gameObject.GetComponent<EquipInstantiateWeapon>().GetEquipTransform();
            Instantiate(skill.GetSkillUserEffect(), equipweapon.position,equipweapon.rotation,equipweapon);
        }

    }

    //�I��������I�񂾃��[�h�����s(�A�j���[�V����,UI�X�V)
    public void ChooseAttackOptions(BattleState selectOption, Skill skill = null, Item item = null)
    {
        //�s������L�����N�^�[�������Ȃ�}�[�J�[��\��
        if (characterStatus as AllyStatus != null)
        {
            transform.Find("Marker/Image2").gameObject.SetActive(false);
        }
        //�X�L����^�[�Q�b�g�̏����Z�b�g
        currentSkill = skill;
        string objnames=null;
        foreach(GameObject obj in targetObjList)
        {
            objnames += obj.name + ", ";
            targetCharacterBattleScriptList.Add(obj.GetComponent<CharacterBattleScript>());
            targetCharacterStatusList.Add(targetCharacterBattleScriptList[targetCharacterBattleScriptList.Count - 1].GetCharacterStatus());
        }

        //�I�������L�����N�^�[�̏�Ԃ�ݒ�
        battleState = selectOption;
        //�A�j���[�V�����Ɉڂ�O�ɋZ�͈͂��폜
        Destroy(this.gameObject.transform.Find("SkillAreaPos").GetChild(0).gameObject);
        if (selectOption == BattleState.DirectAttack)
        {
            animator.SetTrigger("DirectAttack");
            battleManager.ShowMessage(gameObject.name + "��" + objnames + "��" + currentSkill.GetKanjiName() + "���s����");
        }
        else if (IsMagicState(selectOption))
        {
            animator.SetTrigger("MagicAttack");
            battleManager.ShowMessage(gameObject.name + "��" + objnames + "��" + currentSkill.GetKanjiName() + "���s����");
            //���@�g�p�҂�MP�����炷
            SetMp(GetMp() - ((Magic)skill).GetAmountToUseMagicPoints());
            //�g�p�҂������L�����N�^�[�ł����StatusPanel�̍X�V
            if (GetCharacterStatus() as AllyStatus != null)
            {
                battleStatusScript.UpdateStatus(GetCharacterStatus(), BattleStatusScript.Status.MP, GetMp());
            }
            //�g�p�҂̃X�L���G�t�F�N�g��\��(���@�w�͑����̍��W���擾)
            Instantiate(((Magic)skill).GetSkillUserEffect(), transform.Find("ButtomPos").transform.position, ((Magic)skill).GetSkillUserEffect().transform.rotation);
        }
        else if (battleState == BattleState.BurstFinish)
        {
            animator.SetTrigger("BurstFinish");
            battleManager.ShowMessage(gameObject.name + "��" + objnames + "��" + currentSkill.GetKanjiName() + "���s����");
            //�g�p�҂̃X�L���G�t�F�N�g��\��(���̃I�[���̏ꍇ)
            Transform equipweapon = gameObject.GetComponent<EquipInstantiateWeapon>().GetEquipTransform();
            Instantiate(skill.GetSkillUserEffect(), equipweapon.position, equipweapon.rotation, equipweapon);
        }
        else if (IsItemState(selectOption))
        {
            currentItem = item;
            animator.SetTrigger("UseItem");
            battleManager.ShowMessage(gameObject.name + "��" + objnames + "��" + currentSkill.GetKanjiName() + "���g����");
        }
    }
    //�^�[�Q�b�g�G�t�F�N�g�̕\��(���@�U��or��or�o�t)
    public void ShowEffectOnTheTarget()
    {
        //EffectPos�ɂ���ă^�[�Q�b�g�̂ǂ��ɃG�t�F�N�g��\�����邩�ύX����.
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
    //�����U�����󂯂��ۂ̏���(�^�[�Q�b�g����ver)
    public void DirectAttack()
    {
        for(int i=0; i<targetObjList.Count; i++)
        { 
            var targetAnimator = targetObjList[i].GetComponent<Animator>();
            targetAnimator.SetTrigger("Damage");
            
            var damage = 0;
            var burstup = 0;
            if(characterStatus as AllyStatus != null)
            {//�����̍U��
                //�U������̒ʏ�̖h���*�⏕�{��
                var targetDefencePower = targetCharacterStatusList[i].GetStrikingStrength() * targetCharacterBattleScriptList[i].GetAuxiliaryStrikingStrength();
                //�����_���[�W=((�ʏ�̍U����+����l)*�⏕�{��*�o�[�X�g���[�h�{��)-�h���)/2
                damage = (int)Mathf.Max(0, ((characterStatus.GetPower() + ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) * auxiliaryPower * burstbuff - targetDefencePower) / 2);
                battleManager.ShowMessage(targetObjList[i].name + "��" + damage + "�̃_���[�W���󂯂�");
                //�G�̃X�e�[�^�X��HP���Z�b�g
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
            }
            else
            {//�G�̍U��
                //�U�������(�ʏ�̖h���+�h��l)*�⏕�{��
                var targetDefencePower = (targetCharacterStatusList[i].GetStrikingStrength()+ (((AllyStatus)targetCharacterStatusList[i]).GetEquipArmor()?.GetAmount() ?? 0)) * targetCharacterBattleScriptList[i].GetAuxiliaryStrikingStrength();
                //�����_���[�W=((�ʏ�̍U����*�⏕�{��*�o�[�X�g���[�h�{��)-�h���)/2
                damage = (int)Mathf.Max(0, (characterStatus.GetPower() * auxiliaryPower * burstbuff - targetDefencePower) / 2);
                battleManager.ShowMessage(targetObjList[i].name + "��" + damage + "�̃_���[�W���󂯂�");
                //�o�[�X�g�Q�[�W������=�󂯂��_���[�W/�G�̍ő�HP*100
                burstup = (int)(((float)damage / (float)targetCharacterStatusList[i].GetMaxHp()) * 100);
                //�����̃X�e�[�^�X��HP���Z�b�g
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
                //����̃X�e�[�^�X�̃o�[�X�g�Q�[�W���Z�b�g(���Ƀo�[�X�g��ԂȂ�10%)
                targetCharacterBattleScriptList[i].SetBurst(burstup);
                //�����̃X�e�[�^�X��UI�ɔ��f
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.Burstgage, targetCharacterBattleScriptList[i].GetBurst());
            }
            Debug.Log(gameObject.name + "��" + targetCharacterStatusList[i].GetCharacterName() + "��" + currentSkill.GetKanjiName() + "������" + damage + "�_���[�W��^�����B");
            effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, targetObjList[i].transform, damage);
        }  
    }
    //���@�U�����󂯂��ۂ̏���(�f�[�^�X�V)
    /*
    public void MagicAttack()
    {
        var targetAnimator = currentTarget.GetComponent<Animator>();
        targetAnimator.SetTrigger("Damage");
        var damage = 0;
        //�_���[�W����
        if (targetCharacterStatus as AllyStatus != null)
        {//�����ւ̖��@�U��
            var castedTargetStatus = (AllyStatus)targetCharacterBattleScript.GetCharacterStatus();
            //���@�h��=(�h���(���@�h���)+�h��l)*�⏕�{��*�o�[�X�g���[�h�{��
            var targetDefencePower = (castedTargetStatus.GetMagicStrength() + (castedTargetStatus.GetEquipArmor()?.GetAmount() ?? 0))*targetCharacterBattleScript.GetAuxiliaryMagicStrength()*targetCharacterBattleScript.GetBurstBuff();
            damage = (int)Mathf.Max(0, (((Magic)currentSkill).GetMagicPower()*auxiliaryMagicPower - targetDefencePower)/2);
            battleManager.ShowMessage(currentTarget.name + "��" + damage + "�̃_���[�W���󂯂�");
            ////�@����̃X�e�[�^�X��HP���Z�b�g
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
            //�@�X�e�[�^�XUI���X�V
            battleStatusScript.UpdateStatus(targetCharacterStatus, BattleStatusScript.Status.HP, targetCharacterBattleScript.GetHp());
        }
        else if (targetCharacterStatus as EnemyStatus != null)
        {//�G�ւ̖��@�U��
            var castedTargetStatus = (EnemyStatus)targetCharacterBattleScript.GetCharacterStatus();
            var targetDefencePower = castedTargetStatus.GetStrikingStrength()*targetCharacterBattleScript.GetAuxiliaryMagicStrength();
            damage = (int)Mathf.Max(0, ((((Magic)currentSkill).GetMagicPower()+ ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) *auxiliaryMagicPower*burstbuff- targetDefencePower)/2);
            battleManager.ShowMessage(currentTarget.name + "��" + damage + "�̃_���[�W���󂯂�");
            //�@����̃X�e�[�^�X��HP���Z�b�g
            targetCharacterBattleScript.SetHp(targetCharacterBattleScript.GetHp() - damage);
        }
        else
        {
            Debug.LogError("���@�U���Ń^�[�Q�b�g���ݒ肳��Ă��Ȃ�");
        }

        Debug.Log(gameObject.name + "��" + currentTarget.name + "��" + currentSkill.GetKanjiName() + "������" + damage + "�_���[�W��^�����B");
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
                battleManager.ShowMessage(targetObjList[i].name + "��" + damage + "�̃_���[�W���󂯂�");
                //�@����̃X�e�[�^�X��HP���Z�b�g
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
            }else
            {
                var targetDefencePower = (targetCharacterStatusList[i].GetStrikingStrength()+((AllyStatus)targetCharacterStatusList[i]).GetEquipArmor()?.GetAmount()??0) * targetCharacterBattleScriptList[i].GetAuxiliaryMagicStrength();
                damage = (int)Mathf.Max(0, ((((Magic)currentSkill).GetMagicPower() + ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) * auxiliaryMagicPower * burstbuff - targetDefencePower) / 2);
                battleManager.ShowMessage(targetObjList[i].name + "��" + damage + "�̃_���[�W���󂯂�");
                //�o�[�X�g�Q�[�W������=�󂯂��_���[�W/�����̍ő�HP*100
                burstup = (int)(((float)damage / (float)targetCharacterStatusList[i].GetMaxHp()) * 100);
                //�����̃X�e�[�^�X��HP���Z�b�g
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - damage);
                //�����̃X�e�[�^�X�̃o�[�X�g�Q�[�W���Z�b�g(���Ƀo�[�X�g��ԂȂ�10%)
                targetCharacterBattleScriptList[i].SetBurst(burstup);
                //�����̃X�e�[�^�X��UI�ɔ��f
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.Burstgage, targetCharacterBattleScriptList[i].GetBurst());
            }
            
            Debug.Log(gameObject.name + "��" + targetCharacterStatusList[i].GetCharacterName() + "��" + currentSkill.GetKanjiName() + "������" + damage + "�_���[�W��^�����B");
            effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, targetObjList[i].transform, damage);
        }
    }
    //���@�U���ȊO�̖��@���󂯂��Ƃ�(��Ԉُ�,�U����,�h��̓A�b�v��UI�ɔ��f���Ȃ�)
    /*
    public void UseMagic()
    {
        //�o�t,�q�[���n�̓^�[�Q�b�g�̃��[�V�����Ȃ��{���ʉ��̂�
        var magicType = ((Magic)currentSkill).GetSkillType();
        if(magicType == Skill.Type.RecoveryMagic)
        {//�񕜗�=�X�L���̈З�+�L�����̖��@�U����
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
            Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + currentTarget.name + "��" + recoveryPoint + "�񕜂����B");
            battleManager.ShowMessage(currentTarget.name + "��" + recoveryPoint + "�񕜂���");
            effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing, currentTarget.transform, recoveryPoint);
        }
        else if (magicType == Skill.Type.IncreaseAttackPowerMagic)
        {//�����U���̓A�b�v�{��=1.3f
            increasePowerPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryPower(increasePowerPoint);
            targetCharacterBattleScript.SetIsIncreasePower(true);
            Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + currentTarget.name + "�̗͂�" + increasePowerPoint + "���₵���B");
            battleManager.ShowMessage(currentTarget.name + "�̕����U���͂�" + increasePowerPoint + "���₵��");
        }
        else if (magicType == Skill.Type.IncreaseDefencePowerMagic)
        {   //�����h��̓A�b�v�{��=1.3f
            increaseStrikingStrengthPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryStrikingStrength(increaseStrikingStrengthPoint);
            targetCharacterBattleScript.SetIsIncreaseStrikingStrength(true);
            Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + currentTarget.name + "�̑ł��ꋭ����" + increaseStrikingStrengthPoint + "���₵���B");
            battleManager.ShowMessage(currentTarget.name + "�̕����h��͂�" +increaseStrikingStrengthPoint + "���₵��");
        }
        else if (magicType == Skill.Type.IncreaseMagicPowerMagic)
        {
            //���@�U���̓A�b�v�{��=1.3f
            increaseMagicPowerPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryMagicPower(increaseMagicPowerPoint);
            targetCharacterBattleScript.SetIsIncreaseMagicPower(true);
            Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + currentTarget.name + "�̑ł��ꋭ����" + increaseMagicPowerPoint + "���₵���B");
            battleManager.ShowMessage(currentTarget.name + "�̖��@�U���͂�" + increaseStrikingStrengthPoint + "���₵��");
        }
        else if (magicType == Skill.Type.IncreaseMagicStrengthMagic)
        {
            //���@�h��̓A�b�v�{��=1.3f
            increaseMagicStrengthPoint = 1.3f;
            targetCharacterBattleScript.SetAuxiliaryStrikingStrength(increaseMagicStrengthPoint);
            targetCharacterBattleScript.SetIsIncreaseMagicStrength(true);
            Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + currentTarget.name + "�̑ł��ꋭ����" + increaseMagicStrengthPoint + "���₵���B");
            battleManager.ShowMessage(currentTarget.name + "�̖��@�h��͂�" + increaseStrikingStrengthPoint + "���₵��");
        }
        else if (magicType == Skill.Type.NumbnessRecoveryMagic)
        {
            targetCharacterStatus.SetNumbness(false);
            Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + currentTarget.name + "��Ⴢ��������");
            battleManager.ShowMessage(currentTarget.name + "��Ⴢ��������");
        }
        else if (magicType == Skill.Type.PoisonnouRecoveryMagic)
        {
            targetCharacterStatus.SetPoisonState(false);
            Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + currentTarget.name + "�̓ł�������");
            battleManager.ShowMessage(currentTarget.name + "�̓ł�������");
        }
    }
    */
    public void UseMagic()
    {
        //�o�t,�q�[���n�̓^�[�Q�b�g�̃��[�V�����Ȃ��{���ʉ��̂�
        var magicType = ((Magic)currentSkill).GetSkillType();
        if (magicType == Skill.Type.RecoveryMagic)
        {//�񕜗�=�X�L���̈З�+�L�����̖��@�U����
            var recoveryPoint = ((Magic)currentSkill).GetMagicPower() + characterStatus.GetMagicPower();
            for (int i = 0; i < targetObjList.Count; i++)
            {
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() + recoveryPoint);
                if(characterStatus as AllyStatus != null)
                {//�����̉񕜂̎���UI�ɃX�e�[�^�X�𔽉f
                    battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                }
                Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "��" + recoveryPoint + "�񕜂����B");
                battleManager.ShowMessage(targetObjList[i].name + "��" + recoveryPoint + "�񕜂���");
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Healing, targetObjList[i].transform, recoveryPoint);
            }
        }
        else if (magicType == Skill.Type.IncreaseAttackPowerMagic)
        {//�����U���̓A�b�v�{��=1.3f
            increasePowerPoint = 1.3f;
            for(int i=0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreasePower())
                {//�����U���͂��㏸���Ă��Ȃ�
                    targetCharacterBattleScriptList[i].SetAuxiliaryPower(increasePowerPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreasePower(true);
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̕����U���͂𑝂₵���B");
                    battleManager.ShowMessage(targetObjList[i].name + "�̕����U���͂�" + increasePowerPoint + "���₵��");
                }
                else
                {
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̕����U���͂𑝂₻���Ƃ���");
                    battleManager.ShowMessage(targetObjList[i].name + "�̕����U���͂͊��ɏオ���Ă���");
                }
            }
            
        }
        else if (magicType == Skill.Type.IncreaseDefencePowerMagic)
        {   //�����h��̓A�b�v�{��=1.3f
            increaseStrikingStrengthPoint = 1.3f;
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreaseStrikingStrength())
                {//�����h��͂��㏸���Ă��Ȃ�
                    targetCharacterBattleScriptList[i].SetAuxiliaryStrikingStrength(increaseStrikingStrengthPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreaseStrikingStrength(true);
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̕����h��͂𑝂₵���B");
                    battleManager.ShowMessage(targetObjList[i].name + "�̕����h��͂�" + increaseStrikingStrengthPoint + "���₵��");
                }
                else
                {
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̕����h��͂𑝂₻���Ƃ���");
                    battleManager.ShowMessage(targetObjList[i].name + "�̕����h��͂͊��ɏオ���Ă���");
                }
            }
        }
        else if (magicType == Skill.Type.IncreaseMagicPowerMagic)
        {
            //���@�U���̓A�b�v�{��=1.3f
            increaseMagicPowerPoint = 1.3f;
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreaseMagicPower())
                {//���@�U���͂��㏸���Ă��Ȃ�
                    targetCharacterBattleScriptList[i].SetAuxiliaryMagicPower(increaseMagicPowerPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreaseMagicPower(true);
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̖��@�U���͂𑝂₵���B");
                    battleManager.ShowMessage(targetObjList[i].name + "�̖��@�U���͂�" + increaseMagicPowerPoint + "���₵��");
                }
                else
                {
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̖��@�U���͂𑝂₻���Ƃ���");
                    battleManager.ShowMessage(targetObjList[i].name + "�̖��@�U���͂͊��ɏオ���Ă���");
                }
            }
        }
        else if (magicType == Skill.Type.IncreaseMagicStrengthMagic)
        {
            //���@�h��̓A�b�v�{��=1.3f
            increaseMagicStrengthPoint = 1.3f;
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (!targetCharacterBattleScriptList[i].IsIncreaseMagicStrength())
                {//���@�h��͂��㏸���Ă��Ȃ�
                    targetCharacterBattleScriptList[i].SetAuxiliaryMagicStrength(increaseMagicStrengthPoint);
                    targetCharacterBattleScriptList[i].SetIsIncreaseMagicStrength(true);
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̖��@�h��͂𑝂₵���B");
                    battleManager.ShowMessage(targetObjList[i].name + "�̖��@�h��͂�" +increaseMagicStrengthPoint + "���₵��");
                }
                else
                {
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̖��@�h��͂𑝂₻���Ƃ���");
                    battleManager.ShowMessage(targetObjList[i].name + "�̖��@�h��͂͊��ɏオ���Ă���");
                }
            }
        }
        else if (magicType == Skill.Type.NumbnessRecoveryMagic)
        {
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (targetCharacterBattleScriptList[i].IsNumbness())
                {//��Ⴢł���
                    targetCharacterBattleScriptList[i].SetNumbness(false);
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̂��т��������");
                    battleManager.ShowMessage(targetObjList[i].name + "�̂��т��������" );
                }
                else
                {
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̂��т���������Ƃ���");
                    battleManager.ShowMessage(targetObjList[i].name + "�͂��тꂶ�傤�����ł͂Ȃ�");
                }
            }
        }
        else if (magicType == Skill.Type.PoisonnouRecoveryMagic)
        {
            for (int i = 0; i < targetCharacterBattleScriptList.Count; i++)
            {
                if (targetCharacterBattleScriptList[i].IsNumbness())
                {//�łł���
                    targetCharacterBattleScriptList[i].SetPoison(false);
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̓ł�������");
                    battleManager.ShowMessage(targetObjList[i].name + "�̓ł�������");
                }
                else
                {
                    Debug.Log(gameObject.name + "��" + ((Magic)currentSkill).GetKanjiName() + "���g����" + targetCharacterStatusList[i].GetCharacterName() + "�̓ł��������Ƃ���");
                    battleManager.ShowMessage(targetObjList[i].name + "�͓ŏ�Ԃł͂Ȃ�");
                }
            }
        }
        
    }
    //�A�C�e�����g�����Ƃ��̏���(�A�C�e�����g���͖̂��������Ɖ���)
    public void UseItem()
    {
        //�L�����N�^�[�̃A�C�e���������炷
        ((AllyStatus)characterStatus).SetItemNum(currentItem, ((AllyStatus)characterStatus).GetItemNum(currentItem) - 1);

        if(currentItem.GetItemType() == Item.Type.HPRecovery)
        {
            var recoveryPoint = currentItem.GetAmount();
            for(int i=0; i < targetObjList.Count; i++)
            {
                targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() + recoveryPoint);
                battleStatusScript.UpdateStatus(targetCharacterStatusList[i], BattleStatusScript.Status.HP, targetCharacterBattleScriptList[i].GetHp());
                Debug.Log(gameObject.name + "��" + currentItem.GetKanjiName() + "���g����" + targetObjList[i].name + "��HP��" + recoveryPoint + "�񕜂����B");
                battleManager.ShowMessage(targetObjList[i].name + "��HP��" + recoveryPoint + "�񕜂���");
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
                Debug.Log(gameObject.name + "��" + currentItem.GetKanjiName() + "���g����" + targetObjList[i].name + "��MP��" + recoveryPoint + "�񕜂����B");
                battleManager.ShowMessage(targetObjList[i].name + "��MP��" + recoveryPoint + "�񕜂���");
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
                    Debug.Log(gameObject.name + "��" + currentItem.GetKanjiName() + "���g����" +targetObjList[i].name + "��Ⴢ���������B");
                    battleManager.ShowMessage(targetObjList[i].name + "��Ⴢ��������");
                }
                else
                {
                    battleManager.ShowMessage(targetObjList[i].name + "��Ⴢ��Ԃł͂���܂���");
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
                    Debug.Log(gameObject.name + "��" + currentItem.GetKanjiName() + "���g����" + targetObjList[i].name + "�̓ł��������B");
                    battleManager.ShowMessage(targetObjList[i].name + "�̓ł�������");
                }
                else
                {
                    battleManager.ShowMessage(targetObjList[i].name + "�͓ŏ�Ԃł͂���܂���");
                }
            }
        }
        //�A�C�e������0�ɂȂ���k��ItemDictionary���炻�̃A�C�e�����폜
        if (((AllyStatus)characterStatus).GetItemNum(currentItem) == 0)
        {
            ((AllyStatus)characterStatus).GetItemDictionary().Remove(currentItem);
        }
    }
    //�o�[�X�g�t�B�j�b�V�����󂯂��ۂ̏���(�f�[�^�X�V),�����������g���Ɖ���
    public void BurstFinish()
    {
        for(int i = 0; i < targetObjList.Count; i++)
        {
            var targetAnimator = targetObjList[i].GetComponent<Animator>();
            targetAnimator.SetTrigger("Damage");
            var totaldamage = 0;
            for (int j = 0; j < 3; j++)
            {
                //�o�[�X�g�t�B�j�b�V����3��G�ɍU������Ƃ���(�X�L���ɂ��̏�����������̂��A��)
                var damage = 0;
                //�U������̒ʏ�̖h���*�⏕�{��
                var targetDefencePower = targetCharacterStatusList[i].GetStrikingStrength() * targetCharacterBattleScriptList[i].GetAuxiliaryStrikingStrength();
                //�����_���[�W=((�ʏ�̍U����+����l)*�⏕�{��*�o�[�X�g���[�h�{��)-�h���)/2
                damage = (int)Mathf.Max(0, ((characterStatus.GetPower() + ((AllyStatus)characterStatus).GetEquipWeapon()?.GetAmount() ?? 0) * auxiliaryPower * burstbuff - targetDefencePower) / 2);
                totaldamage += damage;
                effectNumericalDisplayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, targetObjList[i].transform, damage);
            }
            //�G�̃X�e�[�^�X��HP���Z�b�g
            targetCharacterBattleScriptList[i].SetHp(targetCharacterBattleScriptList[i].GetHp() - totaldamage);
            Debug.Log(gameObject.name + "��" + targetObjList[i].name + "��" + currentSkill.GetKanjiName() + "������" + totaldamage + "�_���[�W��^�����B");
            battleManager.ShowMessage(targetObjList[i].name + "��" + totaldamage + "�̃_���[�W���󂯂�");
        }
        //�o�[�X�g���[�h�I��
        ChangeBurstMode(0);
    }
    //�h��(�G�t�F�N�g��\�����邽�߃K�[�h�X�L���̃f�[�^�������Ƃ��Ď擾����)
    public void Guard(Skill skill)
    {
        //�s������L�����N�^�[�������Ȃ�}�[�J�[���\����
        if (characterStatus as AllyStatus != null)
        {
            transform.Find("Marker/Image2").gameObject.SetActive(false);
        }
        //�����̃^�[���������̂ŏオ�����p�����[�^�̃`�F�b�N
        CheckIncreaseAttackPower();
        CheckIncreaseStrikingStrength();
        CheckIncreaseMagicStrength();
        CheckIncreaseMagicStrength();
        animator.SetBool("Guard", true);
        //�K�[�h�G�t�F�N�g��\��(�L�����̐^�񒆂��班���O�ɃG�t�F�N�g�\��)
        Instantiate(skill.GetSkillUserEffect(), transform.Find("CenterPos").transform.position+(transform.rotation*Vector3.forward.normalized), skill.GetSkillUserEffect().transform.rotation);
        //�K�[�h���͖h���1.3�{
        SetAuxiliaryStrikingStrength(GetAuxiliaryStrikingStrength()*1.3f);
        SetAuxiliaryMagicStrength(GetAuxiliaryMagicStrength() * 1.3f);
        battleManager.ShowMessage(gameObject.name + "�͖h����s����");
    }
    //�h�������
    public void UnlockGuard()
    {
        animator.SetBool("Guard", false);
        SetAuxiliaryStrikingStrength(GetAuxiliaryStrikingStrength()/1.3f);
        SetAuxiliaryMagicStrength(GetAuxiliaryMagicStrength() / 1.3f);
    }
    //���񂾂Ƃ��Ɏ��s���鏈��
    public void Dead()
    {
        animator.SetTrigger("Dead");
        Debug.Log(gameObject.name + "�͓|�ꂽ");
        battleManager.ShowMessage(gameObject.name + "�͓|�ꂽ");
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
    //�����U���␳���`�F�b�N
    public void CheckIncreaseAttackPower()
    {
        //�����̃^�[���������Ƃ��ɍU���͂��オ���Ă�����o�߃^�[�������ӂ₷
        if (IsIncreasePower())
        {
            numOfTurnsSinceIncreasePower++;
            if (numOfTurnsSinceIncreasePower >= numOfTurnsIncreasePower)
            {
                numOfTurnsSinceIncreasePower = 0;
                SetAuxiliaryPower(1);
                SetIsIncreasePower(false);
                Debug.Log(gameObject.name + "�̍U���͂����ɖ߂���");
               battleManager.ShowMessage(gameObject.name + "�̍U���͂����ɖ߂���");
            }
        }
    }
    //�h��␳���`�F�b�N
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
                Debug.Log(gameObject.name + "�̖h��͂����ɖ߂���");
                battleManager.ShowMessage(gameObject.name + "�̖h��͂����ɖ߂���");
            }
        }
    }

    //���@�U���␳���`�F�b�N
    public void CheckIncreaseMagicPower()
    {
        //�����̃^�[���������Ƃ��ɍU���͂��オ���Ă�����o�߃^�[�������ӂ₷
        if (IsIncreaseMagicPower())
        {
            numOfTurnsSinceIncreaseMagicPower++;
            if (numOfTurnsSinceIncreaseMagicPower >= numOfTurnsIncreaseMagicPower)
            {
                numOfTurnsSinceIncreaseMagicPower = 0;
                SetAuxiliaryMagicPower(1);
                SetIsIncreaseMagicPower(false);
                Debug.Log(gameObject.name + "�̖��@�U���͂����ɖ߂���");
                battleManager.ShowMessage(gameObject.name + "�̖��@�U���͂����ɖ߂���");
            }
        }
    }
    //���@�h��␳���`�F�b�N
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
                Debug.Log(gameObject.name + "�̖��@�h��͂����ɖ߂���");
                battleManager.ShowMessage(gameObject.name + "�̖��@�h��͂����ɖ߂���");
            }
        }
    }
    //BattleState�����@�n�̂����ꂩ�Ȃ�true��Ԃ�
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
    //BattleState�����@�n���o�t�n�̂����ꂩ�Ȃ�true��Ԃ�
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
    //BattleState���A�C�e���n�̂����ꂩ�Ȃ�true��Ԃ�
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
    //�o�[�X�g���[�h�J�ڎ��̃A�j���[�V����
    public void BurstModeAnimation()
    {
        CameraController cameraController = GameObject.Find("CameraManager").GetComponent<CameraController>();
        cameraController.SetCameraToCharaFront(this.gameObject);
        animator.SetTrigger("BurstMode");
        GameObject ins=Instantiate<GameObject>(characterStatus.GetBurstModeEffect(), this.transform.Find("ButtomPos").transform.position, characterStatus.GetBurstModeEffect().transform.rotation, this.transform.Find("ButtomPos").transform);
        ins.name = "BurstModeEffect";
    }
    //�o�[�X�g���[�h�A�j���[�V�������I���+1�b�܂ő҂R���[�`��
    IEnumerator burstmodewait(float value,CameraController cameraController)
    {
        yield return new WaitForSeconds(value);
        cameraController.SetCameraToCharaBack(this.gameObject);
    }
}
