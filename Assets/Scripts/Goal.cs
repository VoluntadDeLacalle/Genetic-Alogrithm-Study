using UnityEngine;


/// <summary>
/// This script updates the goal at the end of the simulation and
/// makes it transparent.
/// </summary>
public class Goal : MonoBehaviour
{
    public Color reachedColor;
    
    MeshRenderer mr;
    bool hasChanged = false;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (!hasChanged && GenerationManager.instance.goalReached)
        {
            mr.material.color = reachedColor;
            hasChanged = true;
        }   
    }
}
