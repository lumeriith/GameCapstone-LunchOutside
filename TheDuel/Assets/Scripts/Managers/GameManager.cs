using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : ManagerBase<GameManager>
{
    public Action onRoundEnded;
    public Action onRoundPrepare;
    public Action onRoundStarted;

    public bool isRoundOngoing { get; private set; }

    public Volume fadeOutVolume;

    public Transform playerStartPos;
    public Transform enemyStartPos;
    public BoxCollider playRegionCollider;
    
    public Bounds playRegion { get; private set; }
    
    public bool isPlayerOutOfBounds
    {
        get => _isPlayerOutOfBounds;
        set
        {
            if (_isPlayerOutOfBounds == value) return;
            _isPlayerOutOfBounds = value;
            if (value) Player.instance.IncrementCheatingCounter();
            else Player.instance.DecrementCheatingCounter();
        }
    }

    private bool _isPlayerOutOfBounds;
    

    public int cheatPenalty;

    private void Awake()
    {
        playRegion = playRegionCollider.bounds;
    }

    private void Start()
    {
        StartCoroutine(StartRoundRoutine());

        Referee.instance.onDetectedCheating += () =>
        {
            StartCoroutine(CheatRestartRoutine());
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) ResetPosition();
    }

    private void FixedUpdate()
    {
        isPlayerOutOfBounds = !playRegionCollider.Raycast(new Ray(Player.instance.transform.position + Vector3.down * 5f, Vector3.up), out var info, 10f);
    }

    public void StartNewRound()
    {
        StartCoroutine(StartNewRoundRoutine());
    }

    private IEnumerator StartNewRoundRoutine()
    {
        yield return EndRoundRoutine();
        yield return StartRoundRoutine();
    }

    private IEnumerator EndRoundRoutine()
    {
        if (isRoundOngoing)
        {
            isRoundOngoing = false;
            onRoundEnded?.Invoke();   
        }
        yield return new WaitForSecondsRealtime(1f);
        DOTween.To(() => fadeOutVolume.weight, v => fadeOutVolume.weight = v, 1f, 1f);
        yield return new WaitForSecondsRealtime(1f);
    }
    
    private IEnumerator StartRoundRoutine()
    {
        fadeOutVolume.weight = 1f;

        StartCoroutine(ResetPositionRoutine());

        onRoundPrepare?.Invoke();
        DOTween.To(() => fadeOutVolume.weight, v => fadeOutVolume.weight = v, 0f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        onRoundStarted?.Invoke();
        isRoundOngoing = true;
    }

    private IEnumerator CheatRestartRoutine()
    {
        isRoundOngoing = false;
        onRoundEnded?.Invoke();
        yield return new WaitForSeconds(1.5f);
        ScoreManager.instance.IncrementEnemyScore(cheatPenalty);
        yield return new WaitForSeconds(2f);
        StartNewRound();
    }

    private IEnumerator ResetPositionRoutine()
    {
        for (int i = 0; i < 10; i++)
        {
            ResetPosition();
            yield return null;
        }    
    }

    private void ResetPosition()
    {
        Player.instance.rigidbody.position = playerStartPos.position;
        Player.instance.rigidbody.isKinematic = true;
        Player.instance.rigidbody.isKinematic = false;
        Player.instance.rigidbody.velocity = Vector3.zero;
        Player.instance.rigidbody.angularVelocity = Vector3.zero;
        
        Enemy.instance.rigidbody.position = enemyStartPos.position;
        Enemy.instance.rigidbody.isKinematic = true;
        Enemy.instance.rigidbody.isKinematic = false;
        Enemy.instance.rigidbody.velocity = Vector3.zero;
        Enemy.instance.rigidbody.angularVelocity = Vector3.zero;
        
        Player.instance.rigidbody.rotation = Quaternion.Euler(0, Quaternion.LookRotation(Enemy.instance.rigidbody.position - Player.instance.rigidbody.position).eulerAngles.y, 0);
        Enemy.instance.rigidbody.rotation = Quaternion.Euler(0, Quaternion.LookRotation(Player.instance.rigidbody.position - Enemy.instance.rigidbody.position).eulerAngles.y, 0);
        
        var cam = FindObjectOfType<vThirdPersonCamera>();
        cam.mouseY = 9;
        cam.mouseX = 183;

    }
}
