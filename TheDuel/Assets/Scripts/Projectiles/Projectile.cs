using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected virtual void Awake()
    {
        StartCoroutine(ColliderSwitchRoutine());
    }

    protected virtual void Start()
    {
        
    }

    private IEnumerator ColliderSwitchRoutine()
    {
        var cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols) c.enabled = false;
        yield return new WaitForSeconds(0.2f);
        foreach (var c in cols) c.enabled = true;
    }
}
