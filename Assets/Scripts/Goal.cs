using UnityEngine;

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
