using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject WEAPON_BOX;
    [SerializeField] GameObject FORK;
    [SerializeField] GameObject GRENADE;
    [SerializeField] GameObject STONE;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("u"))
        {
            Transform tmp = new GameObject().GetComponent<Transform>();
            tmp.position = new Vector3(-3.17f, 1.0f, -0.83f);
            GameObject obj = Instantiate(WEAPON_BOX, tmp);
            obj.GetComponent<WeaponBox>().thisObject = obj;
        }

        if (Input.GetKeyDown("i"))
        {
            Transform tmp = new GameObject().GetComponent<Transform>();
            tmp.position = new Vector3(-3.17f, 1.0f, -0.83f);
            GameObject obj = Instantiate(FORK, tmp);
        }
    }
}
