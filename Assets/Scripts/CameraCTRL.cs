using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class CameraCTRL : MonoBehaviour
{
    [SerializeField] private float m_camRotOffset;
    [SerializeField] private float m_mouseSensitivity;
    [SerializeField] private Transform m_targetPosition;

    private float m_yRot = 0f;

    private Camera m_camera;
    private Player m_player;
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_player = Player.Instance();
    }
    void Update()
    {
        transform.parent.position = !m_targetPosition ? m_player.AveragePosition : m_targetPosition.position;
        transform.LookAt(m_player.AveragePosition);
        if (m_player.Mouse1)
        {
            Cursor.visible = false;
            m_yRot += (m_player.MouseX * m_mouseSensitivity * Time.fixedDeltaTime);
            transform.parent.rotation = Quaternion.Euler(0f, m_yRot + m_camRotOffset, 0f);
        }
        else Cursor.visible = true;
    }
}