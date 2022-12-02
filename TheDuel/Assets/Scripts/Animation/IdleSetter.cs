using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSetter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.GetComponent<Character>().isIdle = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        animator.GetComponent<Character>().isIdle = false;
    }
}
