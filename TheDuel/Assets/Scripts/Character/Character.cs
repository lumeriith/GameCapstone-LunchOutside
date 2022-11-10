using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Action<InfoAttackHit> onDealAttack;
    public Action<InfoAttackHit> onTakeAttack;

    public Weapon defaultWeaponPrefab;
    public List<Weapon> weapons;
    public Weapon equippedWeapon;
    public int maxWeapons = 3;

    public bool canAddWeapon => weapons.Count < maxWeapons;
    
    
    public bool isCheating;

    protected void Start()
    {
        if (defaultWeaponPrefab != null)
        {
            var weap = AddWeapon(defaultWeaponPrefab);
            EquipWeapon(weap);
        }
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
}
