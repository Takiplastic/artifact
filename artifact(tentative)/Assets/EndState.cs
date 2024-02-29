using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndState : StateMachineBehaviour
{
    private BattleManager battleManager;
    private CharacterBattleScript characterBattleScript;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //BattleManagerを取得していなければ探す
        if(battleManager == null)
        {
            battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        }
        if (characterBattleScript == null)
        {
            characterBattleScript = animator.GetComponent<CharacterBattleScript>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //スキルを使った側の場合はアニメーションの終了フラグのオンと次のキャラクターへのターン    
        if (stateInfo.IsName("DirectAttack") || stateInfo.IsName("BlowAttack") || stateInfo.IsName("MagicAttack") || stateInfo.IsName("UseItem") || stateInfo.IsName("BurstFinish3"))
        {
            characterBattleScript.SetIsDoneAnimation();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
