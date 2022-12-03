using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryingManArea : MonoBehaviour
{

    [SerializeField] GameObject WeaponBox;
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
        if(other.gameObject.tag == "Box")
        {
            Instantiate(WeaponBox, other.transform.position, other.transform.rotation); 
            Destroy(other.gameObject);
        }
    }
}
