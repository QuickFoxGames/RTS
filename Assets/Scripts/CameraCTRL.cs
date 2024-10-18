using UnityEngine;
public class CameraCTRL : MonoBehaviour
{
    [SerializeField] private float m_camRotOffset;
    [SerializeField] private Vector3 m_camPosOffset;

    [SerializeField] private float m_mouseSensitivity;
    [SerializeField] private float m_mouseScrollSensitivity;
    [SerializeField] private float m_minZoomDistance;
    [SerializeField] private Transform TargetPosition;

    private float m_yRot = 0f;

    private Vector3 m_originalOffset;

    private Camera m_camera;
    private GameManager m_gameManager;
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_gameManager = GameManager.Instance();
        m_originalOffset = transform.localPosition;
    }
    void Update()
    {
        transform.parent.position = TargetPosition ? TargetPosition.position : m_gameManager.AveragePlayerPosition(m_gameManager.m_playerCharacters);
        transform.LookAt(m_gameManager.AveragePlayerPosition(m_gameManager.m_playerCharacters) + m_camPosOffset);

        if (m_gameManager.Mouse1)
        {
            Cursor.visible = false;
            m_yRot += (m_gameManager.MouseX * m_mouseSensitivity * Time.fixedDeltaTime);
            transform.parent.rotation = Quaternion.Euler(0f, m_yRot + m_camRotOffset, 0f);
        }
        else Cursor.visible = true;
        if (m_gameManager.MouseZ != 0f)
        {
            if (Input.GetKey(KeyCode.R)) transform.position = m_originalOffset;
            else if (Vector3.Distance(transform.position + m_gameManager.MouseZ * m_mouseScrollSensitivity * transform.forward, m_gameManager.AveragePlayerPosition(m_gameManager.m_playerCharacters) + m_camPosOffset) >= m_minZoomDistance) transform.position += m_gameManager.MouseZ * m_mouseScrollSensitivity * transform.forward;
        }

        Vector3 dir = transform.position - (m_gameManager.AveragePlayerPosition(m_gameManager.m_playerCharacters) + m_camPosOffset);
        if (dir.magnitude <= m_minZoomDistance) transform.position = m_minZoomDistance * dir.normalized;
    }
}