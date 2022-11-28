using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    public GameObject[] itemsPool;

    private int _health = 2;

    public void Hit()
    {
        _health--;
        
        if (_health <= 0)
        {
            Instantiate(itemsPool[Random.Range(0, itemsPool.Length)], transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
