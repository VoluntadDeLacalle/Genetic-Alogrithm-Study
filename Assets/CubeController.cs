using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Gene
{
    public Dictionary<int, bool> inputPairs;
    public float pressTime;
}

public class CubeController : MonoBehaviour
{
    List<Gene> chromosome;
    int[] possibleInputs;

    public int chromosomeLength = 1;
    private int chromosomeIndex = 0;

    public float timeLimit = 0;
    public float timeBuffer = 2;
    public float speed = 0;
    public float maxSpeed = 0;
    public float jumpForce = 0;

    public float distanceFromStart = 0;

    public bool isGrounded = false;

    Rigidbody rb;

    MeshFilter mf;
    float overlapSphereRadius = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mf = GetComponent<MeshFilter>();

        chromosome = new List<Gene>();
        possibleInputs = new int[] { (int)KeyCode.RightArrow, (int)KeyCode.LeftArrow, (int)KeyCode.Space};

        InitializeChromosome();

        overlapSphereRadius = mf.mesh.bounds.extents.x + 0.1f;

        timeLimit = Time.time;
    }
    
    void InitializeChromosome()
    {
        for (int i = 0; i < chromosomeLength; i++)
        {
            Gene gene = new Gene();
            Dictionary<int, bool> inputPairs;
            if (i == 0)
            {
                inputPairs = new Dictionary<int, bool>();
                foreach (int keyCodes in possibleInputs)
                {
                    inputPairs[keyCodes] = false;
                }
            }
            else
            {
                inputPairs = new Dictionary<int, bool>();
                foreach (int keyCodes in possibleInputs)
                {
                    inputPairs[keyCodes] = chromosome[i - 1].inputPairs[keyCodes];
                }
            }

            

            int input = possibleInputs[TryRandomInput()];
            inputPairs[input] = !inputPairs[input];

            gene.inputPairs = inputPairs;
            gene.pressTime = Random.Range(1, 15);

            chromosome.Add(gene);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }

    void Update()
    {
        distanceFromStart = Vector3.Distance(transform.position, new Vector3(0, 0.5f, 0));
    }

    void FixedUpdate()
    {
        CheckIsGrounded();
        CheckInputTimes();
    }

    void CheckIsGrounded()
    {
        Collider[] overLapColliders = Physics.OverlapSphere(transform.position, overlapSphereRadius);
        
        isGrounded = overLapColliders.Length > 1;
    }

    int TryRandomInput()
    {
        int rand = Random.Range(0, 3);
        return rand;
    }

    void CheckInputTimes()
    {
        if (Time.time - timeLimit <= chromosome[chromosomeIndex].pressTime)
        {
            RunInputs();
        }
        else
        {
            timeLimit = Time.time;

            if (chromosomeIndex + 1 != chromosome.Count)
            {
                chromosomeIndex++;
            }
        }
    }

    void RunInputs()
    {
        for (int i = 0; i < possibleInputs.Length; i++)
        {
            if (chromosome[chromosomeIndex].inputPairs[possibleInputs[i]])
            {
                switch (possibleInputs[i])
                {
                    case (int)KeyCode.RightArrow:
                        MoveRight();
                        //print("Right.");
                        break;
                    case (int)KeyCode.LeftArrow:
                        MoveLeft();
                        //print("Left.");
                        break;
                    case (int)KeyCode.Space:
                        if (isGrounded)
                        {
                            Jump();
                            //print("Jump.");
                        }
                        chromosome[chromosomeIndex].inputPairs[possibleInputs[i]] = !chromosome[chromosomeIndex].inputPairs[possibleInputs[i]];
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

    void MoveLeft()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            Vector3 tempVect = Vector3.left * speed * Time.deltaTime;
            rb.AddForce(tempVect);
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
