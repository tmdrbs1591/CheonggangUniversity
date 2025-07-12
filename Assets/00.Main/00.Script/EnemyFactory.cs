using UnityEngine;
public enum EnemyType
{
    Robot,
    Drone,
    LightningDrone,
    LightningRobot
}

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject lightningDronePrefab;
    [SerializeField] private GameObject lightningRobotPrefab;

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
                prefab = lightningDronePrefab;
                break;
            case EnemyType.LightningRobot:
                prefab = lightningRobotPrefab;
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
