using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Character : MonoBehaviour
{
    [SerializeField] private float m_maxHp;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_resistance;
    [SerializeField] private AudioClip[] meleeSounds;

    public List<Attack> m_attacks = new();

    public Sprite m_ArenaSprite;

    public LayerMask m_enemyLayers;

    private float m_currentHp = 0f;
    private float m_vel = 0f;

    private Attack m_currentAttack = null;

    private Animator m_animator;
    private NavMeshAgent m_agent;

    private GameManager m_gameManager;
    private AudioSource attackSounds;

    void Start()
    {
        m_gameManager = GameManager.Instance();
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_agent.speed = m_speed;
        m_agent.acceleration = m_speed * 2.285f;
        m_currentHp = m_maxHp;
        attackSounds = GetComponent<AudioSource>();
        
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
    public void UseAttack(int i, bool isPlayer)
    {
        if (m_currentAttack == null)
            StartCoroutine(RunAttack(m_attacks[i], i, isPlayer));
    }
    private IEnumerator RunAttack(Attack a, int i, bool isPlayer)
    {
        if (a.m_currentUses < a.m_maxUses)
        {
            m_currentAttack = a;
            m_animator.SetInteger("AttackIndex", i);
            a.m_currentUses++;
            attackSounds.clip = meleeSounds[(int)Random.Range(0f, meleeSounds.Length - 1f)];
            attackSounds.Play();
            GameObject temp = new();
            if (a.VFX) temp = Instantiate(a.VFX, transform.position, transform.rotation);
            if (a.m_moveVFX) StartCoroutine(MoveVFX(temp.transform, transform.position + a.m_vfxOffset, 
                m_gameManager.m_currentEnemy.m_enemyCharacters[m_gameManager.m_currentEnemy.m_activeIndex].transform.position, a.m_timeOffset, (a.m_clip.averageDuration * a.m_speedMulti) - a.m_timeOffset));
            if (a.m_currentUses >= a.m_maxUses) StartCoroutine(a.Reset());
            yield return new WaitForSeconds(a.m_clip.averageDuration * a.m_speedMulti);
            if (isPlayer) m_gameManager.NearestEnemy.TakeDamage(a.m_damage);
            else m_gameManager.GetNearestCharacter(m_gameManager.m_playerCharacters, transform.position).TakeDamage(a.m_damage);
            m_currentAttack = null;
            m_animator.SetInteger("AttackIndex", -1);
            m_gameManager.EndTurn();
            Destroy(temp);
        }
    }
    private IEnumerator MoveVFX(Transform t, Vector3 start, Vector3 end, float offset, float durration)
    {
        float time = 0f;
        while (time < durration)
        {
            time += Time.deltaTime;
            if (time >= offset) t.position = Vector3.Slerp(start, end, durration / (time - offset));
            yield return null;
        }
    }
    public void TakeDamage(float d)
    {
        m_currentHp -= d;
        if (m_currentHp <= 0) Die();
        Debug.Log(gameObject.name + ":\nTook: " + d + " Damage " + "Hp: " + m_currentHp);
    }
    private void Die()
    {
        m_currentHp = 0;
        //m_gameManager.ReturnToMainGame();
    }
    public bool IsMoving { get {
            if (m_agent.pathPending) return false;
            if (m_agent.remainingDistance > m_agent.stoppingDistance && m_agent.velocity.sqrMagnitude > 0.01f)
            {
                return true;
            }
            return false;
        } }
    public float Health { get { return m_currentHp; } set { m_currentHp -= value; } }
    public float Speed { get { return m_speed; } }
    public float Resistance { get { return m_resistance; } }
}
[System.Serializable]
public class Attack
{
    public float m_damage;
    public float m_speedMulti;
    public float m_resetTurns;
    public AnimationClip m_clip;
    public float m_maxUses;
    public float m_currentUses;
    private int m_resetCount = 0;
    public AudioClip[] meleeSounds;
    public bool m_moveVFX;
    public float m_timeOffset;
    public Vector3 m_vfxOffset;
    public GameObject VFX;
    public void UpdateAfterUse()
    {
        if (m_currentUses == m_maxUses)
        {
            m_resetCount++;
            if (m_resetCount == m_resetTurns)
            {
                m_resetCount = 0;
                m_currentUses = 0;
            }
        }
    }
    public IEnumerator Reset()
    {
        yield return new WaitForSeconds(m_resetTurns);
        m_currentUses = 0;
    }
}