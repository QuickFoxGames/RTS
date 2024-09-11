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
    private Transform m_target;

    private Character m_character;
    private GameManager m_gameManager;
    public enum State
    {
        Wander,
        Seek,
        Attack
    }
    private State m_currentState = State.Wander;
    void Start()
    {
        m_target = GameManager.Instance().MainCharacter.transform;
        m_character = GetComponent<Character>();
        SetRandomRotation();
    }
    void Update()
    {
        CheckForTarget();
        switch (m_currentState)
        {
            case State.Wander:
                Wander();
                break;
            case State.Seek:
                Seek();
                break;
        }
    }
    private void CheckForTarget()
    {
        Vector3 dist = m_target.position - transform.position;
        if (dist.magnitude < m_seekDistance && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle) m_currentState = State.Seek;
        else m_currentState = State.Wander;
    }
    #region Wander
    private void Wander()
    {
        WanderRotate();
        WanderMove();
    }
    private void WanderRotate()
    {
        float angleDifference = Quaternion.Angle(transform.rotation, m_targetRotation);
        float fractionOfRotationThisFrame = (m_rotationSpeed * Time.deltaTime) / angleDifference;
        transform.rotation = Quaternion.Slerp(transform.rotation, m_targetRotation, fractionOfRotationThisFrame);
        if (angleDifference < 10f)
        {
            SetRandomRotation();
        }
    }
    private void WanderMove()
    {
        m_vel = Vector3.Lerp(m_vel, m_character.Speed * transform.forward, m_character.Speed * 2.285f * Time.deltaTime);
        transform.position += m_vel * Time.deltaTime;
    }
    private void SetRandomRotation()
    {
        float randomYRotation = Random.Range(-m_angleMax, m_angleMax);
        m_targetRotation = Quaternion.Euler(0, randomYRotation, 0);
    }
    #endregion
    #region Seek
    private void Seek()
    {
        m_character.SetAgentTarget(m_target.position);
    }
    #endregion
}