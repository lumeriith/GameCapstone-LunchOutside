using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingSetter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        var character = animator.GetComponent<Character>();
        character.isAttacking = true;
        character.isIdle = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        var character = animator.GetComponent<Character>();
        
        character.isAttacking = false;
        character.isIdle = true;
    }
}
