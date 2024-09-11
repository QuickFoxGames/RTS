using UnityEngine;
using UnityEngine.AI;
public class Character : MonoBehaviour
{
    [SerializeField] private float m_maxHp;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_damage;
    [SerializeField] private float m_resistance;

    private float m_currentHp = 0f;
    private float m_vel = 0f;

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
    public float Health { get { return m_currentHp; } set { m_currentHp -= value; } }
    public float Speed { get { return m_speed; } }
    public float Damage { get { return m_damage; } }
    public float Resistance { get { return m_resistance; } }
}