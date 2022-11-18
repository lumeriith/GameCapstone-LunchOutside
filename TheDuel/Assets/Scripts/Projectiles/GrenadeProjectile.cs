using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : Projectile
{
    public GameObject pinPrefab;
    
    protected override void Start()
    {
        base.Start();
        var vel = GetComponent<Rigidbody>().velocity;
        var gobj = Instantiate(pinPrefab, transform.position, transform.rotation);
        foreach (var rb in gobj.GetComponentsInChildren<Rigidbody>())
        {
            rb.velocity = vel * Random.Range(0.2f, 0.8f);
        }

        Destroy(gobj, 5f);

        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
