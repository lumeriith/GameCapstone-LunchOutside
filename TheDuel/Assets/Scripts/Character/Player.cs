using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Character
{
    public static Player instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Player>();
            return _instance;
        }
    }
    private static Player _instance;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Input.GetKey(KeyCode.W)) _animator.SetTrigger("Leap Attack");
            else _animator.SetTrigger("Basic Attack");
        }
    }
}
