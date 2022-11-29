using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : Interactable
{
    public Item itemPrefab;
    private Vector3 sinTranslate = new Vector3(0, 0.1f, 0);
    private float theta = 0.0f;
    private float speed = 1.0f;
    private float rotSpeed = 100;
    
    void Update()
    {
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.World);
        transform.Translate(sinTranslate * Mathf.Sin(theta % 360) * Time.deltaTime);
        theta += speed * Time.deltaTime;
    }
    
    public override void Interact()
    {
        base.Interact();
        Player.instance.AddItem(itemPrefab);
        Destroy(gameObject);
    }
}
