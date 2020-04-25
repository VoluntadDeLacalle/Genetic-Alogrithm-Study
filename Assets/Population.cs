using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public GameObject spawnObject;
    public int populationAmount = 0;
    public int furtherestIndex = 0;

    public List<GameObject> population;

    void Start()
    {
        population = new List<GameObject>();

        InitializePopulation();
    }

    void InitializePopulation()
    {
        for (int i = 0; i < populationAmount; i++)
        {
            GameObject obj = Instantiate(spawnObject, new Vector3(0, .5f, 0), transform.rotation);

            population.Add(obj);
        }

        for (int i = 0; i < population.Count; i++)
        {
            for (int j = 0; j < population.Count; j++)
            {
                Physics.IgnoreCollision(population[i].gameObject.GetComponent<SphereCollider>(), population[j].gameObject.GetComponent<SphereCollider>(), true);
            }
        }
    }

    void Update()
    {
        FindFurthestSubject();
    }

    void FindFurthestSubject()
    {
        float maxValue = population[0].GetComponent<CubeController>().distanceFromStart;
        for (int i = 0; i < population.Count; i++)
        {
            if (maxValue <= population[i].GetComponent<CubeController>().distanceFromStart && population[i].GetComponent<CubeController>().transform.position.x >= 0) {
                maxValue = population[i].GetComponent<CubeController>().distanceFromStart;
                furtherestIndex = i;
            }
        }
    }
}
