using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int m_numEnemies;
    ///////////////
    [SerializeField] private float m_timeBetweenSpawns;
    ///////////////
    [SerializeField] private LayerMask m_playerLayers;
    ///////////////
    [SerializeField] private TempEnemy m_enemyPrefab;
    ///////////////
    private int m_enemyCount = 0;
    ///////////////
    private TempGameManager m_gameManager;
    ///////////////
    private void Start()
    {
        m_gameManager = TempGameManager.Instance();
    }
    ///////////////
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == m_playerLayers && m_numEnemies > 0)
            StartCoroutine(SpawnEnemies());
    }
    ///////////////
    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(m_enemyCount > 0 ? m_timeBetweenSpawns : 0f);
        m_gameManager.AddEnemyToList(Instantiate(m_enemyPrefab));
        m_enemyCount++;
        if (m_enemyCount < m_numEnemies) StartCoroutine(SpawnEnemies());
    }
}