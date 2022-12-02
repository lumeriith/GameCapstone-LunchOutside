using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryEffect : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var character = animator.GetComponent<Character>();
        var trail_renderer = character.equippedItem.GetComponentInChildren<TrailRenderer>();
        if (trail_renderer != null) trail_renderer.enabled = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var character = animator.GetComponent<Character>();
        var trail_renderer = character.equippedItem.GetComponentInChildren<TrailRenderer>();
        if (trail_renderer != null) trail_renderer.enabled = false;
    }

    public void Enable(TrailRenderer trailRenderer)
    {
        if (trailRenderer != null) trailRenderer.enabled = true;
    }

    public void Disable(TrailRenderer trailRenderer)
    {
        if (trailRenderer != null) trailRenderer.enabled = false;
    }
}