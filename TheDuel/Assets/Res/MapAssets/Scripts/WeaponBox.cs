using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject thisObject;
    [SerializeField] GameObject mFork;
    [SerializeField] GameObject mGrenade;
    [SerializeField] GameObject mStone;

    private int mItemType;
    private int mHp = 0;

    private void Awake()
    {
        mHp = 2;
        mItemType = Random.Range(0, 3);
        Debug.Log("init");
        Debug.Log("Item type: " + mItemType);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision");

        mHp -= 1;

        GameObject obj;
        Transform tmp = new GameObject().GetComponent<Transform>();
        tmp.position = new Vector3(-3.17f, 1.0f, -0.83f);

        if (mHp <= 0)
        {
            switch (mItemType)
            {
                case 0:
                    obj = Instantiate(mFork, tmp);
                    Debug.Log("Create fork");
                    break;

                case 1:
                    obj = Instantiate(mGrenade, tmp);
                    Debug.Log("Create grenade");
                    break;

                case 2:
                    obj = Instantiate(mStone, tmp);
                    Debug.Log("Create stone");
                    break;
            }
            Destroy(thisObject);
            Debug.Log("Destroy box");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("fdsdf");
        if (other.gameObject.name=="Player")
        {
            if (Input.GetKeyDown("e"))
            {
                Debug.Log("WeaponBoxTest");
                Instantiate(mFork, thisObject.transform); //ÀÌ°Å ¿Ö ¾È µÅ
                Destroy(thisObject);
            }
        }
    }
}
