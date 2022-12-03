using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Push();
    }

    void Push()
    {

        transform.Translate(Vector3.forward *(-1)  * Time.deltaTime * 0.1f) ;
    }
}
