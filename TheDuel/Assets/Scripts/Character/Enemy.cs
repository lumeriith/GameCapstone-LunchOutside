using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public static Enemy instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Enemy>();
            return _instance;
        }
    }
    private static Enemy _instance;


    private void Awake()
    {
        base.Awake();
        target = Player.instance;
    }
}
