using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler current;
    private int pooledAmount = 20;
    private bool willGrow = true;

    private int count = 1;

    List<GameObject> pooledObjects;

    public enum Key
    {
        Agent
    }

    public Key key;
    public GameObject pooledObject;

    public static Dictionary<Key, ObjectPooler> dict = new Dictionary<Key, ObjectPooler>();


    /// <summary>
    /// Instantiates a pool of desired game objects in the amount equal to the GenerationManager's population amount.
    /// </summary>
    void Start()
    {
        dict[key] = this;

        pooledAmount = GenerationManager.instance.populationAmount;
        willGrow = true;

        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.transform.parent = this.transform;
            obj.name = pooledObject.name + " " + (count);
            count++;

            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    /// <summary>
    /// Returns a pooled Gameobject, in this case an agent, if it is not already active in the scene. If there are not enough
    /// already pooled, it will create more to satisfy the amount needed.
    /// </summary>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.transform.parent = this.transform;
            obj.name = pooledObject.name + " " + (count);
            count++;

            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    /// <summary>
    /// Returns an ObjectPooler object that corresponds to the correct enum. The only enum in the project
    /// is agent, and it relates to the Agent prefab.
    /// </summary>
    /// <param name="key">Enum of ObjectPooler.Key.</param>
    public static ObjectPooler GetPooler(Key key)
    {
        return dict[key];
    }
}
