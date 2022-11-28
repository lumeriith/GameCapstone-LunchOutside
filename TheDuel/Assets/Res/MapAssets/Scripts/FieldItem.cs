using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public Item itemPrefab;
    private Vector3 sinTranslate = new Vector3(0, 0.1f, 0);
    private float theta = 0.0f;
    private float speed = 1.0f;
    private float rotSpeed = 100;

    //[SerializeField] GameObject InteractionMenu;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.World);
        transform.Translate(sinTranslate * Mathf.Sin(theta % 360) * Time.deltaTime);
        theta += speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player.instance.AddItem(itemPrefab);
            Destroy(gameObject);
        }
    }
}
