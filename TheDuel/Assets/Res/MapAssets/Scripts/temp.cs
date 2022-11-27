using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onCollisionEnter(Collision col)
    {
        Debug.Log("dmkd");
        if (col.gameObject.tag == "Respawn")
        {
            Debug.Log("Hello");
        }
    }
}
