using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefereeAnimator : MonoBehaviour
{
    [SerializeField] float IdleCycle = 3.0f; //idle���� �ൿ ��ȭ�ϴ� ���� �ֱ�
    [SerializeField] float PhoneCycle = 3.0f; // phone ���� ��ȭ ���� ���� �ֱ�
    [SerializeField] GameObject Phone;
    
    private Animator animator;
    private int refereeState = 0;

    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Referee.instance.onDetectedCheating += () =>
        {
            StopCoroutine(Idle());
            StopCoroutine(Stretching());
            animator.SetInteger("RefereeState", 5);
            StartCoroutine(PlayAudio());
        };
        //Phone.GetComponent<MeshRenderer>().gameObject.SetActive(false);
        GameManager.instance.onRoundStarted += StartRound;


        StartCoroutine(Idle());

    }

    IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(0.8f);
        audioSource.Play();
    }

    

    void StartRound()
    {
        animator.SetInteger("RefereeState", 0);
        StartCoroutine(Idle());
    }


    IEnumerator Idle()
    {
        
        yield return new WaitForSeconds(IdleCycle);
        refereeState = 
            Random.Range(0, 3);
        if(refereeState == 0)//idle ����
        {
            StartCoroutine(Idle());
        }
        else if(refereeState == 1) //Stretching
        {
            StartCoroutine(Stretching());
            animator.SetInteger("RefereeState", 1);
        }
        else if (refereeState == 2) //Phone Call ����
        {
            
            animator.SetInteger("RefereeState", 2);
            StartCoroutine(PhoneCallStart());

        }
    }

    IEnumerator Stretching()
    {
        yield return new WaitForSeconds(3.0f); //��Ʈ��Ī�� �뷫 3�� �ɸ�
        animator.SetInteger("RefereeState", 0);
        StartCoroutine(Idle());
    }

    IEnumerator PhoneCallStart()
    {
        Phone.SetActive(true);
        Referee.instance.isWatching = false;
        

        yield return new WaitForSeconds(4.0f);
        StartCoroutine(PhoneCall());
    }

    IEnumerator PhoneCall()
    {
        yield return new WaitForSeconds(PhoneCycle); 
        refereeState = Random.Range(0, 3);
        if (refereeState >0 )// phone call ����
        {
            StartCoroutine(PhoneCall());
        }
        else if (refereeState == 0)
        {
            StartCoroutine(PhoneCallStop());
            animator.SetInteger("RefereeState", 0);
        }
    }
    IEnumerator PhoneCallStop()
    {
        yield return new WaitForSeconds(4.0f);
        Referee.instance.isWatching = true;
        Phone.SetActive(false);
        StartCoroutine(Idle());
    }





}
