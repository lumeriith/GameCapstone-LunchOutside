using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Detection : MonoBehaviour
{
    public Transform rotationPivot;
    public Image filledCircle;
    public Image outerRing;
    public Image arrow;

    public TextMeshProUGUI topText;
    public CanvasGroup topTextBoxCanvasGroup;
    public CanvasGroup meterCanvasGroup;
    public float topTextBoxStartScale = 0.7f;
    public float topTextBoxFadeTime = 0.5f;
    public float fillNonSuspiciousAngle = 120f;
    public float fillSuspiciousAngle = 10f;
    public float fillDetectedAngle = 120f;
    public float detectedPunchStrength = 0.5f;
    public float detectedPunchDuration = 0.5f;

    public float meterShowSpeed = 2f;
    public float meterHideSpeed = 1f;

    public Color nonSuspiciousTint;
    public Color suspiciousTint;
    public Color detectedTint;

    public string nonDetectedText = "부정행위";
    public string detectedText = "적발";

    private Color _filledCircleOriginalColor;
    private Color _outerRingOriginalColor;
    private Color _arrowOriginalColor;


    private Camera _camera;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        topTextBoxCanvasGroup.alpha = 0f;
        topTextBoxCanvasGroup.transform.localScale = Vector3.one * topTextBoxStartScale;

        _filledCircleOriginalColor = filledCircle.color;
        _outerRingOriginalColor = outerRing.color;
        _arrowOriginalColor = arrow.color;

        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Player.instance.onCheatingChanged += OnCheatingChanged;
        Referee.instance.onSuspicionChanged += OnSuspicionChanged;
        Referee.instance.onDetectedCheating += DetectedCheating;
        GameManager.instance.onRoundPrepare += RoundPreparing;
        _camera = Camera.main;
    }

    private void DetectedCheating()
    {
        _canvasGroup.DOFade(0f, 1f);
    }

    private void RoundPreparing()
    {
        _canvasGroup.alpha = 1f;
        meterCanvasGroup.alpha = 0f;
        topTextBoxCanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        var camForward = _camera.transform.forward;
        var delta = Referee.instance.transform.position - Player.instance.transform.position;
        camForward.y = 0f;
        delta.y = 0;
        var zAngle = Vector3.SignedAngle(delta, camForward, Vector3.up);
        rotationPivot.transform.localRotation = Quaternion.Euler(0, 0, zAngle);

        if (Referee.instance.currentSuspicion < 0.01f)
        {
            meterCanvasGroup.alpha = Mathf.MoveTowards(meterCanvasGroup.alpha, 0f, meterHideSpeed * Time.deltaTime);
        }
        else
        {
            meterCanvasGroup.alpha = Mathf.MoveTowards(meterCanvasGroup.alpha, 1f, meterShowSpeed * Time.deltaTime);
        }
    }

    private void OnCheatingChanged(bool isCheating)
    {
        if (isCheating)
        {
            topTextBoxCanvasGroup
                .DOFade(1, topTextBoxFadeTime)
                .SetUpdate(true);
            
            topTextBoxCanvasGroup.transform
                .DOScale(1, topTextBoxFadeTime)
                .SetUpdate(true);
        }
        else
        {
            topTextBoxCanvasGroup
                .DOFade(0, topTextBoxFadeTime)
                .SetUpdate(true);
            
            topTextBoxCanvasGroup.transform
                .DOScale(topTextBoxStartScale, topTextBoxFadeTime)
                .SetUpdate(true);
        }
    }

    private void OnSuspicionChanged(float prevSus, float newSus)
    {
        var angle = Mathf.Lerp(fillNonSuspiciousAngle, fillSuspiciousAngle, newSus);
        var tint = Color.Lerp(nonSuspiciousTint, suspiciousTint, newSus);
        
        if (newSus > 0.9999f)
        {
            angle = fillDetectedAngle;
            tint = detectedTint;
            topText.text = detectedText;
            transform.DOPunchScale(Vector3.one * detectedPunchStrength, detectedPunchDuration).SetUpdate(true);
        }
        else
        {
            topText.text = nonDetectedText;
        }
        
        outerRing.fillAmount = angle / 360f;
        outerRing.transform.localRotation = Quaternion.Euler(0, 0, angle / 2f);
        
        filledCircle.color = _filledCircleOriginalColor * tint;
        outerRing.color = _outerRingOriginalColor * tint;
        arrow.color = _arrowOriginalColor * tint;
    }
}
