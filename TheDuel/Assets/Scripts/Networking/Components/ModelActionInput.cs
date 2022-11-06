using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModelActionInput : ModelComponentBase
{
    private int _actionNumber = 0;
    private void DoActionTest()
    {
        if (_actionNumber == 5) _actionNumber = 0;
        writer.WriteDoAction(_actionNumber);
        _actionNumber++;
    }
    
    private void Update()
    {
        if (connection.isConnected)
        {
            if (Input.GetKeyDown(KeyCode.Space)) DoActionTest();
            if (Input.GetKeyDown(KeyCode.Alpha1)) writer.WriteSetDirection(Random.insideUnitCircle);
            if (Input.GetKeyDown(KeyCode.Alpha2)) writer.WriteSetDirection(Vector2.zero);
        }
    }
}
