using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSystem : MonoBehaviour
{
    [SerializeField] float DarknessLevel = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fogDensity = DarknessLevel;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            MakeFog();
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ClearFog();
        }
    }

    void MakeFog()
    {
        RenderSettings.fog = true;
    }

    void ClearFog()
    {
        RenderSettings.fog = false;
    }
}
