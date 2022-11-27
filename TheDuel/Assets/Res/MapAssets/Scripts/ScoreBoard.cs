using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    //窍靛内爹 じさ せ

    [SerializeField] GameObject EnemyScoreBoard;
    [SerializeField] GameObject PlayerScoreBoard;

    [SerializeField] GameObject MinuteBoard;
    [SerializeField] GameObject TensSecBoard;
    [SerializeField] GameObject UnitSecBoard;

    [SerializeField] Material zero;
    [SerializeField] Material one;
    [SerializeField] Material two;
    [SerializeField] Material three;
    [SerializeField] Material four;
    [SerializeField] Material five;
    [SerializeField] Material six;
    [SerializeField] Material seven;
    [SerializeField] Material eight;
    [SerializeField] Material nine;
    [SerializeField] Material ten;
    [SerializeField] Material eleven;
    [SerializeField] Material twelve;
    [SerializeField] Material thirteen;
    [SerializeField] Material fourteen;
    [SerializeField] Material fifteen;

    [SerializeField] Material timezero;
    [SerializeField] Material timeone;
    [SerializeField] Material timetwo;
    [SerializeField] Material timethree;
    [SerializeField] Material timefour;
    [SerializeField] Material timefive;
    [SerializeField] Material timesix;
    [SerializeField] Material timeseven;
    [SerializeField] Material timeeight;
    [SerializeField] Material timenine;

    private int minute = 3;
    private int tensSec = 0;
    private int unitSec = 0;



    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.onRoundPrepare += InitializeTime;
        ScoreManager.instance.onPlayerScoreChanged += UpdatePlayerScore;
        ScoreManager.instance.onEnemyScoreChanged += UpdateEnemyScore;
        
        StartCoroutine(OneSec());
        
        
        

    }

    // Update is called once per frame

   void InitializeTime()
    {
         minute = 3;
        tensSec = 0;
        unitSec = 0;
    }

    IEnumerator OneSec()
    {
        yield return new WaitForSeconds(1.0f);
        if(unitSec == 0)
        {
            if(tensSec == 0) // ?:00
            {
                if (minute == 0) // 0:00 老版快
                {
                    Debug.Log("Round End");
                }
                else // ?:00 老 版快
                {
                    minute--;
                    tensSec = 5;
                    unitSec = 9;
                }
            }
            else //?:?0 
            {
                tensSec--;
                unitSec = 9;
            }
        }
        else
        {
            unitSec--;
        }

        UpdateTimeBoard();
        StartCoroutine(OneSec());

    }

    void UpdateTimeBoard()
    {
        switch (minute)
        {
            case 0:
                MinuteBoard.GetComponent<MeshRenderer>().material = timezero;
                break;
            case 1:
                MinuteBoard.GetComponent<MeshRenderer>().material = timeone;
                break;
            case 2:
                MinuteBoard.GetComponent<MeshRenderer>().material = timetwo;
                break;
        }

        switch (tensSec)
        {
            case 0:
                TensSecBoard.GetComponent<MeshRenderer>().material = timezero;
                break;
            case 1:
                TensSecBoard.GetComponent<MeshRenderer>().material = timeone;
                break;
            case 2:
                TensSecBoard.GetComponent<MeshRenderer>().material = timetwo;
                break;
            case 3:
                TensSecBoard.GetComponent<MeshRenderer>().material = timethree;
                break;
            case 4:
                TensSecBoard.GetComponent<MeshRenderer>().material = timefour;
                break;
            case 5:
                TensSecBoard.GetComponent<MeshRenderer>().material = timefive;
                break;
        }

        switch (unitSec)
        {
            case 0:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timezero;
                break;
            case 1:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timeone;
                break;
            case 2:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timetwo;
                break;
            case 3:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timethree;
                break;
            case 4:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timefour;
                break;
            case 5:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timefive;
                break;
            case 6:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timesix;
                break;
            case 7:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timeseven;
                break;
            case 8:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timeeight;
                break;
            case 9:
                UnitSecBoard.GetComponent<MeshRenderer>().material = timenine;
                break;
        }

    }

    void UpdatePlayerScore(float newScore)
    {
        switch (newScore)
        {
            case 1:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = one;
                break;
            case 2:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = two;
                break;
            case 3:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = three;
                break;
            case 4:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = four;
                break;
            case 5:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = five;
                break;
            case 6:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = six;
                break;
            case 7:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = seven;
                break;
            case 8:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = eight;
                break;
            case 9:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = nine;
                break;
            case 10:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = ten;
                break;
            case 11:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = eleven;
                break;
            case 12:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = twelve;
                break;
            case 13:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = thirteen;
                break;
            case 14:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = fourteen;
                break;
            case 15:
                PlayerScoreBoard.GetComponent<MeshRenderer>().material = fifteen;
                break;
        }
        
    }

    void UpdateEnemyScore(float newScore)
    {
        switch (newScore)
        {
            case 1:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = one;
                break;
            case 2:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = two;
                break;
            case 3:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = three;
                break;
            case 4:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = four;
                break;
            case 5:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = five;
                break;
            case 6:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = six;
                break;
            case 7:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = seven;
                break;
            case 8:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = eight;
                break;
            case 9:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = nine;
                break;
            case 10:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = ten;
                break;
            case 11:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = eleven;
                break;
            case 12:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = twelve;
                break;
            case 13:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = thirteen;
                break;
            case 14:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = fourteen;
                break;
            case 15:
                EnemyScoreBoard.GetComponent<MeshRenderer>().material = fifteen;
                break;
        }

    }

    void Update()

    {
        
    }
}
