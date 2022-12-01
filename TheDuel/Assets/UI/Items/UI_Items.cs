using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Items : MonoBehaviour
{
    public UI_ItemIcon iconPrefab;
    public Transform iconParent;
    
    private void Start()
    {
        Player.instance.onAddItem += AddItemIcon;
        foreach (var item in Player.instance.items)
        {
            AddItemIcon(item);
        }
    }

    private void AddItemIcon(Item target)
    {
        Instantiate(iconPrefab, iconParent).target = target;
    }
}
