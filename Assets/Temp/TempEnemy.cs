using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy : MonoBehaviour
{
    public enum State
    {
        Wander,
        Seek,
        Engage,
        Attack
    }
    ///////////////
    [SerializeField] private float m_rotationSpeed;
    [SerializeField] private float m_angleMax;
    [SerializeField] private float m_seekDistance;
    [SerializeField] private float m_seekAngle;
    [SerializeField] private float m_attackDistance;
    [SerializeField] private float m_distanceBetweenCharacters;
    ///////////////
    [SerializeField] private int[] m_indexes;
    ///////////////
    private float m_averageSpeed;
    ///////////////
    public List<Character> m_enemyCharacters;
    ///////////////
    private State m_currentState;
    ///////////////
    private TempGameManager m_gameManager;
    ///////////////
    private void Start()
    {
        m_gameManager = TempGameManager.Instance();
        SpawnCharacters();
        foreach (Character character in m_enemyCharacters)
        {
            m_averageSpeed += character.Speed;
        }
        m_averageSpeed /= m_enemyCharacters.Count;
        StartCoroutine(NewWanderDestination(0.1f));
    }
    ///////////////
    private void SpawnCharacters()
    {
        for (int i = 0; i < m_indexes.Length; i++)
        {
            m_enemyCharacters.Add(m_gameManager.AllCharacterPrefabs()[i]);
        }
    }
    ///////////////
    void Update()
    {
        if (m_currentState != State.Attack) CheckForTarget();
        else
        {
            // Change for better AI combat
            //if (!m_activeCharacter) m_activeCharacter = m_characters[(int)Random.Range(0f, m_characters.Count)]; // Grabs a Random Character to do an action with
            /*if (!m_gameManager.m_turnOrder[m_gameManager.m_turnCount])
            {
                Vector3 nearestPos = GetNearestPlayer();
                if (!m_activeCharacter.IsMoving && Vector3.Distance(transform.position, nearestPos) < 1.5f)
                {
                    m_activeCharacter.SetAgentTarget(m_activeCharacter.transform.position);
                    m_activeCharacter.UseAttack((int)Random.Range(0f, m_activeCharacter.m_attacks.Count - 1));
                }
                else
                    m_activeCharacter.SetAgentTarget(nearestPos);
            }*/
        }
    }
    ///////////////
    private void CheckForTarget()
    {
        Vector3 dist = m_gameManager.AveragePlayerPosition - transform.position;
        if (dist.magnitude < m_attackDistance && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle) m_currentState = State.Engage;
        else if (dist.magnitude < m_seekDistance && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle) m_currentState = State.Seek;
        else m_currentState = State.Wander;
    }
    ///////////////
    private IEnumerator NewWanderDestination(float s)
    {
        yield return new WaitForSeconds(s);
        Vector3 dest = transform.position + Random.Range(m_seekDistance * 0.1f, m_seekDistance * 0.5f) * transform.forward + Random.Range(-m_seekDistance, m_seekDistance) * transform.right;
        if (m_currentState == State.Engage) Engage();
        else if (m_currentState == State.Wander)
        {
            m_gameManager.MoveAll(m_enemyCharacters, dest);
            StartCoroutine(NewWanderDestination(Vector3.Distance(transform.position, dest) / (m_averageSpeed * 2f)));
        }
        else StartCoroutine(NewSeekDestination(Vector3.Distance(transform.position, m_gameManager.AveragePlayerPosition) / (m_averageSpeed * 4f)));
    }
    ///////////////
    private IEnumerator NewSeekDestination(float s)
    {
        yield return new WaitForSeconds(s);
        m_gameManager.MoveAll(m_enemyCharacters, m_gameManager.AveragePlayerPosition);
        if (m_currentState == State.Engage) Engage();
        else if (m_currentState == State.Seek) StartCoroutine(NewSeekDestination(Vector3.Distance(transform.position, m_gameManager.AveragePlayerPosition) / (m_averageSpeed * 4f)));
        else StartCoroutine(NewWanderDestination(0.1f));
    }
    ///////////////
    private void Engage()
    {
        foreach (Character c in m_enemyCharacters)
        {
            DontDestroyOnLoad(c);
            c.SetAgentTarget(c.transform.position);
        }
        DontDestroyOnLoad(this);
        m_currentState = State.Attack;
    }
}