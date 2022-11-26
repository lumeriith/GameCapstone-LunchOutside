using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private GameObject camera1;
    



    // Start is called before the first frame update
    void Start()
    {
        camera1 = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void LateUpdate()
    {

        transform.LookAt(camera1.transform.position);
    }
}
