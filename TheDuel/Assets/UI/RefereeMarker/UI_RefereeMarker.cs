using System;
using System.Collections;
using System.Collections.Generic;
using PixelPlay.OffScreenIndicator;
using UnityEngine;

public class UI_RefereeMarker : MonoBehaviour
{
    public float watchingAlpha = 1f;
    public float notWatchingAlpha = 0.5f;
    public float alphaRiseSpeed = 2f;
    public float alphaDecaySpeed = 0.5f;
    public float viewportXMargin = 0.06f;
    public float viewportYMargin = 0.1f;
    public float angleOffset;
    public Transform arrowPivot;
    public GameObject openEye;
    public GameObject closedEye;

    public Vector3 visibleWorldOffset;
    public Vector3 nonVisibleWorldOffset;
    
    private Camera _main;
    private CanvasGroup _canvasGroup;
    private Vector3 _screenCentre;
    private Vector3 _screenBounds;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
        _screenBounds = new Vector3(_screenCentre.x * (1 - viewportXMargin), _screenCentre.y * (1 - viewportYMargin));
    }

    private void Start()
    {
        _main = Camera.main;
        Referee.instance.onDetectedCheating += () =>
        {
            gameObject.SetActive(false);
        };
        GameManager.instance.onRoundPrepare += () =>
        {
            gameObject.SetActive(true);
        };
    }

    private void Update()
    {
        var isWatching = Referee.instance.isWatching;
        openEye.SetActive(isWatching);
        closedEye.SetActive(!isWatching);
        if (Referee.instance.currentVisibility > 0.05f)
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, watchingAlpha, alphaRiseSpeed * Time.deltaTime);
        else
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, notWatchingAlpha, alphaDecaySpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        var targetPoint = Referee.instance.transform.position + visibleWorldOffset;
        var screenPoint = _main.WorldToScreenPoint(targetPoint);
        float angle = 0f;

        if (!OffScreenIndicatorCore.IsTargetVisible(screenPoint))
        {
            targetPoint = Referee.instance.transform.position + nonVisibleWorldOffset;
            screenPoint = _main.WorldToScreenPoint(targetPoint);
            OffScreenIndicatorCore.GetArrowIndicatorPositionAndAngle(ref screenPoint, ref angle, _screenCentre, _screenBounds);
            angle = angle * Mathf.Rad2Deg + angleOffset;
        }
        
        transform.position = screenPoint;
        arrowPivot.rotation = Quaternion.Euler(0, 0, angle);
    }
}
