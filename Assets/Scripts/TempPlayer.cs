/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TempPlayer : Singleton_template<TempPlayer>
{
    public enum State
    {
        Explore,
        Select,
        Wait,
        Move,
        Fight
    }
    public State m_currentState = State.Explore;

    public bool BackToSelect { get; private set; }
    public bool EndMovePhase { get; private set; }
    public bool Mouse1 { get; private set; }
    public bool Mouse0 { get; private set; }
    public float MouseX { get; private set; }
    //public float MouseY { get; private set; }

    [SerializeField] private float m_distanceBetweenCharacters;
    [SerializeField] private List<Character> m_characters;
    [SerializeField] private Transform m_moveTargetMarker;

    public Character m_activeCharacter;

    private GameManager m_gameManager;
    void Start()
    {
        m_gameManager = GameManager.Instance();
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        MouseX = Input.GetAxisRaw("Mouse X");
        //MouseY = Input.GetAxisRaw("Mouse Y");
        Mouse0 = Input.GetKey(KeyCode.Mouse0);
        Mouse1 = Input.GetKey(KeyCode.Mouse1);
        BackToSelect = Input.GetKey(KeyCode.E);
        EndMovePhase = Input.GetKey(KeyCode.R);

        if (BackToSelect && m_currentState != State.Fight) m_currentState = State.Select;
        if (EndMovePhase) m_currentState = State.Fight;

        if (Mouse0) 
        { 
            switch (m_currentState)
            {
                case State.Explore:
                    MoveAllOnClick();
                    break;
                case State.Move:
                    if (m_gameManager.m_turnOrder[m_gameManager.m_turnCount]) MoveActiveOnClick();
                    break;
            }
        }
        if (m_currentState == State.Fight)
                    m_gameManager.m_closestEnemy = LookAtNearestEnemy();
    }
    public void ChangeState(State state)
    {
        m_currentState = state;
    }
    private void MoveAllOnClick()
    { // gets circular positions around the clicked destination and moves each character accordingly
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            float angleStep = 360f / m_characters.Count;
            for (int i = 0; i < m_characters.Count; i++)
            {
                float angleInRadians = Mathf.Deg2Rad * angleStep * i;
                float x = hit.point.x + m_distanceBetweenCharacters * Mathf.Cos(angleInRadians);
                float z = hit.point.z + m_distanceBetweenCharacters * Mathf.Sin(angleInRadians);

                m_characters[i].SetAgentTarget(new (x, hit.point.y, z));
            }
            m_moveTargetMarker.position = new(hit.point.x, 0f, hit.point.z);
        }
    }
    private void MoveActiveOnClick()
    { // moves the current active character to the clicked destination
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            m_activeCharacter.SetAgentTarget(hit.point);
            if (m_moveTargetMarker) m_moveTargetMarker.position = new(hit.point.x, 0f, hit.point.z);
        }
    }
    private Character LookAtNearestEnemy()
    {
        float temp = Mathf.Infinity;
        Vector3 dir = Vector3.zero;
        List<Character> clist = FindObjectOfType<Enemy>().m_characters;
        Character closet = null;
        foreach (Character c in clist)
        {
            float d = Vector3.Distance(m_activeCharacter.transform.position, c.transform.position);
            if (d < temp)
            {
                temp = d;
                dir = c.transform.position - m_activeCharacter.transform.position;
                closet = c;
            }
        }
        m_activeCharacter.transform.forward = dir.normalized;
        return closet;
    }
    public List<Character> Characters { get { return m_characters; } }
    public Vector3 AveragePosition { get
        {
            Vector3 temp = Vector3.zero;
            foreach(Character character in m_characters)
            {
                temp += character.transform.position;
            }
            return temp / m_characters.Count;
        } }

    public void EnterArena(int index)
    {
        foreach (var character in m_characters)
        {
            DontDestroyOnLoad(character);
        }
        m_currentState = State.Select;
        SceneManager.LoadScene(index);
    }
}*/