using UnityEngine;

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

    /// <summary>
    /// Initializes certain values.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mf = GetComponent<MeshFilter>();

        chromosome = new Chromosome();
        chromosome.InitializeChromosome();

        overlapSphereRadius = mf.mesh.bounds.extents.x - 0.01f;

        timeLimit = Time.time;
    }

    /// <summary>
    /// Resets global variables once the objects are enabled from their object pooling.
    /// </summary>
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

    /// <summary>
    /// Resets global variables once the objects are disabled.
    /// </summary>
    void OnDisable()
    {
        distanceFromStart = 0;
    }

    /// <summary>
    /// Purely for debugging. Draws a wire sphere gizmo that represents the "Jump Collider." More on this in a later function.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(overlapSphereColliderPosition, overlapSphereRadius);
    }

    /// <summary>
    /// Gets the absolute distance from the population's starting point to where the agent is every frame. This distance
    /// is later used as a scale of fitness for the population.
    /// </summary>
    void Update()
    {
        float distanceCheck = Mathf.Abs(Vector3.Distance(transform.position, GenerationManager.instance.populationStartingPosition));
        if (distanceFromStart < distanceCheck)
        {
            distanceFromStart = distanceCheck;
        }
    }

    /// <summary>
    /// Fixed update for more accurate physics calculations.
    /// </summary>
    void FixedUpdate()
    {
        CheckIsGrounded();
        CheckInputTimes();
    }

    /// <summary>
    /// Uses the "Jump Collider" overlap sphere mentioned above to detect if the bottom of the sphere is colliding with a ground
    /// of some kind. If it is, then the agent is allowed to jump.
    /// </summary>
    void CheckIsGrounded()
    {
        overlapSphereColliderPosition = transform.position - new Vector3(0, 0.1f, 0);
        Collider[] overLapColliders = Physics.OverlapSphere(overlapSphereColliderPosition, overlapSphereRadius, LayerMask.GetMask("Default"));
        
        isGrounded = overLapColliders.Length > 0;
    }

    /// <summary>
    /// Each input has a duration time in the genes of the agent's chromosome. This function runs every input for its allotted
    /// duration time.
    /// </summary>
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

    /// <summary>
    /// This function retrieves the current active inputs from the agent's genes and
    /// calls the respective functions below.
    /// </summary>
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

    /// <summary>
    /// Applies a force to the right of the agent, allowing it to move right.
    /// </summary>
    void MoveRight()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            Vector3 tempVect = Vector3.right * speed * Time.deltaTime;
            rb.AddForce(tempVect);
        }
    }

    /// <summary>
    /// Applies an impulse force to the agent upward, effectively allowing the agent to jump.
    /// </summary>
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// This function is called every time the agent collides with a trigger collider. If the collider belongs to the goal,
    /// the GenerationManager is notified and the simulation ends.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            GenerationManager.instance.goalReached = true;
        }
    }
}
