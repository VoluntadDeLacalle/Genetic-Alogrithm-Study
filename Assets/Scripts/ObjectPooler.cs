using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler current;
    public int pooledAmount = 20;
    public bool willGrow = true;

    private int count = 1;

    List<GameObject> pooledObjects;

    public enum Key
    {
        Agent
    }

    public Key key;
    public GameObject pooledObject;

    public static Dictionary<Key, ObjectPooler> dict = new Dictionary<Key, ObjectPooler>();



    void Start()
    {
        dict[key] = this;

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

    public static ObjectPooler GetPooler(Key key)
    {
        return dict[key];
    }
}
