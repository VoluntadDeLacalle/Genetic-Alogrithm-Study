using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    private ObjectPooler.Key agentKey = ObjectPooler.Key.Agent;

    void Start()
    {
        GenerationManager.instance.currentPopulation = new List<GameObject>();

        InitializePopulation();
    }

    void InitializePopulation()
    {
        for (int i = 0; i < GenerationManager.instance.populationAmount; i++)
        {
            GameObject pooledObj = ObjectPooler.GetPooler(agentKey).GetPooledObject();
            pooledObj.transform.position = GenerationManager.instance.populationStartingPosition;

            pooledObj.SetActive(true);

            GenerationManager.instance.currentPopulation.Add(pooledObj);
        }

        for (int i = 0; i < GenerationManager.instance.currentPopulation.Count; i++)
        {
            for (int j = i; j < GenerationManager.instance.currentPopulation.Count; j++)
            {
                Physics.IgnoreCollision(GenerationManager.instance.currentPopulation[i].gameObject.GetComponent<SphereCollider>(), GenerationManager.instance.currentPopulation[j].gameObject.GetComponent<SphereCollider>(), true);
            }
        }
    }
}
