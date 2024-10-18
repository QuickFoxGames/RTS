/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy : MonoBehaviour
{
    [SerializeField] private int m_minCharacters;
    [SerializeField] private int m_maxCharacters;

    [SerializeField] private float m_rotationSpeed;
    [SerializeField] private float m_angleMax;
    [SerializeField] private float m_seekDistance;
    [SerializeField] private float m_seekAngle;
    [SerializeField] private float m_attackDistance;
    [SerializeField] private float m_distanceBetweenCharacters;

    private float m_averageSpeed;

    private Vector3 m_target;

    public Character m_activeCharacter;
    public List<Character> m_characters = new();
    private Player m_player;
    private GameManager m_gameManager;
    public enum State
    {
        Wander,
        Seek,
        Engage,
        Attack
    }
    private State m_currentState = State.Wander;
    private void SpawnCharacters()
    {
        m_characters.Clear();
        int n = (int)Random.Range(m_minCharacters, m_maxCharacters);
        for (int i = 0; i < n; i++)
        {
            m_characters.Add(m_gameManager.GrabRandomCharacter());
        }
    }
    void Start()
    {
        m_player = Player.Instance();
        m_gameManager = GameManager.Instance();
        m_target = m_player.AveragePosition;
        SpawnCharacters();
        foreach (Character character in m_characters)
        {
            m_averageSpeed += character.Speed;
        }
        m_averageSpeed /= m_characters.Count;
        StartCoroutine(NewWanderDestination(0.1f));
    }
    void Update()
    {
        if (m_currentState != State.Attack) CheckForTarget();
        else
        {
            // Change for better AI combat
            if (!m_activeCharacter) m_activeCharacter = m_characters[(int)Random.Range(0f, m_characters.Count)]; // Grabs a Random Character to do an action with
            else if (!m_gameManager.m_turnOrder[m_gameManager.m_turnCount])
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
        }
    }
    private void MoveCharacters(Vector3 pos)
    {
        float angleStep = 360f / m_characters.Count;
        for (int i = 0; i < m_characters.Count; i++)
        {
            float angleInRadians = Mathf.Deg2Rad * angleStep * i;
            float x = pos.x + m_distanceBetweenCharacters * Mathf.Cos(angleInRadians);
            float z = pos.z + m_distanceBetweenCharacters * Mathf.Sin(angleInRadians);

            m_characters[i].SetAgentTarget(new(x, pos.y, z));
        }
    }
    private Vector3 GetNearestPlayer()
    {
        float temp = Mathf.Infinity;
        Vector3 pos = Vector3.zero;
        Vector3 dir = Vector3.zero;
        foreach (Character c in m_player.Characters)
        {
            float d = Vector3.Distance(transform.position, c.transform.position);
            if (d < temp)
            {
                temp = d;
                pos = c.transform.position;
                dir = transform.position - pos;
            }
        }
        m_activeCharacter.transform.forward = -dir.normalized;
        return pos + dir.normalized;
    }
    private void FixedUpdate()
    {
        if (m_currentState != State.Attack) m_target = m_player.AveragePosition;
    }
    private void CheckForTarget()
    {
        Vector3 dist = m_target - transform.position;
        if (dist.magnitude < m_attackDistance && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle) m_currentState = State.Engage;
        else if (dist.magnitude < m_seekDistance && Vector3.Dot(dist.normalized, transform.forward) < m_seekAngle) m_currentState = State.Seek;
        else m_currentState = State.Wander;
    }
    private IEnumerator NewWanderDestination(float s)
    {
        yield return new WaitForSeconds(s);
        Vector3 dest = transform.position + Random.Range(m_seekDistance * 0.1f, m_seekDistance * 0.5f) * transform.forward + Random.Range(-m_seekDistance, m_seekDistance) * transform.right;
        if (m_currentState == State.Engage) Engage();
        else if (m_currentState == State.Wander)
        {
            MoveCharacters(dest);
            StartCoroutine(NewWanderDestination(Vector3.Distance(transform.position, dest) / (m_averageSpeed * 2f)));
        }
        else StartCoroutine(NewSeekDestination(Vector3.Distance(transform.position, m_target) / (m_averageSpeed * 4f)));
    }
    private IEnumerator NewSeekDestination(float s)
    {
        yield return new WaitForSeconds(s);
        MoveCharacters(m_target);
        if (m_currentState == State.Engage) Engage();
        else if (m_currentState == State.Seek) StartCoroutine(NewSeekDestination(Vector3.Distance(transform.position, m_target) / (m_averageSpeed * 4f)));
        else StartCoroutine(NewWanderDestination(0.1f));
    }
    private void Engage()
    {
        foreach (Character c in m_characters)
        {
            DontDestroyOnLoad(c);
            c.SetAgentTarget(c.transform.position);
        }
        DontDestroyOnLoad(this);
        m_currentState = State.Attack;
        m_player.EnterArena(2);
    }
    private IEnumerator FreezeForT(float t)
    {
        Time.timeScale = 0f;
        yield return new WaitForSeconds(t);
        Time.timeScale = 1f;
    }
}*/