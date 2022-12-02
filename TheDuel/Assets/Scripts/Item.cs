using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isEquipped { get; set; }
    public Character owner { get; set; }
    public bool isUseReady => isEquipped && CanUse();

    public Sprite icon;
    public GameObject[] activatedOnEquip;

    public float useCooldown = 0.1f;
    public HumanBodyBones equipParentBone;
    public Vector3 equipLocalPosition;
    public Vector3 equipLocalRotation;
    public bool isCheating;
    
    private Transform _followTransform;
    private float _lastUseTime;

    public float requireStamina = 20f;

    protected virtual void Awake()
    {
        foreach (var gobj in activatedOnEquip) gobj.SetActive(false);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        if (isEquipped)
        {
            transform.localPosition = equipLocalPosition;
            transform.localRotation = Quaternion.Euler(equipLocalRotation);
        }
    }

    public void Use(GameObject obj)
    {
        if (!isUseReady) return;
        if (obj != null)
        {
            Character character = obj.GetComponent<Character>();
            if (character != null)
            {
                if (character.GetStamina() >= requireStamina)
                {
                    _lastUseTime = Time.time;
                    OnUse();
                    character.AddStamina(-requireStamina);
                }
            }
        }
    }

    /// <summary>
    /// By default it checks for cooldown time.
    /// </summary>
    /// <returns></returns>
    public virtual bool CanUse()
    {
        return Time.time - _lastUseTime > useCooldown;
    }

    public virtual void OnUse()
    {
        
    }

    public virtual void OnEquip()
    {
        foreach (var gobj in activatedOnEquip) gobj.SetActive(true);

        if (isCheating) owner.IncrementCheatingCounter();
        
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

        if (isCheating) owner.DecrementCheatingCounter();
        
        _followTransform = null;
        transform.parent = owner.transform;
    }
    
    public void Kill()
    {
        owner.RemoveItem(this);
    }
}
