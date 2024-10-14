using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TempGameManager : Singleton_template<TempGameManager>
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
    public Vector3 AveragePlayerPosition;
    ///////////////
    [SerializeField] private float m_distanceBetweenCharacters;
    [SerializeField] private float m_lookAtEnemySpeed;
    ///////////////
    [SerializeField] private List<Character> m_allCharacterPrefabs;
    ///////////////
    [SerializeField] private string m_saveData;
    ///////////////
    private int m_activeCharacterIndex = 0;
    private List<Character> m_playerCharacters = new();
    ///////////////
    private List<TempEnemy> m_enemyList = new();
    private TempEnemy m_currentEnemy;
    private Character m_nearestEnemy;
    ///////////////
    private bool Mouse0;
    private bool Mouse0Down;
    private bool Mouse0Up;
    ///////////////
    private bool BackToSelect;
    private bool EndMovePhase;
    ///////////////
    public void AddEnemyToList(TempEnemy e)
    {
        m_enemyList.Add(e);
    }
    ///////////////
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (m_saveData == "")
            m_playerCharacters.Add(Instantiate(m_allCharacterPrefabs[0]));
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
        BackToSelect = Input.GetKey(KeyCode.E);
        EndMovePhase = Input.GetKey(KeyCode.R);

        if (BackToSelect && m_currentPlayerState != State.Fight) m_currentPlayerState = State.Select;
        switch (m_currentPlayerState)
        {
            case State.Explore:
                if (Mouse0Down) AveragePlayerPosition = MoveAllOnClick(); break;
            case State.Move:
                if (Mouse0Down) MoveCharacterOnClick(m_playerCharacters[m_activeCharacterIndex]);
                if (EndMovePhase) m_currentPlayerState = State.Fight;
                break;
            case State.Fight:
                m_nearestEnemy = GetNearestEnemy();
                m_playerCharacters[m_activeCharacterIndex].transform.forward
                    = Vector3.Slerp(m_playerCharacters[m_activeCharacterIndex].transform.forward,
                    m_nearestEnemy.transform.position - m_playerCharacters[m_activeCharacterIndex].transform.position,
                    m_lookAtEnemySpeed * Time.deltaTime);
                break;
        }
    }
    ///////////////
    public void ChangeState(State state)
    {
        m_currentPlayerState = state;
    }
    // Movement Methods ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Movement
    private Vector3 MoveAllOnClick()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            MoveAll(m_playerCharacters, hit.point);
        }
        return hit.point;
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
    private Character GetNearestEnemy()
    {
        float temp = Mathf.Infinity;
        Character closet = null;
        foreach (Character c in m_currentEnemy.m_enemyCharacters)
        {
            float d = Vector3.Distance(m_playerCharacters[m_activeCharacterIndex].transform.position, c.transform.position);
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
        //m_turnCount++;
        //if (m_turnCount >= m_maxTurns) m_turnCount = 0;
    }
}