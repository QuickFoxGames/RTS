using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : Singleton_template<Player>
{
    [SerializeField] private float m_distanceBetweenCharacters;
    [SerializeField] private List<Character> m_characters;
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) MoveOnClick();
    }
    private void MoveOnClick()
    {
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
        }
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
}