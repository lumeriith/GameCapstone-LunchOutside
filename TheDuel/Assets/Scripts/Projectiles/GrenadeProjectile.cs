using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : Projectile
{
    public GameObject pinPrefab;
    public Effect explodeEffect;
    public float delay = 2.5f;
    public float explodeMinRadius = 2f;
    public float explodeMaxRadius = 5f;
    public float explodeStunTimeMax = 4f;
    public float explodeStunTimeMin = 0.5f;
    
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
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        explodeEffect.PlayNew(transform.position, Quaternion.identity);
        var cols = Physics.OverlapSphere(transform.position, explodeMaxRadius);
        foreach (var c in cols)
        {
            var chr = c.GetComponent<Character>();
            if (chr == null) continue;
            var closestPoint = c.ClosestPoint(transform.position);
            var dist = Vector3.Distance(transform.position, closestPoint);
            var stunTime = Mathf.Lerp(explodeStunTimeMax, explodeStunTimeMin,
                (dist - explodeMinRadius) / (explodeMaxRadius - explodeMinRadius));
            chr.Stun(stunTime);

            if (Vector3.Angle(chr.transform.forward, transform.position - closestPoint) < 90)
                chr.PlayHitFront();
            else
                chr.PlayHitBack();
        }
    }
}
