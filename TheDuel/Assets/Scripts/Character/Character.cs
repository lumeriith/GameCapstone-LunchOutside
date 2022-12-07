using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public const float MaxStamina = 100f;
    public const float StaminaRecoveryRate = 15f;
    public const float StaminaRecoveryDelay = 1f;
    public const float StaminaRecoveryBonusThreshold = 35f;
    public const float StaminaRecoveryBonusMultiplier = 5f;
    public const double AgilityRate = 0.01;

    public const float MaxSpeed = 1.2f;
    public const float MinSpeed = 0.65f;
    public const float MinSpeedStaminaThreshold = 20f;

    public const float BreathStartStaminaThreshold = 50f;
    public const float HeartbeatStartStaminaThreshold = 20f;
    public const float BreathHeartbeatVolumeSpeed = 2f;

    public bool canAct => isIdle && !isStunned && !isSwitchingWeapon;

    public Action<Item> onAddItem;
    public Action<Item> onRemoveItem;
    public Action<Item> onChangeEquippedItem;
    public Action<InfoAttackHit> onDealAttack;
    public Action<InfoAttackHit> onTakeAttack;
    public Action<bool> onCheatingChanged;
    public Action onStaminaCheckFail;

    public Item[] defaultItemPrefabs;
    public List<Item> items;
    public Item equippedItem;
    public int maxItems = 7;

    public Effect dodgeEffect;
    public Effect drawSwordEffect;
    public Effect pickUpItemEffect;

    public AudioSource heartbeatAudio;
    public AudioSource breathAudio;

    public float stamina = MaxStamina;

    public float parryingDecreateStamina = 50f; 

    public double basicAgility = 1.0;

    public bool canAddItem => items.Count < maxItems;
    
    public bool isCheating { get; private set; }
    public bool isAiming { get; protected set; }
    public bool isDodging { get; private set; }
    public bool isStunned => _currentStunDuration > 0;

    public Animator animator { get; private set; }
    public ModelActionInput modelActionInput { get; private set; }

    [NonSerialized]
    public bool isIdle;
    public bool isAttacking;
    public bool isParrying;
    public bool isSwitchingWeapon { get; private set; }

    public float dodgeDuration = 0.5f;
    public float dodgeStaminaCost = 20f;
    public float dodgeVelocityMagnitude = 7;
    public float dodgeVelocityDecay = 15;

    public Vector3 currentDodgeVelocity { get; private set; }

    public Item lastEquipedItem;
    public bool isGetLastScore= false;

    private Item _defaultItem;
    private byte _isCheatingCounter;
    private float _currentStunDuration;
    private float _lastStaminaUseTime = float.NegativeInfinity;
    private double _lastTotalAgility;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        modelActionInput = GetComponent<ModelActionInput>();
        _lastTotalAgility = GetTotalAgility();
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
        GameManager.instance.onRoundPrepare += PlayDrawSword;
        GameManager.instance.onRoundPrepare += () => stamina = MaxStamina;
        GameManager.instance.onRoundEnded += () =>
        {
            if (lastEquipedItem != null)
            {
                if (lastEquipedItem.isFork && isGetLastScore)
                    RemoveItem(lastEquipedItem);
            }

            isGetLastScore = false;
        };
    }

    protected virtual void Update()
    {
        _currentStunDuration = Mathf.MoveTowards(_currentStunDuration, 0, Time.deltaTime);
        animator.SetBool("IsStunned", isStunned);

        if (breathAudio != null)
            breathAudio.volume = Mathf.MoveTowards(breathAudio.volume, stamina <= BreathStartStaminaThreshold ? 1 : 0, Time.deltaTime * BreathHeartbeatVolumeSpeed);
        if (heartbeatAudio != null) 
            heartbeatAudio.volume = Mathf.MoveTowards(heartbeatAudio.volume, stamina <= HeartbeatStartStaminaThreshold ? 1 : 0, Time.deltaTime * BreathHeartbeatVolumeSpeed);

        if (stamina < MaxStamina && Time.time - _lastStaminaUseTime > StaminaRecoveryDelay)
        {
            AddStamina(StaminaRecoveryRate * (stamina > StaminaRecoveryBonusThreshold ? StaminaRecoveryBonusMultiplier : 1) * Time.deltaTime);
        }

        double totalAgility = GetTotalAgility();
        if (_lastTotalAgility != totalAgility)
        {
            _lastTotalAgility = totalAgility;
            if (modelActionInput != null) modelActionInput.UpdateTotalAgility(totalAgility);
        }

        transform.position += currentDodgeVelocity * Time.deltaTime * GetSpeed();
        currentDodgeVelocity =
            Vector3.MoveTowards(currentDodgeVelocity, Vector3.zero, dodgeVelocityDecay * Time.deltaTime * GetSpeed());
        
        animator.SetFloat("Speed", GetSpeed());
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
            StartCoroutine(SwitchWeaponRoutine(EquipDefaultItem));
        }

        items.Remove(item);
        onRemoveItem?.Invoke(item);
        Destroy(item.gameObject);
    }

    public void UseItem()
    {
        if (equippedItem == null || !equippedItem.isUseReady) return;
        if (canAct && UseStamina(equippedItem.requireStamina))
        {
            equippedItem.Use();
        }
    }

    public void CycleItemUp()
    {
        if (!canAct) return;
        if (items.Count < 2) return;
        var i = items.IndexOf(equippedItem);
        if (i == -1) return;
        i++;
        if (i >= items.Count) i = 0;
        SwitchToItemAtIndex(i);
    }

    public void CycleItemDown()
    {
        if (!canAct) return;
        if (items.Count < 2) return;
        var i = items.IndexOf(equippedItem);
        if (i == -1) return;
        i--;
        if (i < 0) i = items.Count - 1;
        SwitchToItemAtIndex(i);
    }

    public void SwitchToItemAtIndex(int index)
    {
        if (index == items.IndexOf(equippedItem)) return;
        if (!HasItemAt(index)) return;
        if (!canAct) return;
        isSwitchingWeapon = true;
        StartCoroutine(SwitchWeaponRoutine(() => EquipItem(items[index])));
    }

    private IEnumerator SwitchWeaponRoutine(Action callback)
    {
        PlaySwitch();
        isSwitchingWeapon = true;
        yield return new WaitForSeconds(0.4f);
        callback?.Invoke();
        yield return new WaitForSeconds(0.4f);
        isSwitchingWeapon = false;
    }

    public bool HasItemAt(int index)
    {
        return index >= 0 && index < items.Count;
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
        isIdle = false;
        animator.SetTrigger("GetHitFront");
    }

    public void PlayHitBack()
    {
        isIdle = false;
        animator.SetTrigger("GetHitBack");
    }

    public void PlayPickUp()
    {
        isIdle = false;
        animator.SetTrigger("PickUp");
        pickUpItemEffect.Play();
    }

    public void PlayUseRemote()
    {
        animator.SetTrigger("UseRemote");
    }

    public void PlayThrow()
    {
        animator.SetTrigger("Throw");
    }

    public void PlayDodge(int direction)
    {
        isIdle = false;
        animator.SetInteger("DodgeDirection", direction);
        animator.SetTrigger("Dodge");
        dodgeEffect.Play();
    }

    public void PlayDrawSword()
    {
        isIdle = false;
        animator.SetTrigger("DrawSword");
        drawSwordEffect.Play();
    }

    public void PlaySwitch()
    {
        animator.SetTrigger("Switch");
    }

    public void PlayBasicAttack()
    {
        isIdle = false;
        animator.SetTrigger("Basic Attack");
    }
    
    public void PlayLeapAttack()
    {
        isIdle = false;
        animator.SetTrigger("Leap Attack");
    }
    
    public void PlayLowAttack()
    {
        isIdle = false;
        animator.SetTrigger("Low Attack");
    }

    public void PlayParry()
    {
        isIdle = false;
        animator.SetTrigger("Parry");
    }

    public void PlayParred()
    {
        isIdle = false;
        animator.SetTrigger("Parryed");
    }
    
    public void SetStamina(float val)
    {
        stamina = Mathf.Clamp(val, 0, MaxStamina);
    }

    public void AddStamina(float val)
    {
        stamina = Mathf.Clamp(stamina + val, 0, MaxStamina);
    }
    
    public bool UseStamina(float val)
    {
        if (val < 0) throw new InvalidOperationException("Cannot use negative amount of stamina");
        if (stamina < val)
        {
            onStaminaCheckFail?.Invoke();
            return false;
        }
        stamina -= val;
        _lastStaminaUseTime = Time.time;
        return true;
    }

    public float GetStamina()
    {
        return stamina;
    }

    public double GetAgilityRate()
    {
        return AgilityRate;
    }

    public double GetTotalAgility()
    {
        double totalAgility = basicAgility + (MaxStamina - stamina) * AgilityRate;
        if (totalAgility > 2) totalAgility = 2;
        if (totalAgility < 0.1) totalAgility = 0.1;
        return totalAgility;
    }

    public float GetSpeed()
    {
        return Mathf.Lerp(MinSpeed, MaxSpeed, (stamina - MinSpeedStaminaThreshold) / (MaxStamina - MinSpeedStaminaThreshold));
    }

    public void Dodge(int direction)
    {
        if (!canAct) return;
        if (stamina < dodgeStaminaCost) return;
        switch (direction)
        {
            case 1:
                currentDodgeVelocity = transform.forward * dodgeVelocityMagnitude;
                break;
            case 2:
                currentDodgeVelocity = transform.right * dodgeVelocityMagnitude;
                break;            
            case 3:
                currentDodgeVelocity = -transform.forward * dodgeVelocityMagnitude;
                break;            
            case 4:
                currentDodgeVelocity = -transform.right * dodgeVelocityMagnitude;
                break;
        }
        StartCoroutine(DodgeRoutine());
        IEnumerator DodgeRoutine()
        {
            isDodging = true;
            if (!UseStamina(dodgeStaminaCost)) yield break;
            PlayDodge(direction);
            yield return new WaitForSeconds(dodgeDuration);
            isDodging = false;
        }
    }
}
