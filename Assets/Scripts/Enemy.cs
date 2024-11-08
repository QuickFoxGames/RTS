using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public enum State
    {
        Wander,
        Seek,
        Attack
    }
    ///////////////
    private State m_currentState = State.Wander;
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
    private int m_activeIndex = 0;
    ///////////////
    private float m_averageSpeed;
    ///////////////
    private Vector3 m_wanderTarget;
    ///////////////
    public List<Character> m_enemyCharacters;
    ///////////////
    private GameManager m_gameManager;
    ///////////////
    private void Start()
    {
        m_gameManager = GameManager.Instance();
        SpawnCharacters();
        foreach (Character character in m_enemyCharacters)
        {
            m_averageSpeed += character.Speed;
        }
        m_averageSpeed /= m_enemyCharacters.Count;
        m_wanderTarget = transform.position;
        StartCoroutine(PickRandomWanderTarget());
    }
    ///////////////
    private void SpawnCharacters()
    {
        for (int i = 0; i < m_indexes.Length; i++)
        {
            m_enemyCharacters.Add(Instantiate(m_gameManager.AllCharacterPrefabs()[m_indexes[i]], transform.position, Quaternion.identity));
        }
    }
    ///////////////
    void Update()
    {
        transform.position = m_gameManager.AveragePlayerPosition(m_enemyCharacters);
        if (m_currentState != State.Attack) CheckForTarget();
        switch (m_currentState)
        {
            case State.Wander:
                Wander();
                break;
            case State.Seek:
                SeekNearestPlayer();
                break;
            case State.Attack:
                if (!m_enemyCharacters[m_activeIndex]) m_activeIndex = (int)Random.Range(0f, m_enemyCharacters.Count);
                if (!m_gameManager.m_turnOrder[m_gameManager.m_turnIndex])
                {
                    Vector3 temp = m_gameManager.GetNearestCharacter(m_gameManager.m_playerCharacters, m_enemyCharacters[m_activeIndex].transform.position).transform.position;
                    Vector3 dir = temp - transform.position;
                    if (dir.magnitude < 1.25f)
                    {
                        m_enemyCharacters[m_activeIndex].SetAgentTarget(m_enemyCharacters[m_activeIndex].transform.position);
                        m_enemyCharacters[m_activeIndex].UseAttack((int)Random.Range(0f, m_enemyCharacters[m_activeIndex].m_attacks.Count - 1), false);
                    }
                    else m_enemyCharacters[m_activeIndex].SetAgentTarget(((dir.magnitude - 1f) * dir) + m_enemyCharacters[m_activeIndex].transform.position);
                }
                break;
        }
        
        /*else
        {
            // Change for better AI combat
            if (!m_activeCharacter) m_activeCharacter = m_characters[(int)Random.Range(0f, m_characters.Count)]; // Grabs a Random Character to do an action with
            if (!m_gameManager.m_turnOrder[m_gameManager.m_turnCount])
            {
                Vector3 nearestPos = GetNearestPlayer();
                if (!m_activeCharacter.IsMoving && Vector3.Distance(transform.position, nearestPos) < 1.5f)
                {
                    m_activeCharacter.SetAgentTarget(m_activeCharacter.transform.position);
                    m_activeCharacter.UseAttack((int)Random.Range(0f, m_activeCharacter.m_attacks.Count - 1));
                }
                else
                    m_activeCharacter.SetAgentTarget(nearestPos);
            }
        }*/
        CheckForEndOfBattle();
    }
    ///////////////
    private void CheckForEndOfBattle()
    {
        int count = 0;
        foreach (Character c in m_enemyCharacters)
        {
            if (c.Health <= 0) count++;
        }
        if (count >= m_enemyCharacters.Count) m_gameManager.ExitArena();
    }
    ///////////////
    private void CheckForTarget()
    {
        Vector3 dist = m_gameManager.AveragePlayerPosition(m_gameManager.m_playerCharacters) - transform.position;
        if (dist.magnitude < m_attackDistance/* && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle*/) Engage();
        else if (dist.magnitude < m_seekDistance/* && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle*/) m_currentState = State.Seek;
        else m_currentState = State.Wander;
    }
    ///////////////
    private IEnumerator PickRandomWanderTarget()
    {
        Vector3 offset = new(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
        yield return new WaitForSeconds(offset.magnitude / m_averageSpeed);
        m_wanderTarget = transform.position + offset;
        StartCoroutine(PickRandomWanderTarget());
    }
    private void Wander()
    {
        m_gameManager.MoveAll(m_enemyCharacters, m_wanderTarget);
    }
    ///////////////
    private void SeekNearestPlayer()
    {
        m_gameManager.MoveAll(m_enemyCharacters, m_gameManager.GetNearestCharacter(m_gameManager.m_playerCharacters, transform.position).transform.position);
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
        m_gameManager.EnterArena(this);
    }
}