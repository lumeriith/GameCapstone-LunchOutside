using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isEquipped { get; set; }
    public Character owner { get; set; }

    public GameObject[] activatedOnEquip;
    
    public HumanBodyBones equipParentBone;
    public Vector3 equipLocalPosition;
    public Vector3 equipLocalRotation;
    public bool isCheating;
    
    private Transform _followTransform;

    protected virtual void Awake()
    {
        foreach (var gobj in activatedOnEquip) gobj.SetActive(false);
    }

    protected virtual void Update()
    {
        if (isEquipped)
        {
            transform.localPosition = equipLocalPosition;
            transform.localRotation = Quaternion.Euler(equipLocalRotation);
        }
    }

    public virtual void OnEquip()
    {
        foreach (var gobj in activatedOnEquip) gobj.SetActive(true);

        if (isCheating) owner.isCheating = true;
        
        var animator = GetComponentInParent<Animator>();
        if (animator == null) GetComponentInChildren<Animator>();
        if (animator == null) throw new NullReferenceException("Animator not found");
        _followTransform = animator.GetBoneTransform(equipParentBone);
        if (_followTransform == null) throw new NullReferenceException("Weapon parent bone not found");
        transform.parent = _followTransform;
    }

    public virtual void OnUnequip()
    {
        foreach (var gobj in activatedOnEquip) gobj.SetActive(false);

        if (isCheating) owner.isCheating = false;
        
        _followTransform = null;
        transform.parent = owner.transform;
    }
    
    public void Kill()
    {
        owner.RemoveWeapon(this);
    }
}
