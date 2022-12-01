using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FencingSwordBend : MonoBehaviour
{
    public Transform baseBone;
    public Transform middleBone;

    public Transform targetBase;
    public Transform targetMiddle;
    
    

    private void Update()
    {
        baseBone.rotation = targetBase.rotation;
        middleBone.rotation = targetMiddle.rotation;
    }
}
