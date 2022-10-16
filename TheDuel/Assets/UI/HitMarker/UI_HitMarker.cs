using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_HitMarker : MonoBehaviour
{
    public InfoAttackHit hit;
    public float sustainTime = 1f;
    public float decayTime = 1f;

    public float scaleStart = 2f;
    public float scaleDuration = 0.5f;
    public Ease scaleEase;
    
    private Camera _main;
    private Vector3 _localPoint;
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _main = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _localPoint = hit.collider.transform.InverseTransformPoint(hit.point);
        
        UpdatePosition();
        
        transform.localScale = Vector3.one * scaleStart;
        transform.DOScale(1, scaleDuration)
            .SetEase(scaleEase)
            .SetUpdate(true);

        DOTween.Sequence()
            .AppendInterval(sustainTime)
            .Append(_canvasGroup.DOFade(0, decayTime).SetUpdate(false))
            .AppendCallback(() => Destroy(gameObject))
            .SetUpdate(true);
    }

    private void UpdatePosition()
    {
        var worldPoint = hit.collider.transform.TransformPoint(_localPoint);
        transform.position = _main.WorldToScreenPoint(worldPoint);
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }
}
