using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMartialAI : MonoBehaviour
{
    public float weight;
    public Animator modelCharacter;
    public HumanBodyBones rootBone;
    public bool disablePosition;
    public bool disableRotation;
    public float distanceThreshold = 3f;
    public float setDirectionInterval = 0.25f;
    public float setTimeScaleInterval = 0.25f;
    public float timeScaleMultiplier = 1.5f;
    public int[] actionSequence;
    public float actionInterval = 2f;
    public float postActionWaitTime = 1f;
    public float staminaUsage = 40f;
    public Transform[] collisionCheckBones;
    public float collisionCheckRadius;
    public float attackStunDuration = 2f;
    public float attackStaminaDamage = 50f;
    public float attackCooldown = 2f;
    public Effect hitEffect;

    private Animator _animator;
    private Dictionary<Transform, Transform> _modelToCharacter;
    private Vector3 _lastRootPosition;
    private Quaternion _lastRootRotation;

    private Transform _rootBoneModel;
    private Transform _rootBoneCharacter;
    private Rigidbody _rb;

    private ModelDataWriter _writer;
    private ModelConnection _connection;

    private float _lastActionTime = float.NegativeInfinity;
    private int _lastActionIndex = -1;

    private float _lastAttackTime = float.NegativeInfinity;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _modelToCharacter = new Dictionary<Transform, Transform>();
        for (int i = (int) HumanBodyBones.Hips; i < (int) HumanBodyBones.LastBone; i++)
        {
            var bone = (HumanBodyBones) i;
            var modelB = modelCharacter.GetBoneTransform(bone);
            if (modelB == null) continue;
            _modelToCharacter.Add(modelB, _animator.GetBoneTransform(bone));
        }

        _rootBoneModel = modelCharacter.GetBoneTransform(rootBone);
        _rootBoneCharacter = _animator.GetBoneTransform(rootBone);
        _writer = FindObjectOfType<ModelDataWriter>();
        _connection = FindObjectOfType<ModelConnection>();
        _connection.onConnected += () =>
        {
            _writer.WriteDoAction(1);
        };

        GameManager.instance.onRoundPrepare += () =>
        {
            StopCoroutine(nameof(StartMartialArtsRoutine));
            weight = 0;
        };

        StartCoroutine(WriteDirectionRoutine());
        StartCoroutine(WriteTimeScaleRoutine());
    }

    private IEnumerator WriteDirectionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(setDirectionInterval);
            var dir = Player.instance.transform.position - transform.position;
            _writer.WriteSetDirection(new Vector2(dir.x, dir.z));
        }
    }

    private IEnumerator WriteTimeScaleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(setTimeScaleInterval);
            _writer.WriteSetTimescale(Time.timeScale * timeScaleMultiplier);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) weight = weight < 0.5 ? 1 : 0;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine(nameof(MartialArtsRoutine));
        }
        if (weight > 0.99f)
        {
            _animator.enabled = false;
            Enemy.instance.isAttacking = false;
        }
        else _animator.enabled = true;
    }

    private IEnumerator MartialArtsRoutine()
    {
        yield return StartMartialArtsRoutine();
        while (Vector3.Distance(Enemy.instance.transform.position, Player.instance.transform.position) > distanceThreshold)
        {
            yield return new WaitForSeconds(.1f);
        }

        for (int i = 0; i < actionSequence.Length; i++)
        {
            _writer.WriteDoAction(actionSequence[i]);
            Enemy.instance.AddStamina(-staminaUsage);
            yield return new WaitForSeconds(actionInterval);
        }

        yield return new WaitForSeconds(postActionWaitTime);
        yield return StopMartialArtsRoutine();
    }

    private Collider[] _overlapResult = new Collider[128];

    private void LateUpdate()
    {
        foreach (var pair in _modelToCharacter)
        {
            if (pair.Key == _rootBoneModel) continue;
            pair.Value.localRotation = Quaternion.Slerp(pair.Value.localRotation, pair.Key.localRotation, weight);
        }


        var rotDelta = Quaternion.Inverse(_lastRootRotation) * _rootBoneModel.rotation;
        
        rotDelta = Quaternion.Slerp(Quaternion.identity, rotDelta, weight);
        rotDelta = Quaternion.Euler(0, -rotDelta.eulerAngles.x, 0);
        
        if (!disableRotation)
        {
            transform.rotation = rotDelta * transform.rotation;
            
            _rootBoneCharacter.rotation = Quaternion.Slerp(_rootBoneCharacter.rotation, _rootBoneModel.rotation, weight);
        }


        var posDelta = _rootBoneModel.position - _lastRootPosition;
        posDelta.y = 0;
        var nextPos = transform.position + posDelta * weight;
        if (!disablePosition)
        {
            transform.position = nextPos;
            
            var oriPos = _rootBoneCharacter.position;
            oriPos.y = _rootBoneModel.position.y;
            _rootBoneCharacter.position = Vector3.Lerp(_rootBoneCharacter.position, oriPos, weight);
        }
        
        _lastRootRotation = _rootBoneModel.rotation;
        _lastRootPosition = _rootBoneModel.position;
        
        if (weight < 0.8f) return;
        if (Time.time - _lastAttackTime < attackCooldown) return;
        foreach (var t in collisionCheckBones)
        {
            var count = Physics.OverlapSphereNonAlloc(t.position, collisionCheckRadius, _overlapResult,
                LayerMask.GetMask("Attackable"));
            Debug.DrawLine(t.position, Player.instance.transform.position, Color.green, 0.1f);
            for (int i = 0; i < count; i++)
            {
                if (_overlapResult[count] == null) continue;
                var p = _overlapResult[count].GetComponentInParent<Player>();
                if (p == null) continue;
                Debug.Log($"MartialArts Attack from {t} to {_overlapResult[count]}");
                Debug.DrawLine(t.position, _overlapResult[count].transform.position, Color.red, 1f);
                p.PlayHitHead();
                p.Stun(attackStunDuration);
                p.AddStamina(-attackStaminaDamage);
                hitEffect.PlayNew(_overlapResult[count].transform.position, Quaternion.identity);
                _lastAttackTime = Time.time;
                StopCoroutine(nameof(MartialArtsRoutine));
                StartCoroutine(StopMartialArtsRoutine());
                break;
            }
        }
    }

    private IEnumerator StartMartialArtsRoutine()
    {
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            weight = t;
            yield return null;
        }
        weight = 1;
    }

    private IEnumerator StopMartialArtsRoutine()
    {
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            weight = 1 - t;
            yield return null;
        }
        weight = 0;
    }
}
