using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : Singleton_template<GameManager>
{
    [SerializeField] private int m_maxTurns = 100;
    [SerializeField] private float m_timePerTurn;

    [SerializeField] private List<Character> m_possibleEnemies;

    [SerializeField] private TextMeshProUGUI m_turnIndicator;
    [SerializeField] private Transform m_turnsDisplay;

    public bool[] m_turnOrder = null;
    public int m_turnCount = 0;

    public Character m_closestEnemy;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        CreateTurnOrder();
    }
    private void CreateTurnOrder()
    {
        m_turnOrder = new bool[m_maxTurns];
        for (int i = 0; i < m_maxTurns; i++)
        {
            m_turnOrder[i] = Random.Range(0, 2) == 0;
        }
    }
    private void Update()
    {
        if (m_turnOrder.Length > 0)
        {
            if (m_turnIndicator) m_turnIndicator.text = m_turnOrder[m_turnCount] ? "<color=#0000FF> Player Turn </color>" : "<color=#FF0000> Enemy Turn </color>";
            if (m_turnsDisplay)
            {
                Image[] images = m_turnsDisplay.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = m_turnOrder[m_turnCount + i] ? Color.blue : Color.red;
                }
            }
        }
    }
    public void GenerateTurnOrder()
    {
        m_turnOrder = new bool[m_maxTurns];
        for (int i = 0; i < m_maxTurns; i++)
        {
            m_turnOrder[i] = Random.Range(0f, 1f) >= 0.5f;
        }
        m_turnCount = 0;
    }
    public void EndTurn()
    {
        if (m_turnOrder[m_turnCount]) FindObjectOfType<Arena>().ResetPlayerTurn();
        if (!m_turnOrder[m_turnCount] || m_turnOrder[m_turnCount + 1 < m_maxTurns ? m_turnCount + 1 : 0]) FindObjectOfType<Arena>().SetPlayerToSelect();
        m_turnCount++;
        if (m_turnCount >= m_maxTurns) m_turnCount = 0;
    }
    public Character GrabRandomCharacter()
    {
        return m_possibleEnemies[(int) Random.Range(0f, m_possibleEnemies.Count)];
    }
    public void ReturnToMainGame()
    {
        SceneManager.LoadScene(1);
    }
}