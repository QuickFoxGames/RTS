using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Arena : MonoBehaviour
{
    [SerializeField] private int m_numberOfCharacters;
    [SerializeField] private int m_collums;
    [SerializeField] private float m_distance;
    [SerializeField] private List<Character> m_enemies;

    private List<Character> m_characters;
    private Player m_player;
    private void Start()
    {
        m_player = Player.Instance();
        m_characters = new List<Character>();
        int count = 0;
        float offset = 0f;
        for (int i = 0; i < (m_numberOfCharacters < m_player.Characters.Count ? m_numberOfCharacters : m_player.Characters.Count); i++)
        {
            m_characters.Add(m_player.Characters[i]);
            m_characters[i].transform.position = ((m_distance * 0.5f * (m_collums-1)) - (count * m_distance)) * transform.forward + offset * transform.right + Vector3.up;
            count++;
            if (count > (m_collums-1))
            {
                count = 0;
                offset -= m_distance;
            }
        }

    }
    private void UpdateLayout()
    {
        int count = 0;
        float offset = 0f;
        for (int i = 0; i < (m_numberOfCharacters < m_player.Characters.Count ? m_numberOfCharacters : m_player.Characters.Count); i++)
        {
            m_characters[i].transform.position = transform.position + (((m_distance * 0.5f * (m_collums - 1)) - (count * m_distance)) * transform.forward) + (offset * transform.right);
            count++;
            if (count > (m_collums - 1))
            {
                count = 0;
                offset -= m_distance;
            }
        }
    }
    void Update()
    {
        
    }
    /*private void FixedUpdate()
    {
        UpdateLayout();
    }*/
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
}