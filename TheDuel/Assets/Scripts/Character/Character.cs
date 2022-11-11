using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Action<InfoAttackHit> onDealAttack;
    public Action<InfoAttackHit> onTakeAttack;
    public Action<bool> onCheatingChanged;

    public Weapon[] defaultWeaponPrefabs;
    public List<Weapon> weapons;
    public Weapon equippedWeapon;
    public int maxWeapons = 3;

    public bool canAddWeapon => weapons.Count < maxWeapons;

    private Weapon _defaultWeapon;
    
    public bool isCheating
    {
        get => _isCheating;
        set
        {
            if (_isCheating == value) return;
            _isCheating = value;
            onCheatingChanged?.Invoke(_isCheating);
        }
    }

    public bool isAiming { get; protected set; }

    private bool _isCheating;

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
}
