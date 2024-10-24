using UnityEngine;
using UnityEngine.UI;
public class Arena : MonoBehaviour
{
    [SerializeField] private GameObject m_buttonsParent;
    [SerializeField] private GameObject m_moveMenu;
    [SerializeField] private GameObject m_combatMenu;
    ///////////////
    private GameManager m_gameManager;
    private void Start()
    {
        m_gameManager = GameManager.Instance();
        foreach (Character c in m_gameManager.m_playerCharacters)
        {
            Vector3 temp = Vector3.zero;
            c.transform.SetPositionAndRotation(temp, Quaternion.identity);
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
    }
    void Update()
    {
        m_buttonsParent.SetActive(m_gameManager.m_currentPlayerState == GameManager.State.Select);
        m_moveMenu.SetActive(m_gameManager.m_currentPlayerState == GameManager.State.Move);
        m_combatMenu.SetActive(m_gameManager.m_currentPlayerState == GameManager.State.Fight);
    }
    public void UseAttack(int i)
    {
        ///if (!m_gameManager.m_turnOrder[m_gameManager.m_turnCount]) return;
        m_gameManager.m_playerCharacters[m_gameManager.m_activeCharacterIndex].UseAttack(i);
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