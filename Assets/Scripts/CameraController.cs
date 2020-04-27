using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 offset;
    private int fitIndex = 0;

    void Start()
    {
        offset = transform.position - GenerationManager.instance.populationStartingPosition;
    }

    void Update()
    {
        if (!GenerationManager.instance.timeLimitReached)
        {
            fitIndex = GenerationManager.instance.FindFittestSubject();
        }
        else
        {
            fitIndex = 0;
        }
    }

    void LateUpdate()
    {
        if (!GenerationManager.instance.timeLimitReached)
        {
            transform.position = GenerationManager.instance.currentPopulation[fitIndex].transform.position + offset;
        }
    }

}
