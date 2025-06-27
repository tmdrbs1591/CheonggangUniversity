using System.Collections;
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

    [SerializeField] private float spawnDelay = 0.8f;       // ¿Ã∆Â∆Æ »ƒ µÙ∑π¿Ã

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
                    Transform spawnPoint = data.spawnPoints[i % pointCount];
                    StartCoroutine(SpawnWithEffect(data.enemyType, spawnPoint));
                }
            }
        }
    }

    private IEnumerator SpawnWithEffect(EnemyType type, Transform spawnPoint)
    {
        GameObject effect = ObjectPool.SpawnFromPool("SpawnEffect", spawnPoint.position);
        yield return new WaitForSeconds(spawnDelay);

        factory.CreateEnemy(type, spawnPoint.position);
    }

}
