using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public Item itemPrefab;
    [SerializeField] private GameObject thisObject;
    
    //[SerializeField] GameObject InteractionMenu;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 150 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           
            Player.instance.AddItem(itemPrefab);

            Debug.Log("æ∆¿Ã≈€ »πµÊ");

            Destroy(thisObject);
            
        }
    }
}
