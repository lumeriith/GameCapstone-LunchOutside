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
        equippedItem.Use();
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
}
