using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryingMan : MonoBehaviour
{
    [SerializeField] private GameObject carryingMan;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private GameObject startPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveCycle());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        carryingMan.transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * speed);
    }

    IEnumerator MoveCycle()
    {
        yield return new WaitForSeconds(75.0f);
        StartCoroutine(RandomStart());

    }
    IEnumerator RandomStart()
    {
        yield return new WaitForSeconds(3.0f);
        int randomint = Random.Range(0, 3);
        if (randomint == 0) // 1/3 È®·ü
        {
            carryingMan.transform.position = startPosition.transform.position;
            StartCoroutine(MoveCycle());
        }
        else
        {
            StartCoroutine(RandomStart());
        }
    }
}
