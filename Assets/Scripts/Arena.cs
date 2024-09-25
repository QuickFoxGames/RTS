using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Arena : MonoBehaviour
{
    [SerializeField] private int m_numberOfCharacters;
    [SerializeField] private int m_collums;
    [SerializeField] private float m_distance;
    [SerializeField] private List<Transform> m_spawnSpots;
    [SerializeField] private GameObject m_buttonParent;
    [SerializeField] private GameObject m_combatMenu;

    private Player m_player;
    private void Start()
    {
        m_player = Player.Instance();
        //SetLayout();
        foreach (var character in m_player.Characters)
        {
            Transform temp = PickSpawn();
            character.transform.SetPositionAndRotation(temp.position, temp.rotation);
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
        m_combatMenu.SetActive(m_player.m_currentState == Player.State.Fight);
    }
    public void UsesAttackOn(Character attacker, Character defender)
    {
        float diff = Mathf.Abs(attacker.Speed - defender.Speed);
        float rand = Random.Range(attacker.Speed, defender.Speed);
        if (rand < Mathf.Abs(attacker.Speed - defender.Speed))
        {
            float finalDamage = attacker.Damage;
            finalDamage *= defender.Resistance / 100f;
        }
    }
    public void SetActiveCharacter(Character character)
    {
        m_player.m_activeCharacter = character;
        m_player.ChangeState(Player.State.Move);
        m_buttonParent.gameObject.SetActive(false);
    }
}