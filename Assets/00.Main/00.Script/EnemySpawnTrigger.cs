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
    [Header("스폰 설정")]
    [SerializeField] private EnemyFactory factory;
    [SerializeField] private List<EnemySpawnData> spawnDataList;
    [SerializeField] private float spawnDelay = 0.8f;
    [SerializeField] private int cycle = 1; // 몇 번 반복 생성할지 (최초 포함)

    private List<EnemyBase> spawnedEnemies = new List<EnemyBase>();
    private int currentCycle;
    private bool hasTriggered = false;

    private void Start()
    {
        currentCycle = cycle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;
            SpawnAllEnemies();
        }
    }

    private void SpawnAllEnemies()
    {
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

    private IEnumerator SpawnWithEffect(EnemyType type, Transform spawnPoint)
    {
        GameObject effect = ObjectPool.SpawnFromPool("SpawnEffect", spawnPoint.position);

        yield return new WaitForSeconds(spawnDelay);

        EnemyBase enemy = factory.CreateEnemy(type, spawnPoint.position);
        if (enemy != null)
        {
            spawnedEnemies.Add(enemy);
            enemy.OnDeath += OnEnemyDied;
        }
    }

    private void OnEnemyDied(EnemyBase deadEnemy)
    {
        if (spawnedEnemies.Contains(deadEnemy))
        {
            spawnedEnemies.Remove(deadEnemy);
        }

        if (spawnedEnemies.Count == 0)
        {
            currentCycle--;

            if (currentCycle > 0)
            {
                Debug.Log($"사이클 반복 남음: {currentCycle}");
                SpawnAllEnemies();
            }
            else
            {
                OnAllEnemiesDead();
            }
        }
    }

    private void OnAllEnemiesDead()
    {
        Debug.Log("모든 적이 제거되었습니다! 사이클 종료!");
        // 문 열기, 퀘스트 진행 등 추가 처리 가능
    }
}
