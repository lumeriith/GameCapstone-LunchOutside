using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public Vector2 audioPitchRange = Vector2.one;
    public void Play()
    {
        var audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.enabled = true;
            audio.pitch = Random.Range(audioPitchRange.x, audioPitchRange.y);
            audio.Play();
        }
        
        if (TryGetComponent<ParticleSystem>(out var ps))
            ps.Play();
        
        if (TryGetComponent<CinemachineImpulseSource>(out var cis))
            cis.GenerateImpulse();
    }
    
    public void PlayNew()
    {
        var newEff = Instantiate(this, transform.position, transform.rotation);
        newEff.gameObject.AddComponent<EffectAutoDestroy>();
        newEff.Play();
    }
    
    public void PlayNew(Vector3 pos, Quaternion rot)
    {
        var newEff = Instantiate(this, pos, rot);
        newEff.gameObject.AddComponent<EffectAutoDestroy>();
        newEff.Play();
    }
}