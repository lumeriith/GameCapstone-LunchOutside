using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingItem : Item
{
    public GameObject projectilePrefab;
    public Vector3 throwVelocity;
    private Camera _mainCamera;


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

    public override void OnUse()
    {
        base.OnUse();
        var gobj = Instantiate(projectilePrefab, transform.position, _mainCamera.transform.rotation);
        gobj.GetComponent<Rigidbody>().velocity = _mainCamera.transform.rotation * throwVelocity;
        Kill();
    }
}
