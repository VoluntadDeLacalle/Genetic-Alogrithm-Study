using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Population;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - new Vector3(0, 0.5f, 0);
    }

    void LateUpdate()
    {
        transform.position = Population.GetComponent<Population>().population[Population.GetComponent<Population>().furtherestIndex].transform.position + offset;
    }

}
