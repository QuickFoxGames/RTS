using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraCTRL : MonoBehaviour
{
    [SerializeField] private Vector3 m_offset;
    private Camera m_camera;
    private Player m_player;
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_player = Player.Instance();
    }
    void Update()
    {
        transform.position = m_player.AveragePosition + m_offset;
        transform.LookAt(m_player.AveragePosition);
    }
}