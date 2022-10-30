using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHitWindow : MonoBehaviour
{
    public Color successFillColor;
    public Color successBoxColor;
    public Color failBoxColor;
    public TextMeshProUGUI timerText;
    public Image fillImage;
    public float timerTextMultiplier = 0.2f;
    public int timerTextDecimals = 2;
    public Image timerTextBox;

    public float startScaleStart = 1f;
    public float startScaleDuration = 0.5f;
    public Ease startScaleEase;
    
    public float successTimerBoxScaleStart = 2f;
    public float successTimerBoxScaleDuration = 0.5f;
    public Ease successTimerBoxScaleEase;

    public float fadeInDuration = 0.2f;
    public float fadeOutSustain = 1f;
    public float fadeOutDuration = 0.5f;

    private CanvasGroup _canvasGroup;
    private Color _defaultFillColor;
    private Color _defaultBoxColor;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _defaultFillColor = fillImage.color;
        _defaultBoxColor = timerTextBox.color;
    }

    private void Start()
    {
        ScoreManager.instance.onPlayerScoreWindowStarted += StartScoreWindow;
        ScoreManager.instance.onPlayerScoreWindowFailed += FailScoreWindow;
        ScoreManager.instance.onPlayerScoreWindowSucceeded += SucceedScoreWindow;
    }

    private void Update()
    {
        if (ScoreManager.instance.scoreWindowState == ScoreWindowState.OpenForPlayer)
        {
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        UpdateTimerText(ScoreManager.instance.remainingScoreWindowTime * timerTextMultiplier, ScoreManager.instance.scoreWindowTime * timerTextMultiplier);
    }

    private void UpdateTimerText(float time, float max)
    {
        timerText.text = time.ToString($"n{timerTextDecimals}") + "s";
        fillImage.fillAmount = Mathf.Clamp(time / max * 0.5f, 0f, 1f);
    }

    private void StartScoreWindow()
    {
        transform.localScale = Vector3.one * startScaleStart;
        transform.DOScale(Vector3.one, startScaleDuration)
            .SetEase(startScaleEase)
            .SetUpdate(true);
        _canvasGroup.DOFade(1f, fadeInDuration)
            .SetUpdate(true);
        fillImage.color = _defaultFillColor;
        timerTextBox.color = _defaultBoxColor;
    }

    private void FailScoreWindow()
    {
        UpdateTimerText(0, ScoreManager.instance.scoreWindowTime * timerTextMultiplier);
        timerTextBox.color = failBoxColor;
        Hide();
    }

    private void SucceedScoreWindow()
    {
        timerTextBox.transform.localScale = Vector3.one * successTimerBoxScaleStart;
        timerTextBox.transform.DOScale(Vector3.one, successTimerBoxScaleDuration)
            .SetEase(successTimerBoxScaleEase)
            .SetUpdate(true);
        fillImage.color = successFillColor;
        timerTextBox.color = successBoxColor;
        Hide();
    }

    private void Hide()
    {
        DOTween.Sequence()
            .AppendInterval(fadeOutSustain)
            .Append(_canvasGroup.DOFade(0, fadeOutDuration))
            .SetUpdate(true);
    }
}
