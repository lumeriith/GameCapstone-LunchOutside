using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSystem : MonoBehaviour
{
    [SerializeField] float DarknessLevel = 0.3f;
    [SerializeField] AudioSource LightOnSound;
    [SerializeField] AudioSource LightOffSound;
    [SerializeField] float LightRecoveryTime = 8.0f;

    public bool isFogged = false;

    public static FogSystem instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<FogSystem>();
            return _instance;
        }
    }
    private static FogSystem _instance;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fogDensity = DarknessLevel;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MakeFog()
    {
        
        StartCoroutine(StartMakeFog());
    }

    IEnumerator StartClearFog()
    {
        yield return new WaitForSeconds(2.3f);
        RenderSettings.fog = false;
        isFogged = false;
    }

    IEnumerator StartMakeFog()
    {
        yield return new WaitForSeconds(0.5f);
        LightOffSound.Play();
        isFogged = true;
        RenderSettings.fog = true;
        StartCoroutine(ClearFog());
    }

    IEnumerator ClearFog()
    {
        yield return new WaitForSeconds(LightRecoveryTime);
        LightOnSound.Play();
        StartCoroutine(StartClearFog());
    }


}
