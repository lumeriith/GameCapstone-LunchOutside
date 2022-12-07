using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CinematicSkipper : MonoBehaviour
{
    public CanvasGroup darkOverlay;
    public Image progressBar;
    public float skipHoldDuration = 2f;
    
    private CanvasGroup _canvasGroup;
    private bool _isInTransition;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_isInTransition) return;
        var isHoldingDown = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) ||
                            Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1);

        if (isHoldingDown)
        {
            _canvasGroup.alpha = 1f;
            progressBar.fillAmount += 1 / skipHoldDuration * Time.deltaTime;
            if (progressBar.fillAmount > 0.99f)
            {
                StartCoroutine(SkipRoutine());
                _isInTransition = true;
            }
        }
        else
        {
            _canvasGroup.alpha = 0f;
            progressBar.fillAmount = 0f;
        }
    }

    private IEnumerator SkipRoutine()
    {
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            darkOverlay.alpha = t;
            yield return null;
        }
        darkOverlay.alpha = 1;
        yield return null;
        LoadMapScene();
    }

    public void LoadMapScene()
    {
        SceneManager.LoadScene("Map");
    }
}
