using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : Singleton_template<Player>
{
    private enum State
    {
        Explore,
        Fight
    }
    private State m_currentState = State.Explore;

    public bool Mouse1 { get; private set; }
    public bool Mouse0 { get; private set; }
    public float MouseX { get; private set; }
    //public float MouseY { get; private set; }

    [SerializeField] private float m_distanceBetweenCharacters;
    [SerializeField] private List<Character> m_characters;

    private Character m_activeCharacter;
    void Start()
    {
        m_activeCharacter = m_characters[0];
    }
    void Update()
    {
        MouseX = Input.GetAxisRaw("Mouse X");
        //MouseY = Input.GetAxisRaw("Mouse Y");
        Mouse0 = Input.GetKey(KeyCode.Mouse0);
        Mouse1 = Input.GetKey(KeyCode.Mouse1);

        if (Mouse0) 
        { 
            switch (m_currentState)
            {
                case State.Explore:
                    MoveAllOnClick();
                    break;
                case State.Fight:
                    MoveActiveOnClick();
                    break;
            }
        }
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
        }
    }
    private void MoveActiveOnClick()
    { // moves the current active character to the clicked destination
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            m_activeCharacter.SetAgentTarget(hit.point);
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