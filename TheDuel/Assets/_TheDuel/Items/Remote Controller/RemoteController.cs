using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : Item
{
    [SerializeField] AudioSource RemoteSound;

    private Character _parent;

    protected override void Awake()
    {
        base.Awake();
        _parent = GetComponent<Character>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (!isEquipped) return;
    }
    public override void OnUse()
    {
        base.OnUse();
        
        StartCoroutine(MakeFog());


    }


    IEnumerator MakeFog()
    {
        RemoteSound.Play();
        FogSystem.instance.MakeFog();
        yield return new WaitForSeconds(0.8f);
        Kill();
    }
}
