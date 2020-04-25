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

        print(chromosome[chromosomeIndex].pressTime);
        print(chromosome[chromosomeIndex].inputPairs[(int)KeyCode.RightArrow]);
        print(chromosome[chromosomeIndex].inputPairs[(int)KeyCode.LeftArrow]);
        print(chromosome[chromosomeIndex].inputPairs[(int)KeyCode.Space]);

        overlapSphereRadius = mf.mesh.bounds.extents.x + 0.1f;

        timeLimit = Time.time;
    }
    
    void InitializeChromosome()
    {
        for (int i = 0; i < chromosomeLength; i++)
        {
            Gene gene = new Gene();
            Dictionary<int, bool> inputPairs = new Dictionary<int, bool>();
            foreach (int keyCodes in possibleInputs)
            {
                inputPairs[keyCodes] = false;
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

            if (chromosomeIndex + 1 != chromosome.Count)
            {
                chromosomeIndex++;
            }
            //return;
        }
        else
        {
            timeLimit = Time.time;
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
                        print("Right.");
                        break;
                    case (int)KeyCode.LeftArrow:
                        MoveLeft();
                        print("Left.");
                        break;
                    case (int)KeyCode.Space:
                        if (isGrounded)
                        {
                            Jump();
                            print("Jump.");
                        }
                        chromosome[chromosomeIndex].inputPairs[possibleInputs[i]] = !chromosome[chromosomeIndex].inputPairs[possibleInputs[i]];
                        break;
                }

                timeLimit = Time.time;
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
