using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Character : MonoBehaviour
{
    [SerializeField] private float m_maxHp;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_resistance;

    public List<Attack> m_attacks = new();

    public Sprite m_ArenaSprite;

    public LayerMask m_enemyLayers;

    private float m_currentHp = 0f;
    private float m_vel = 0f;

    private Attack m_currentAttack = null;

    private Animator m_animator;
    private NavMeshAgent m_agent;
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_agent.speed = m_speed;
        m_agent.acceleration = m_speed * 2.285f;
        m_currentHp = m_maxHp;
    }
    private void Update()
    {
        m_vel = m_agent.velocity.magnitude;
        float v = Mathf.Lerp(m_animator.GetFloat("Z"), m_vel, 10f * Time.deltaTime);
        m_animator.SetFloat("Z", v);
    }
    public void SetAgentTarget(Vector3 pos)
    {
        m_agent.SetDestination(pos);
    }
    public void WarpAgentPosition(Vector3 pos)
    {
        m_agent.Warp(pos);
    }
    public void UseAttack(int i)
    {
        if (m_currentAttack == null)
            StartCoroutine(RunAttack(m_attacks[i], i));
    }
    private IEnumerator RunAttack(Attack a, int i)
    {
        if (a.m_currentUses < a.m_maxUses)
        {
            m_animator.SetInteger("AttackIndex", i);
            m_currentAttack = a;
            a.m_currentUses++;
            if (a.m_currentUses >= a.m_maxUses) StartCoroutine(a.Reset());
            yield return new WaitForSeconds(a.m_clip.averageDuration * a.m_speedMulti);
            m_currentAttack = null;
            m_animator.SetInteger("AttackIndex", -1);
            Collider[] cs = Physics.OverlapBox(transform.position + transform.forward, Vector3.one, Quaternion.identity, m_enemyLayers);
            if (cs.Length > 0)
            {
                foreach (Collider c in cs)
                {
                    c.GetComponent<Character>().TakeDamage(a.m_damage);
                }
            }
        }
    }
    public void TakeDamage(float d)
    {
        m_currentHp -= d;
        if (m_currentHp <= 0) Die();
    }
    private void Die()
    {
        m_currentHp = 0;
    }
    public bool IsMoving { get { return !m_agent.isStopped; } }
    public float Health { get { return m_currentHp; } set { m_currentHp -= value; } }
    public float Speed { get { return m_speed; } }
    public float Resistance { get { return m_resistance; } }
}
[System.Serializable]
public class Attack
{
    public float m_damage;
    public float m_speedMulti;
    public float m_resetTime;
    public AnimationClip m_clip;
    public float m_maxUses;
    public float m_currentUses;
    public IEnumerator Reset()
    {
        yield return new WaitForSeconds(m_resetTime);
        m_currentUses = 0;
    }
}