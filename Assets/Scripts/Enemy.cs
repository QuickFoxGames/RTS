using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed;
    [SerializeField] private float m_angleMax;
    [SerializeField] private float m_seekDistance;
    [SerializeField] private float m_seekAngle;

    private Vector3 m_vel;
    private Quaternion m_targetRotation;
    private Vector3 m_target;

    private Character m_character;
    private Player m_player;
    public enum State
    {
        Wander,
        Seek,
        Attack
    }
    private State m_currentState = State.Wander;
    void Start()
    {
        m_player = Player.Instance();
        m_character = GetComponent<Character>();
        m_target = m_player.AveragePosition;
        //SetRandomRotation();
        StartCoroutine(NewWanderDestination(0.1f));
    }
    void Update()
    {
        CheckForTarget();
    }
    private void FixedUpdate()
    {
        m_target = m_player.AveragePosition;
    }
    private void CheckForTarget()
    {
        Vector3 dist = m_target - transform.position;
        if (dist.magnitude < m_seekDistance && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle) m_currentState = State.Seek;
        else m_currentState = State.Wander;
    }
    private IEnumerator NewWanderDestination(float s)
    {
        yield return new WaitForSeconds(s);
        Vector3 dest = transform.position + Random.Range(m_seekDistance * 0.1f, m_seekDistance * 0.5f) * transform.forward + Random.Range(-m_seekDistance, m_seekDistance) * transform.right;
        if (m_currentState == State.Wander)
        {
            m_character.SetAgentTarget(dest);
            StartCoroutine(NewWanderDestination(Vector3.Distance(transform.position, dest) / (m_character.Speed * 2f)));
        }
        else StartCoroutine(NewSeekDestination(Vector3.Distance(transform.position, m_target) / (m_character.Speed * 4f)));
    }
    private IEnumerator NewSeekDestination(float s)
    {
        yield return new WaitForSeconds(s);
        m_character.SetAgentTarget(m_target);
        if (m_currentState == State.Seek) StartCoroutine(NewSeekDestination(Vector3.Distance(transform.position, m_target) / (m_character.Speed * 4f)));
        else StartCoroutine(NewWanderDestination(0.1f));
    }
}