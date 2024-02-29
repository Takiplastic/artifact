using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayAudioSource : MonoBehaviour
{
    public enum SEType 
    {
        DirectAttack,
        MagicAttack,
        Healing,
        Buff,
        UseItem,
        BurstMode,
        BurstFinish,
        Blow,
        Damage,
        Guard,
        Dead,
    }
    //åƒÇ—èoÇ∑ÉLÅ[Ç∆SEÇÃé´èë
    [SerializeField]
    private SEDictionary seDictionary = new SEDictionary();
    private AudioSource audioSource;
    private void Start()
    {
        audioSource=this.GetComponent<AudioSource>();
    }
    public void PlaySE_DA()
    {
        audioSource.clip = seDictionary[SEType.DirectAttack];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_MA()
    {
        audioSource.clip = seDictionary[SEType.MagicAttack];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_H()
    {
        audioSource.clip = seDictionary[SEType.Healing];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_Bff()
    {
        audioSource.clip = seDictionary[SEType.Buff];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_UI()
    {
        audioSource.clip = seDictionary[SEType.UseItem];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_BM()
    {
        audioSource.clip = seDictionary[SEType.BurstMode];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_BF()
    {
        audioSource.clip = seDictionary[SEType.BurstFinish];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_Bl()
    {
        audioSource.clip = seDictionary[SEType.Blow];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_Damage()
    {
        audioSource.clip = seDictionary[SEType.Damage] ;
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_G()
    {
        audioSource.clip = seDictionary[SEType.Guard];
        audioSource.Play();
        Debug.Log("Excuted");
    }
    public void PlaySE_Dead()
    {
        audioSource.clip = seDictionary[SEType.Dead];
        audioSource.Play();
        Debug.Log("Excuted");
    }
}
