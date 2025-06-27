using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public EnemyType enemyType;
    public Transform[] spawnPoints;
}


public class EnemySpawnTrigger : MonoBehaviour
{
    [SerializeField] private EnemyFactory factory;
    [SerializeField] private List<EnemySpawnData> spawnDataList;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;

            foreach (var data in spawnDataList)
            {
                int pointCount = data.spawnPoints.Length;

                for (int i = 0; i < pointCount; i++)
                {
                    // spawnCount > spawnPoints.Length이면 다시 순환
                    Transform spawnPoint = data.spawnPoints[i % pointCount];
                    factory.CreateEnemy(data.enemyType, spawnPoint.position);
                }
            }
        }
    }
}
