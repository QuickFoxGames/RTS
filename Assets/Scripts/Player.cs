using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Character m_mainCharacter;
    private GameManager m_gameManager;
    void Start()
    {
        m_gameManager = GameManager.Instance();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) MoveOnClick();
    }
    private void MoveOnClick()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) m_mainCharacter.SetAgentTarget(hit.point);
    }
}