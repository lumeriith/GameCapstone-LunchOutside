using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{

    [SerializeField] GameObject EnemyScoreBoard;
    [SerializeField] GameObject PlayerScoreBoard;

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


    // Start is called before the first frame update
    void Start()
    {
        ScoreManager.instance.onPlayerScoreChanged += UpdatePlayerScore;
        ScoreManager.instance.onEnemyScoreChanged += UpdateEnemyScore;
    }

    // Update is called once per frame

   
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
