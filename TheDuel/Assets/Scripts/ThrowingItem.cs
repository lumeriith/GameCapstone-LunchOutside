using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingItem : Item
{
    public GameObject projectilePrefab;
    public Vector3 throwVelocity;
    private Camera _mainCamera;

    private bool _disabled;

    protected override void Start()
    {
        base.Start();
        _mainCamera = Camera.main;
    }

    protected override void Update()
    {
        base.Update();
        if (!isEquipped) return;
    }

    public override bool CanUse()
    {
        return !_disabled;
    }

    public override void OnUse()
    {
        base.OnUse();
        StartCoroutine(UseRoutine());

        IEnumerator UseRoutine()
        {
            _disabled = true;
            var gobj = Instantiate(projectilePrefab, transform.position, _mainCamera.transform.rotation);
            gobj.GetComponent<Rigidbody>().velocity = _mainCamera.transform.rotation * throwVelocity;
            owner.PlayThrow();
            foreach (var g in activatedOnEquip)
            {
                g.SetActive(false);
            }

            yield return new WaitForSeconds(0.65f);
            Kill();
        }
    }
}
