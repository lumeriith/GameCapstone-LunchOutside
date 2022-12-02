using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private static readonly int IsStunned = Animator.StringToHash("IsStunned");

    public Action<Item> onAddItem;
    public Action<Item> onRemoveItem;
    public Action<Item> onChangeEquippedItem;
    public Action<InfoAttackHit> onDealAttack;
    public Action<InfoAttackHit> onTakeAttack;
    public Action<bool> onCheatingChanged;

    public Item[] defaultItemPrefabs;
    public List<Item> items;
    public Item equippedItem;
    public int maxItems = 7;

    public const float maxStamina = 100f;
    public float stamina = 100f;
    public const float minStamina = 0f;

    public double basicAgility = 1.0;
    public const double agilityRate = 1.0;

    public const float maxHealth = 100f;
    public float health = 100f;
    public const float minHealth = 0f;

    public const float staminaRecoveryRate = 0.1f;

    public bool canAddItem => items.Count < maxItems;

    private Item _defaultItem;

    public bool isCheating { get; private set; }
    public bool isAiming { get; protected set; }
    public bool isStunned => _currentStunDuration > 0;

    private byte _isCheatingCounter;
    public Animator animator { get; private set; }
    private float _currentStunDuration;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        if (defaultItemPrefabs != null)
        {
            foreach (var w in defaultItemPrefabs)
            {
                var weap = AddItem(w);
                if (_defaultItem == null)
                {
                    _defaultItem = weap;
                    EquipDefaultItem();
                }
            }
        }

        GameManager.instance.onRoundPrepare += EquipDefaultItem;
    }

    protected virtual void Update()
    {
        _currentStunDuration = Mathf.MoveTowards(_currentStunDuration, 0, Time.deltaTime);
        animator.SetBool(IsStunned, isStunned);
<<<<<<< HEAD

        if (stamina < maxStamina) AddStamina(staminaRecoveryRate * health * Time.deltaTime);
        Debug.Log("stamina: " + stamina);
=======
>>>>>>> parent of 8ecc085 (Add several animations + Fix bugs)
    }

    public void EquipDefaultItem()
    {
        if (_defaultItem == null)
        {
            Debug.LogWarning("Default item not given", this);
            return;
        }
        EquipItem(_defaultItem);
    }

    public Item AddItem(Item prefab)
    {
        if (!canAddItem) throw new InvalidOperationException("Max item limit reached");
        var newItem = Instantiate(prefab, transform);
        newItem.owner = this;
        items.Add(newItem);
        onAddItem?.Invoke(newItem);
        return newItem;
    }

    public void EquipItem(Item item)
    {
        if (equippedItem != null) UnequipItem();
        item.isEquipped = true;
        equippedItem = item;
        item.OnEquip();
        onChangeEquippedItem?.Invoke(item);
    }

    public void UnequipItem()
    {
        if (equippedItem == null) throw new InvalidOperationException();
        equippedItem.OnUnequip();
        equippedItem = null;
    }

    public void RemoveItem(Item item)
    {
        if (item == null || !items.Contains(item)) throw new InvalidOperationException();

        if (equippedItem == item)
        {
            UnequipItem();
            EquipDefaultItem();
        }

        items.Remove(item);
        onRemoveItem?.Invoke(item);
        Destroy(item.gameObject);
    }

    public void UseItem()
    {
        if (equippedItem == null || !equippedItem.isUseReady) return;
        equippedItem.Use(gameObject);
    }

    public void CycleItemUp()
    {
        if (items.Count < 2) return;
        var i = items.IndexOf(equippedItem);
        if (i == -1) return;
        i++;
        if (i >= items.Count) i = 0;
        EquipItem(items[i]);
    }

    public void CycleItemDown()
    {
        if (items.Count < 2) return;
        var i = items.IndexOf(equippedItem);
        if (i == -1) return;
        i--;
        if (i < 0) i = items.Count - 1;
        EquipItem(items[i]);
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
        animator.SetTrigger("GetHitHead");
    }

    public void PlayHitFront()
    {
        animator.SetTrigger("GetHitFront");
    }

    public void PlayHitBack()
    {
        animator.SetTrigger("GetHitBack");
    }
<<<<<<< HEAD

    public void SetStamina(float val)
    {
        stamina = val;
        if (stamina > maxStamina) stamina = maxStamina;
        if (stamina < minStamina) stamina = minStamina;
    }

    public void AddStamina(float val)
    {
        stamina += val;
        if (stamina > maxStamina) stamina = maxStamina;
        if (stamina < minStamina) stamina = minStamina;
    }

    public float GetStamina()
    {
        return stamina;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetMinHealth()
    {
        return minHealth;
    }

    public double GetAgilityRate()
    {
        return agilityRate;
    }
=======
>>>>>>> parent of 8ecc085 (Add several animations + Fix bugs)
}
