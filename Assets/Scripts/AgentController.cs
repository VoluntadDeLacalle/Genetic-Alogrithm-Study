﻿using UnityEngine;

public class AgentController : MonoBehaviour
{
    public Chromosome chromosome;
    private int chromosomeIndex = 0;

    public float timeLimit = 0;
    public float timeBuffer = 2;
    public float speed = 0;
    public float maxSpeed = 0;
    public float jumpForce = 0;

    public float distanceFromStart = 0;

    public bool isGrounded = true;
    private bool lastGeneAdded = false;
    private bool hasJumped = false;

    Rigidbody rb;

    MeshFilter mf;
    float overlapSphereRadius = 0;
    private Vector3 overlapSphereColliderPosition = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mf = GetComponent<MeshFilter>();

        chromosome = new Chromosome();
        chromosome.InitializeChromosome();

        overlapSphereRadius = mf.mesh.bounds.extents.x - 0.01f;

        timeLimit = Time.time;
    }

    void OnEnable()
    {
        timeLimit = Time.time;
        chromosomeIndex = 0;
        distanceFromStart = 0;
        isGrounded = true;
        hasJumped = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }   
    }

    void OnDisable()
    {
        distanceFromStart = 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(overlapSphereColliderPosition, overlapSphereRadius);
    }

    void Update()
    {
        float distanceCheck = Mathf.Abs(Vector3.Distance(transform.position, GenerationManager.instance.populationStartingPosition));
        if (distanceFromStart < distanceCheck)
        {
            distanceFromStart = distanceCheck;
        }
    }

    void FixedUpdate()
    {
        CheckIsGrounded();
        CheckInputTimes();
    }

    void CheckIsGrounded()
    {
        overlapSphereColliderPosition = transform.position - new Vector3(0, 0.1f, 0);
        Collider[] overLapColliders = Physics.OverlapSphere(overlapSphereColliderPosition, overlapSphereRadius, LayerMask.GetMask("Default"));
        
        isGrounded = overLapColliders.Length > 0;
    }

    void CheckInputTimes()
    {
        if (!GenerationManager.instance.timeLimitReached && !GenerationManager.instance.goalReached)
        {
            if (Time.time - timeLimit <= chromosome.genes[chromosomeIndex].pressTime)
            {
                RunInputs();
            }
            else
            {
                if (!lastGeneAdded)
                {
                    chromosome.AddGene(ref lastGeneAdded);
                }

                timeLimit = Time.time;
                if (chromosomeIndex + 1 != chromosome.genes.Count)
                {
                    chromosomeIndex++;
                    hasJumped = false;
                }
            }
        }
    }

    void RunInputs()
    {
        for (int i = 0; i < GenerationManager.instance.possibleInputs.Length; i++)
        {
            if (chromosome.genes[chromosomeIndex].inputPairs[GenerationManager.instance.possibleInputs[i]])
            {
                switch (GenerationManager.instance.possibleInputs[i])
                {
                    case (int)KeyCode.RightArrow:
                        MoveRight();
                        break;
                    case (int)KeyCode.Space:
                        if (isGrounded && !hasJumped)
                        {
                            Jump();
                            hasJumped = true;
                        }
                        break;
                }
            }
        }
    }

    void MoveRight()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            Vector3 tempVect = Vector3.right * speed * Time.deltaTime;
            rb.AddForce(tempVect);
        }
    }


    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            GenerationManager.instance.goalReached = true;
        }
    }
}
