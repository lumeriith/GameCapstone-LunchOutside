using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private static readonly int IsStunned = Animator.StringToHash("IsStunned");
    
    public Action<InfoAttackHit> onDealAttack;
    public Action<InfoAttackHit> onTakeAttack;
    public Action<bool> onCheatingChanged;

    public Weapon[] defaultWeaponPrefabs;
    public List<Weapon> weapons;
    public Weapon equippedWeapon;
    public int maxWeapons = 3;

    public bool canAddWeapon => weapons.Count < maxWeapons;

    private Weapon _defaultWeapon;
    
    public bool isCheating { get; private set; }
    public bool isAiming { get; protected set; }
    public bool isStunned => _currentStunDuration > 0;

    private byte _isCheatingCounter;
    private Animator _animator;
    private float _currentStunDuration;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        if (defaultWeaponPrefabs != null)
        {
            foreach (var w in defaultWeaponPrefabs)
            {
                var weap = AddWeapon(w);
                if (_defaultWeapon == null)
                {
                    _defaultWeapon = weap;
                    EquipDefaultWeapon();
                }
            }
        }

        GameManager.instance.onRoundPrepare += EquipDefaultWeapon;
    }

    protected virtual void Update()
    {
        _currentStunDuration = Mathf.MoveTowards(_currentStunDuration, 0, Time.deltaTime);
        _animator.SetBool(IsStunned, isStunned);
    }

    public void EquipDefaultWeapon()
    {
        if (_defaultWeapon == null)
        {
            Debug.LogWarning("Default weapon not given", this);
            return;
        }
        EquipWeapon(_defaultWeapon);
    }

    public Weapon AddWeapon(Weapon prefab)
    {
        if (!canAddWeapon) throw new InvalidOperationException("Max weapon limit reached");
        var newWeapon = Instantiate(prefab, transform);
        newWeapon.owner = this;
        weapons.Add(newWeapon);
        return newWeapon;
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (equippedWeapon != null) UnequipWeapon();
        weapon.isEquipped = true;
        equippedWeapon = weapon;
        weapon.OnEquip();
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon == null) throw new InvalidOperationException();
        equippedWeapon.OnUnequip();
        equippedWeapon = null;
    }

    public void RemoveWeapon(Weapon weapon)
    {
        if (weapon == null || !weapons.Contains(weapon)) throw new InvalidOperationException();
        
        if (equippedWeapon == weapon)
        {
            UnequipWeapon();
            EquipDefaultWeapon();
        }

        weapons.Remove(weapon);
        Destroy(weapon);
    }
    
    public void UseWeapon()
    {
        if (equippedWeapon == null || !equippedWeapon.isUseReady) return;
        equippedWeapon.Use();
    }

    public void CycleWeaponUp()
    {
        if (weapons.Count < 2) return;
        var i = weapons.IndexOf(equippedWeapon);
        if (i == -1) return;
        i++;
        if (i >= weapons.Count) i = 0;
        EquipWeapon(weapons[i]);
    }

    public void CycleWeaponDown()
    {
        if (weapons.Count < 2) return;
        var i = weapons.IndexOf(equippedWeapon);
        if (i == -1) return;
        i--;
        if (i < 0) i = weapons.Count - 1;
        EquipWeapon(weapons[i]);
    }

    public void IncrementCheatingCounter()
    {
        if (_isCheatingCounter == byte.MaxValue) throw new InvalidOperationException();
        _isCheatingCounter++;
        UpdateCheatingState();
    }

    public void DecrementCheatingCounter()
    {
        if (_isCheatingCounter == byte.MinValue) throw new InvalidOperationException();
        _isCheatingCounter--;
        UpdateCheatingState();
    }

    private void UpdateCheatingState()
    {
        if (isCheating != _isCheatingCounter > 0)
        {
            onCheatingChanged?.Invoke(_isCheatingCounter > 0);
            isCheating = _isCheatingCounter > 0;
        }
    }

    public void Stun(float duration)
    {
        _currentStunDuration = Mathf.Max(_currentStunDuration, duration);
    }

    public void PlayHitHead()
    {
        _animator.SetTrigger("GetHitHead");
    }

    public void PlayHitFront()
    {
        _animator.SetTrigger("GetHitFront");
    }

    public void PlayHitBack()
    {
        _animator.SetTrigger("GetHitBack");
    }


}
