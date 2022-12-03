using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public void Play()
    {
        if (TryGetComponent<DuelAudioSource>(out var ras))
        {
            ras.Play();
        }
        else if (TryGetComponent<AudioSource>(out var audio))
        {
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