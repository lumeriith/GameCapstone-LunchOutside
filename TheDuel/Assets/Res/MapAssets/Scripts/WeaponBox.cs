using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject thisObject;
    [SerializeField] GameObject mFork;
    [SerializeField] GameObject mGrenade;
    [SerializeField] GameObject mStone;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void asd()
    {
        return;
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("fdsdf");
        if (other.gameObject.name=="Player")
        {
            if (Input.GetKeyDown("e"))
            {
                Debug.Log("WeaponBoxTest");
                Instantiate(mFork, thisObject.transform); //¿Ã∞≈ ø÷ æ» µ≈
                Destroy(thisObject);
            }
        }
    }
}
