using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemIcon : MonoBehaviour
{
    public GameObject selectedIndicator;
    public Image iconImage;
    public float selectedScale = 1.1f;
    public Item target;
    public TextMeshProUGUI keyText;
    
    private void Start()
    {
        iconImage.sprite = target.icon;
        keyText.text = (Player.instance.items.IndexOf(target) + 1).ToString();
        UpdateSelectedState(Player.instance.equippedItem);
        Player.instance.onChangeEquippedItem += UpdateSelectedState;
        Player.instance.onRemoveItem += RemoveItem;
    }

    private void RemoveItem(Item item)
    {
        if (target == item)
        {
            Destroy(gameObject);
        }
        else
        {
            keyText.text = (Player.instance.items.IndexOf(target) + 1).ToString();
        }
    }
    
    private void OnDestroy()
    {
        Player.instance.onChangeEquippedItem -= UpdateSelectedState;
        Player.instance.onRemoveItem -= RemoveItem;
    }

    private void UpdateSelectedState(Item item)
    {
        var isSelected = item == target;
        var targetScale = isSelected ? selectedScale : 1;
        transform.localScale = Vector3.one * targetScale;
        selectedIndicator.SetActive(isSelected);
    }
}
