using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_velocity : MonoBehaviour
{
    public float buttomspeed = 0.0f;
    //���x�����W��,�傫���قǑ��x������������(���C��������)
    //"�d��"�X�e�[�^�X���Ƃɕω�������
    float mu=0.999f;
    CharacterBattleScript characterBattleScript;
    EffectNumericalDisplayScript displayScript;
    Rigidbody rb;
    BattleEnemyStatus battleEnemyStatus;
    AudioSource audioSource;
    [SerializeField]
    AudioClip audioClip;
    public bool flag = false;
    // Start is called before the first frame update
    void Start()
    {
        characterBattleScript = this.GetComponent<CharacterBattleScript>();
        rb=GetComponent<Rigidbody>();
        displayScript = GameObject.Find("BattleManager").GetComponent<EffectNumericalDisplayScript>();
        battleEnemyStatus = GetComponent<BattleEnemyStatus>();
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        //rb.velocity*=mu;
        //��葬�x�����������~�߂�
        //Debug.Log(this.name+":"+rb.velocity.x+rb.velocity.y+rb.velocity.z);
        if(rb.velocity.magnitude<buttomspeed){
            rb.velocity=Vector3.zero;
        }
    }
    //�����ɏՓ˂�����_���[�W���󂯂�
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(this.name+"��" + collision.gameObject.name+"�ƏՓ�");
        if (flag)
        {
            int damage = ((EnemyStatus)characterBattleScript.GetCharacterStatus()).GetBlowEffect() * characterBattleScript.GetCharacterStatus().GetMaxHp() / 100;
            displayScript.InstantiatePointText(EffectNumericalDisplayScript.NumberType.Damage, this.transform, damage);
            int hpamount = characterBattleScript.GetHp() - damage;
            characterBattleScript.SetHp(characterBattleScript.GetHp() - damage);
            battleEnemyStatus.UpdateEnemyStatus(characterBattleScript.GetCharacterStatus(), BattleEnemyStatus.Status.HP, characterBattleScript.GetHp());
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
