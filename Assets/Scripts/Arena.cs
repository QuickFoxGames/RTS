using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Arena : MonoBehaviour
{
    [SerializeField] private GameObject m_buttonsParent;
    [SerializeField] private GameObject m_moveMenu;
    [SerializeField] private GameObject m_combatMenu;
    [SerializeField] private List<Transform> m_playerSpawns;
    [SerializeField] private List<Transform> m_enemySpawns;
    ///////////////
    private GameManager m_gameManager;
    private void Start()
    {
        m_gameManager = GameManager.Instance();
        foreach (Character c in m_gameManager.m_playerCharacters)
        {
            var temp = m_playerSpawns[Random.Range(0, m_playerSpawns.Count)];
            m_playerSpawns.Remove(temp);
            c.transform.SetPositionAndRotation(temp.position, Quaternion.identity);
            c.WarpAgentPosition(c.transform.position);
            GameObject g = new()
            {
                name = c.name + "_Button"
            };
            g.transform.parent = m_buttonsParent.transform;
            g.AddComponent<CanvasRenderer>();
            Image im = g.AddComponent<Image>();
            im.sprite = c.m_ArenaSprite;
            Button b = g.AddComponent<Button>();
            b.targetGraphic = im;
            b.onClick.AddListener(() => SetActiveCharacter(c));
        }
        foreach (Character c in m_gameManager.m_currentEnemy.m_enemyCharacters)
        {
            var temp = m_enemySpawns[Random.Range(0, m_playerSpawns.Count)];
            m_enemySpawns.Remove(temp);
            c.transform.SetPositionAndRotation(temp.position, Quaternion.identity);
            c.WarpAgentPosition(c.transform.position);
        }
    }
    void Update()
    {
        m_buttonsParent.SetActive(m_gameManager.m_currentPlayerState == GameManager.State.Select);
        m_moveMenu.SetActive(m_gameManager.m_currentPlayerState == GameManager.State.Move);
        m_combatMenu.SetActive(m_gameManager.m_currentPlayerState == GameManager.State.Fight);
        if (m_gameManager.m_currentPlayerState != GameManager.State.Fight)
        {
            m_combatMenu.transform.GetChild(0).gameObject.SetActive(true);
            m_combatMenu.transform.GetChild(1).gameObject.SetActive(true);
            m_combatMenu.transform.GetChild(2).gameObject.SetActive(false);
            m_combatMenu.transform.GetChild(3).gameObject.SetActive(false);
        }
    }
    public void UseAttack(int i)
    {
        ///if (!m_gameManager.m_turnOrder[m_gameManager.m_turnCount]) return;
        m_gameManager.m_playerCharacters[m_gameManager.m_activeCharacterIndex].UseAttack(i, true);
    }
    public void SetActiveCharacter(Character character)
    {
        if (m_gameManager.m_turnOrder[m_gameManager.m_turnIndex])
        {
            m_gameManager.m_activeCharacterIndex = m_gameManager.m_playerCharacters.IndexOf(character);
            m_gameManager.m_currentPlayerState = GameManager.State.Move;
        }
    }
    public void ToggleMenu(GameObject menuToToggle)
    {
        menuToToggle.SetActive(!menuToToggle.activeInHierarchy);
    }
}
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Arena : MonoBehaviour
{
    [SerializeField] private int m_numberOfCharacters;
    [SerializeField] private int m_collums;
    [SerializeField] private float m_distance;
    [SerializeField] private List<Transform> m_spawnSpots;
    [SerializeField] private GameObject m_buttonParent;
    [SerializeField] private GameObject m_moveMenu;
    [SerializeField] private GameObject m_combatMenu;

    private Player m_player;
    private GameManager m_gameManager;
    private void Start()
    {
        m_player = Player.Instance();
        m_gameManager = GameManager.Instance();
        m_gameManager.GenerateTurnOrder();
        //SetLayout();
        foreach (var character in m_player.Characters)
        {
            Transform temp = PickSpawn();
            character.transform.SetPositionAndRotation(temp.position, temp.rotation);
            character.WarpAgentPosition(character.transform.position);
            m_spawnSpots.Remove(temp);
            GameObject g = new()
            {
                name = character.name + "_Button"
            };
            g.transform.parent = m_buttonParent.transform;
            g.AddComponent<CanvasRenderer>();
            Image im = g.AddComponent<Image>();
            im.sprite = character.m_ArenaSprite;
            Button b = g.AddComponent<Button>();
            b.targetGraphic = im;
            b.onClick.AddListener(() => SetActiveCharacter(character));
        }
    }
    private Transform PickSpawn()
    {
        Transform spawn = m_spawnSpots[(int)Random.Range(0f, m_spawnSpots.Count)];
        if (spawn == null) PickSpawn();
        return spawn;
    }
    void Update()
    {
        m_buttonParent.SetActive(m_player.m_currentState == Player.State.Select);
        m_moveMenu.SetActive(m_player.m_currentState == Player.State.Move);
        m_combatMenu.SetActive(m_player.m_currentState == Player.State.Fight);
    }
    public void UseAttack(int i)
    {
        if (!m_gameManager.m_turnOrder[m_gameManager.m_turnCount]) return;
        m_player.m_activeCharacter.UseAttack(i);
    }
    public void SetActiveCharacter(Character character)
    {
        m_player.m_activeCharacter = character;
        m_player.ChangeState(Player.State.Move);
        m_buttonParent.SetActive(false);
    }
    public void ToggleMenu(GameObject menuToToggle)
    {
        menuToToggle.SetActive(!menuToToggle.activeInHierarchy);
    }
    public void ResetPlayerTurn()
    {
        m_buttonParent.SetActive(false);
        m_moveMenu.SetActive(false);
        m_combatMenu.SetActive(false);
        m_player.m_currentState = Player.State.Wait;
    }
    public void SetPlayerToSelect()
    {
        m_buttonParent.SetActive(true);
        m_player.m_currentState = Player.State.Select;
    }
    public void ExitArena(int index)
    {
        m_player.m_currentState = Player.State.Explore;
        // destroy enemies
        SceneManager.LoadScene(index);
    }
}*/