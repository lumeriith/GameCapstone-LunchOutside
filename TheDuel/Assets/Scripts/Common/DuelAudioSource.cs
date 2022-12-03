using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DuelAudioSource : MonoBehaviour
{
    public bool isPlaying => _audio.isPlaying;
    
    public AudioClip[] clips;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float volume = 1f;
    public bool affectedByTimeScale = true;
    public float delay;
    
    private AudioSource _audio;
    private float _unscaledPitch = 1f;
    
    private void Awake()
    {
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.spatialize = true;
        _audio.spatialBlend = 1f;
        _audio.volume = volume;
    }

    public void Play()
    {
        _audio.clip = clips[Random.Range(0, clips.Length)];
        _unscaledPitch = Random.Range(minPitch, maxPitch);
        _audio.pitch = _unscaledPitch * Time.timeScale;
        _audio.PlayDelayed(delay);
    }

    private void Update()
    {
        if (Math.Abs(_audio.pitch - _unscaledPitch * Time.timeScale) > 0.01f)
        {
            _audio.pitch = _unscaledPitch * Time.timeScale;
        }
    }

    public void Pause()
    {
        _audio.Pause();
    }

    public void Stop()
    {
        _audio.Stop();
    }
}
