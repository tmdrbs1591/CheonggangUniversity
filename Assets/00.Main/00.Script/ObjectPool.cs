using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public int poolCount; 
    public string poolName; 

    public int poolLength => pool.Count;

    public GameObject poolObject; 
    public Transform parentObject;

    private Queue<GameObject> pool = new Queue<GameObject>(); 

    public void Enqueue(GameObject _object) => pool.Enqueue(_object);
    public GameObject Dequeue() => pool.Dequeue();
}


public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance = null;

    public Dictionary<string, Pool> poolDictionary = new Dictionary<string, Pool>();

    public List<Pool> poolList = new List<Pool>();

    #region Unity_Function
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start() => _Init();
    #endregion

    #region Private_Fucntion
    private void _Init()
    {
        foreach (Pool pool in poolList)
            poolDictionary.Add(pool.poolName, pool);

        foreach (Pool pool in poolDictionary.Values) 
        {
            GameObject parent = new GameObject();

            pool.parentObject = parent.transform; 
            parent.transform.SetParent(transform); 
            parent.name = pool.poolName; 

            for (int i = 0; i < pool.poolCount; i++)
            {
                GameObject currentObject = Instantiate(pool.poolObject, parent.transform); 
                currentObject.SetActive(false);

                pool.Enqueue(currentObject); 
            }
        }
    }

    private GameObject _SpawnFromPool(string name, Vector3 position)
    {
        Pool currentPool = poolDictionary[name]; 
        if (currentPool.poolLength <= 0)
        {
            GameObject obj = Instantiate(currentPool.poolObject, currentPool.parentObject);
            obj.SetActive(false);
            currentPool.Enqueue(obj);
        }

        GameObject currentObject = currentPool.Dequeue(); 
        currentObject.transform.position = position;

        currentObject.SetActive(true); 

        return currentObject;
    }

    private GameObject _SpawnFromPool(string name, Vector3 position, Quaternion rotate)
    {
        Pool currentPool = poolDictionary[name]; 

        if (currentPool.poolLength <= 0)
        {
            GameObject obj = Instantiate(currentPool.poolObject, currentPool.parentObject);
            obj.SetActive(false);
            currentPool.Enqueue(obj);
        }

        GameObject currentObject = currentPool.Dequeue(); 
        currentObject.transform.position = position;
        currentObject.transform.rotation = rotate; 

        currentObject.SetActive(true);

        return currentObject;
    }

    private void _ReturnToPool(string name, GameObject currentObject)
    {
        Pool pool = poolDictionary[name]; 

        currentObject.SetActive(false); 
        currentObject.transform.SetParent(pool.parentObject);
        pool.Enqueue(currentObject);
    }
    #endregion  

    #region Public_Function
    public static GameObject SpawnFromPool(string name, Vector3 position) => instance._SpawnFromPool(name, position);
    public static GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotate) => instance._SpawnFromPool(name, position, rotate);

    public static void ReturnToPool(string name, GameObject currentObejct) => instance._ReturnToPool(name, currentObejct);
    #endregion

}