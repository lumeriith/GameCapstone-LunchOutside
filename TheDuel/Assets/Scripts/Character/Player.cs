using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;



public class Player : Character
{
    public static Player instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Player>();
            return _instance;
        }
    }
    private static Player _instance;

    private LineRenderer _trajectoryRenderer;
    private Camera _cam;

    public Interactable focusedInteractable { get; private set; }
    public float interactableAngle = 60f;
    public float interactableRange = 6f;

    private vThirdPersonController _tpController;

    protected override void Awake()
    {
        base.Awake();
        _trajectoryRenderer = GetComponent<LineRenderer>();
        _trajectoryRenderer.positionCount = 20;
        _tpController = GetComponent<vThirdPersonController>();
    }

    protected override void Start()
    {
        base.Start();
        _cam = Camera.main;
        GameManager.instance.onRoundPrepare += () =>
        {
            var rot = Quaternion.LookRotation(Enemy.instance.transform.position - transform.position);
            FindObjectOfType<vThirdPersonCamera>().SetRotation(rot);
            transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
        };
    }

    protected override void Update()
    {
        base.Update();
        _tpController.strafeSpeed.rotateWithCamera = canAct;
        if (Input.GetKeyDown(KeyCode.Mouse0)) UseItem();
        var wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0)
        {
            CycleItemUp();
        }
        else if (wheel < 0)
        {
            CycleItemDown();
        }

        isAiming = Input.GetKey(KeyCode.Mouse1) && equippedItem != null && equippedItem is ThrowingItem && canAct;
        _trajectoryRenderer.enabled = isAiming;
        if (isAiming) UpdateTrajectory();

        if (Input.GetKeyDown(KeyCode.E) && focusedInteractable != null && focusedInteractable.CanInteract() && canAct)
        {
            Interact();
        }

        if (canAct)
        {
            for (int i = 0; i <= 8; i++)
            {
                if (HasItemAt(i) && Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
                {
                    SwitchToItemAtIndex(i);
                    break;
                }
            }
        }
    }


    private readonly Collider[] _interactableCheckResults = new Collider[64];
    private void FixedUpdate()
    {
        focusedInteractable = null;
        var closestAngle = Mathf.Infinity;
        var count = Physics.OverlapSphereNonAlloc(transform.position, interactableRange, _interactableCheckResults, LayerMask.GetMask("Interactable"));
        for (var i = 0; i < count; i++)
        {
            var interactable = _interactableCheckResults[i].GetComponent<Interactable>();
            if (interactable == null) continue;
            if (!interactable.CanInteract()) continue;
            var angle = Vector3.Angle(_cam.transform.forward, interactable.transform.position - _cam.transform.position);
            if (angle > interactableAngle / 2f) continue;
            if (angle > closestAngle) continue;
            focusedInteractable = interactable;
            closestAngle = angle;
        }
    }

    private void UpdateTrajectory()
    {
        const float TrajectoryDeltaTime = 0.06f;

        var weap = (ThrowingItem)equippedItem;

        var pos = weap.transform.position;
        var vel = _cam.transform.rotation * weap.throwVelocity;
        for (int i = 0; i < _trajectoryRenderer.positionCount; i++)
        {
            _trajectoryRenderer.SetPosition(i, pos);
            pos += vel * TrajectoryDeltaTime;
            vel += Physics.gravity * TrajectoryDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Respawn")
        {
            Debug.Log("Hello");
        }
    }

    public void Interact()
    {
        if (focusedInteractable == null) return;
        focusedInteractable.Interact();
    }
}