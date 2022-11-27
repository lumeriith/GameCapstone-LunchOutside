using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience : MonoBehaviour
{
    int randomMan;
    [SerializeField] Transform audience1;
    [SerializeField] Transform audience2;
    [SerializeField] Transform audience3;
    [SerializeField] Transform audience4;
    [SerializeField] GameObject can1;
    [SerializeField] GameObject can2;

    

    Transform startPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))

        {

            CreateCan();

        }
    }

    void CreateCan()
    {
        randomMan = Random.Range(1, 5);
        switch (randomMan)
        {
            case 1:
            startPosition = audience1;
            break;
            case 2:
            startPosition = audience2;
            break;
            case 3:
            startPosition = audience3;
            break;
            case 4:
            startPosition = audience4;
            break;
        }
        if(randomMan < 3)
        {
            Instantiate(can1, startPosition);
        }
        else
        {
            Instantiate(can2, startPosition);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            CreateCan();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            CreateCan();
        }
    }
}
