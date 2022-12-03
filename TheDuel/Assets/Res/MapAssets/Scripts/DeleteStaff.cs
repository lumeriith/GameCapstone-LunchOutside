using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteStaff : MonoBehaviour
{
    [SerializeField] private int RandomTime;
    [SerializeField] private Transform startPosition;
    [SerializeField] private GameObject CarryingMan;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "CarryingMan")
        {
            Destroy(other.gameObject);
            StartCoroutine(RandomStart());
        }
    }

    IEnumerator RandomStart()
    {
        yield return new WaitForSeconds(RandomTime);
        int randomint = Random.Range(0, 2);
        if (randomint == 0) // 1/2 È®·ü
        {
            Instantiate(CarryingMan, startPosition.transform.position, startPosition.transform.rotation);
        }
        else
        {
            StartCoroutine(RandomStart());
        }
    }
}
