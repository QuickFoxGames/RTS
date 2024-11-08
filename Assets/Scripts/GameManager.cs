using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : Singleton_template<GameManager>
{
    public enum State
    {
        Explore,
        Select,
        Wait,
        Move,
        Fight
    }
    public State m_currentPlayerState = State.Explore;
    public bool[] m_turnOrder; // true == player turn
    public int m_turnIndex = 0;
    public Vector3 AveragePlayerPosition(List<Character> clist)
    {
        Vector3 temp = Vector3.zero;
        foreach (Character c in clist) 
        { 
            temp += c.transform.position;
        }
        return temp / clist.Count;
    }
    ///////////////
    [SerializeField] private float m_distanceBetweenCharacters;
    [SerializeField] private float m_lookAtEnemySpeed;
    ///////////////
    [SerializeField] private List<Character> m_allCharacterPrefabs;
    ///////////////
    [SerializeField] private string m_saveData = "";
    ///////////////
    [SerializeField] private GameObject m_turnsDisplay;
    ///////////////
    public int m_activeCharacterIndex = 0;
    public List<Character> m_playerCharacters = new();
    ///////////////
    private List<Enemy> m_enemyList = new();
    public Enemy m_currentEnemy;
    public Character NearestEnemy { get; private set; }
    ///////////////
    public bool Mouse0 { get; private set; }
    public bool Mouse0Down { get; private set; }
    public bool Mouse0Up { get; private set; }
    ///////////////
    public bool Mouse1 { get; private set; }
    public bool Mouse1Down { get; private set; }
    public bool Mouse1Up { get; private set; }
    ///////////////
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public float MouseZ { get; private set; }
    ///////////////
    private bool BackToSelect;
    private bool EndMovePhase;
    ///////////////
    public CameraCTRL m_cameraCtrl;
    ///////////////
    public void AddEnemyToList(Enemy e)
    {
        m_enemyList.Add(e);
    }
    ///////////////
    void Start()
    {
        m_cameraCtrl = FindObjectOfType<CameraCTRL>();
        DontDestroyOnLoad(m_cameraCtrl.transform.parent.gameObject);
        DontDestroyOnLoad(gameObject);
        m_turnOrder = new bool[100];
        if (m_saveData == "")
        {
            m_playerCharacters.Add(Instantiate(m_allCharacterPrefabs[0]));
            //m_playerCharacters.Add(Instantiate(m_allCharacterPrefabs[1]));
            for (int i = 0; i < m_turnOrder.Length; i++)
            {
                m_turnOrder[i] = Random.Range(0f, 1f) > 0.5f;
            }
        }
        else LoadSaveData();
    }
    ///////////////
    public List<Character> AllCharacterPrefabs() { return m_allCharacterPrefabs; }
    ///////////////
    private void LoadSaveData() { }
    ///////////////
    void Update()
    {
        Mouse0 = Input.GetKey(KeyCode.Mouse0);
        Mouse0Down = Input.GetKeyDown(KeyCode.Mouse0);
        Mouse0Up = Input.GetKeyUp(KeyCode.Mouse0);
        ///////////////
        Mouse1 = Input.GetKey(KeyCode.Mouse1);
        Mouse1Down = Input.GetKeyDown(KeyCode.Mouse1);
        Mouse1Up = Input.GetKeyUp(KeyCode.Mouse1);
        ///////////////
        MouseX = Input.GetAxisRaw("Mouse X");
        MouseY = Input.GetAxisRaw("Mouse Y");
        ///////////////
        MouseZ = Input.GetAxisRaw("Mouse ScrollWheel");
        ///////////////
        BackToSelect = Input.GetKey(KeyCode.E);
        EndMovePhase = Input.GetKey(KeyCode.R);
        ///////////////
        if (BackToSelect && m_currentPlayerState != State.Fight) m_currentPlayerState = State.Select;
        switch (m_currentPlayerState)
        {
            case State.Explore:
                if (Mouse0Down) MoveAllOnClick(); break;
            case State.Move:
                if (Mouse0Down && m_turnOrder[m_turnIndex]) MoveCharacterOnClick(m_playerCharacters[m_activeCharacterIndex]);
                if (EndMovePhase && m_turnOrder[m_turnIndex]) m_currentPlayerState = State.Fight;
                break;
            case State.Fight:
                if (m_turnOrder[m_turnIndex])
                {
                    NearestEnemy = GetNearestCharacter(m_currentEnemy.m_enemyCharacters, m_playerCharacters[m_activeCharacterIndex].transform.position);
                    m_playerCharacters[m_activeCharacterIndex].transform.forward
                        = Vector3.Slerp(m_playerCharacters[m_activeCharacterIndex].transform.forward,
                        NearestEnemy.transform.position - m_playerCharacters[m_activeCharacterIndex].transform.position,
                        m_lookAtEnemySpeed * Time.deltaTime);
                }
                break;
        }
        ///////////////
        if (m_turnOrder.Length > 0)
        {
            //if (m_turnIndicator) m_turnIndicator.text = m_turnOrder[m_turnCount] ? "<color=#0000FF> Player Turn </color>" : "<color=#FF0000> Enemy Turn </color>";
            if (m_turnsDisplay)
            {
                Image[] images = m_turnsDisplay.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = m_turnOrder[m_turnIndex + i] ? Color.blue : Color.red;
                }
            }
        }
    }
    ///////////////
    public void ChangeState(State state)
    {
        m_currentPlayerState = state;
    }
    // Movement Methods ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Movement
    private void MoveAllOnClick()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            MoveAll(m_playerCharacters, hit.point);
        }
    }
    ///////////////
    public void MoveAll(List<Character> characters, Vector3 pos)
    {
        float angleStep = 360f / characters.Count;
        for (int i = 0; i < characters.Count; i++)
        {
            float angleInRadians = Mathf.Deg2Rad * angleStep * i;
            float x = pos.x + m_distanceBetweenCharacters * characters.Count * Mathf.Cos(angleInRadians);
            float z = pos.z + m_distanceBetweenCharacters * characters.Count * Mathf.Sin(angleInRadians);

            characters[i].SetAgentTarget(new(x, pos.y, z));
        }
    }
    ///////////////
    private Vector3 MoveCharacterOnClick(Character c)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            c.SetAgentTarget(hit.point);
            //if (m_moveTargetMarker) m_moveTargetMarker.position = new(hit.point.x, 0f, hit.point.z);
        }
        return hit.point;
    }
    #endregion
    ///////////////
    public Character GetNearestCharacter(List<Character> clist, Vector3 activePosition)
    {
        float temp = Mathf.Infinity;
        Character closet = null;
        foreach (Character c in clist)
        {
            float d = Vector3.Distance(activePosition, c.transform.position);
            if (d < temp)
            {
                temp = d;
                closet = c;
            }
        }
        return closet;
    }
    ///////////////
    public void EndTurn()
    {
        m_turnIndex++;
        if (m_turnIndex >= 100) m_turnIndex = 0;
        m_currentPlayerState = State.Select;
    }
    public void EnterArena(Enemy e)
    {
        foreach (Character c in m_playerCharacters)
        {
            DontDestroyOnLoad(c);
        }
        m_currentPlayerState = State.Select;
        m_currentEnemy = e;
        SceneManager.LoadScene(2);
    }
    public void ExitArena()
    {
        foreach (Enemy e in m_enemyList)
        {
            foreach (Character c in e.m_enemyCharacters)
            {
                Destroy(c.gameObject);
            }
            Destroy(e.gameObject);
        }
        SceneManager.LoadScene(1);
    }
}