using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlyingCan : MonoBehaviour
{
    public Vector3 m_Target;
    public float m_InitialAngle = 60f; // 처음 날라가는 각도
    private Rigidbody m_Rigidbody;
    int randomx, randomy, randomz;

    public float stunDuration;
    public float headshotStunDuration;
    public string[] headshotGameObjectNames;
    public float minVelocity = 3f;
    public float destroyTime = 3f;

    private bool isValid = true;

    public Effect hitEffect;

    void Start()
    {
        MakeRandom();
        GetTargetPosition();
       
        m_Rigidbody = GetComponent<Rigidbody>();
        Vector3 velocity = GetVelocity(transform.position, m_Target, m_InitialAngle);
        m_Rigidbody.velocity = velocity;
        m_Rigidbody.angularVelocity = new Vector3(0f, 0f, 5f);
    }

    void MakeRandom()
    {
        randomx = Random.Range(-3, 4);
        randomy = Random.Range(-3, 4);
        randomz = Random.Range(-3, 4);
    }

    void GetTargetPosition()
    {
        m_Target = GameObject.FindWithTag("Player").transform.position;
        m_Target += new Vector3(randomx, randomy, randomz);
    }

    void Update()
    {

    }

    public Vector3 GetVelocity(Vector3 player, Vector3 target, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarPosition = new Vector3(player.x, 0, player.z);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        float yOffset = player.y - target.y;

        float initialVelocity
            = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity
            = new Vector3(0f, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects
            = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (target.x > player.x ? 1 : -1);
        Vector3 finalVelocity
            = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }

    protected void OnCollisionEnter(Collision other)
    {
        if(isValid)
        {
            isValid = false;
            Destroy(gameObject, destroyTime);
            if (other.relativeVelocity.magnitude < minVelocity) return;
            var character = other.gameObject.GetComponentInParent<Character>();
            if (character == null) return;
            hitEffect.Play();
            var hitName = other.collider.name;
            var isHeadshot = headshotGameObjectNames.Contains(hitName);
            character.Stun(isHeadshot ? headshotStunDuration : stunDuration);
            if (isHeadshot) character.PlayHitHead();
            else character.PlayHitFront();
        }
    }
}
