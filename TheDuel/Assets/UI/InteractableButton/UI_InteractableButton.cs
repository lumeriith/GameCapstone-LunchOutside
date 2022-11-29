using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InteractableButton : MonoBehaviour
{
    public float alphaRiseSpeed = 5f;
    
    private CanvasGroup _cg;
    private Camera _main;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0f;

        _main = Camera.main;
    }

    private void Update()
    {
        if (Player.instance.focusedInteractable != null)
        {
            _cg.alpha = Mathf.MoveTowards(_cg.alpha, 1f, alphaRiseSpeed * Time.deltaTime);
            transform.position = _main.WorldToScreenPoint(Player.instance.focusedInteractable.transform.position);
        }
        else
        {
            _cg.alpha = 0f;
        }
    }
}
