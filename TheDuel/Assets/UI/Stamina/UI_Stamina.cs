using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stamina : MonoBehaviour
{
    public Image fillImage;
    public Image deltaImage;
    public RectTransform boxTransform;
    public RectTransform glowTransform;
    public CanvasGroup glowCanvasGroup;
    public float glowAlphaSpeed = 4f;
    public float glowScaleSpeed = 4f;

    public float fillSmoothTime = 0.1f;
    public float deltaSpeed = 0.5f;
    private float _cv;
    
    private void Start()
    {
        GameManager.instance.onRoundPrepare += () => deltaImage.fillAmount = 1f;
    }

    private void Update()
    {
        var normalized = Player.instance.GetStamina() / Player.maxStamina;

        var isDecreasing = normalized < fillImage.fillAmount;
        glowCanvasGroup.alpha = Mathf.MoveTowards(glowCanvasGroup.alpha, isDecreasing ? 1 : 0,
            Time.unscaledDeltaTime * glowAlphaSpeed);
        glowTransform.localScale = Mathf.MoveTowards(glowTransform.localScale.x, isDecreasing ? 1 : 0,
            Time.unscaledDeltaTime * glowScaleSpeed) * Vector3.one;
        
        fillImage.fillAmount = Mathf.SmoothDamp(fillImage.fillAmount, normalized, ref _cv, fillSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        deltaImage.fillAmount =
            Mathf.MoveTowards(deltaImage.fillAmount, fillImage.fillAmount, deltaSpeed * Time.unscaledDeltaTime);
        var aPos = glowTransform.anchoredPosition;
        aPos.x = fillImage.fillAmount * boxTransform.rect.width;
        glowTransform.anchoredPosition = aPos;
        
    }
}
