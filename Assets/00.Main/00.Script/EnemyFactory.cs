using UnityEngine;
public enum EnemyType
{
    Robot,
    Drone,
    LightningDrone
}

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject lightningDroneDronePrefab;

    public EnemyBase CreateEnemy(EnemyType type, Vector3 position)
    {
        GameObject prefab = null;

        switch (type)
        {
            case EnemyType.Robot:
                prefab = robotPrefab;
                break;
            case EnemyType.Drone:
                prefab = dronePrefab;
                break;
            case EnemyType.LightningDrone:
                prefab = lightningDroneDronePrefab;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError("프리팹이 없음");
            return null;
        }

        GameObject enemyObj = Instantiate(prefab, position, Quaternion.identity);
        EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
        return enemy;
    }
}
