using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : Interactable
{
    private readonly Color DefaultOutlineColor = new Color(1, 1, 1);
    private const float DefaultOutlineWidth = 3f;

    private readonly Color FocusedOutlineColor = new Color(1, 1, 1);
    private const float FocusedOutlineWidth = 6f;
    
    public Item itemPrefab;
    private Vector3 sinTranslate = new Vector3(0, 0.1f, 0);
    private float theta = 0.0f;
    private float speed = 1.0f;
    private float rotSpeed = 100;

    private QuickOutline _outline;

    private void Awake()
    {
        _outline = gameObject.AddComponent<QuickOutline>();
        _outline.OutlineMode = QuickOutline.Mode.OutlineVisible;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.World);
        transform.Translate(sinTranslate * Mathf.Sin(theta % 360) * Time.deltaTime);
        theta += speed * Time.deltaTime;

        var isFocused = Player.instance != null && Player.instance.focusedInteractable == this;
        _outline.OutlineColor = isFocused ? FocusedOutlineColor : DefaultOutlineColor;
        _outline.OutlineWidth = isFocused ? FocusedOutlineWidth : DefaultOutlineWidth;
    }
    
    public override void Interact()
    {
        base.Interact();
        StartCoroutine(PickupRoutine());
        IEnumerator PickupRoutine()
        {
            Player.instance.PlayPickUp();
            yield return new WaitForSeconds(.3f);
            Player.instance.AddItem(itemPrefab);
            Destroy(gameObject);
        }
    }

    public override bool CanInteract(Character character)
    {
        return base.CanInteract(character) && character.canAddItem;
    }
}
