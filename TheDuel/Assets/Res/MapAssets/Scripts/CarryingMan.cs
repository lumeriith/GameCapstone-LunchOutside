using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryingMan : MonoBehaviour
{
    [SerializeField] private GameObject carryingMan;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(carryingMan, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    
    void Move()
    {
        carryingMan.transform.Translate(new Vector3(0, 0, 0.5f) * Time.deltaTime);
    }
}
