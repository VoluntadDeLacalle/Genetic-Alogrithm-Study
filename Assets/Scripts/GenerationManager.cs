using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance = null;

    [Tooltip("Current Population's Life Span in Minutes")]
    [Range(1, 5)]
    public float popLifeSpanInMin = 0;
    [Tooltip("The current Population size")]
    [Range(10, 200)]
    public int populationAmount = 0;

    [Tooltip("Scales the time of the program\n\n" +
             "Ex. timeScale = 2 -> 2x faster\n\n" +
             "WARNING: A high time scale and big population may lag the renderering of the program")]
    [Range(1, 20)]
    public float timeScale = 1;
    [Tooltip("Determines the amount of contestants allowed to be tried in the Tourney Selection")]
    [Range(3, 6)]
    public int tourneySelectNumb = 3;

    [Tooltip("Determines if an offspring's Chromosome is mutated or not\n(1 = 100%)")]
    [Range(0,1)]
    public float mutationChanceRate = 0;
    [Tooltip("Determines what percentage of the offspring's chromosome is mutated\n(1 = 100%)")]
    [Range(0, 1)]
    public float mutationPercentAcrossChromosome = 0;


    [HideInInspector]
    public Vector3 populationStartingPosition = new Vector3(0, 0.5f, 0);
    [HideInInspector]
    public float popLifeSpan = 0;

    [HideInInspector]
    public List<AgentController> currentPopulation;
    [HideInInspector]
    public List<AgentController> newPopulation;

    [HideInInspector]
    public bool timeLimitReached = false;
    [HideInInspector]
    public bool goalReached = false;
    [HideInInspector]
    public int currentGeneration = 1;

    [HideInInspector]
    public int fittestIndex = 0;
    [HideInInspector]
    public int secondFittestIndex = 0;
    [HideInInspector]
    public int leastFittestIndex = 0;
    [HideInInspector]
    private int eliteIndex = 0;

    [HideInInspector]
    public float maxGeneDurationTime = 0;
    [HideInInspector]
    public int[] possibleInputs;


    private ObjectPooler.Key agentKey = ObjectPooler.Key.Agent;
    private Chromosome offspringChromosome;
    private UIManager uim;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        offspringChromosome = new Chromosome();
        currentPopulation = new List<AgentController>();
        possibleInputs = new int[] { (int)KeyCode.RightArrow, (int)KeyCode.Space };

        uim = GetComponent<UIManager>();

        Time.timeScale = timeScale;
    }

    void Start()
    {
        popLifeSpan = popLifeSpanInMin * 60;

        InitializePopulation();
    }

    void DecreaseTimer()
    {
        popLifeSpan -= Time.deltaTime;

        if (popLifeSpan <= 0 && !timeLimitReached)
        {
            timeLimitReached = true;
            NextGeneration();
            uim.UpdateGeneration();
        }

        uim.UpdateTimer();
    }

    void InitializePopulation()
    {
        for (int i = 0; i < populationAmount; i++)
        {
            GameObject pooledObj = ObjectPooler.GetPooler(agentKey).GetPooledObject();
            pooledObj.transform.position = populationStartingPosition;

            pooledObj.SetActive(true);

            currentPopulation.Add(pooledObj.GetComponent<AgentController>());
        }

        for (int i = 0; i < currentPopulation.Count; i++)
        {
            for (int j = i; j < currentPopulation.Count; j++)
            {
                Physics.IgnoreCollision(currentPopulation[i].gameObject.GetComponent<SphereCollider>(), currentPopulation[j].gameObject.GetComponent<SphereCollider>(), true);
            }
        }
    }

    void NextGeneration()
    {
        newPopulation.Clear();
        for (int i = 0; i < currentPopulation.Count; i++)
        {
            newPopulation.Add(currentPopulation[i]);
        }

        eliteIndex = Chromosome.FindFittestSubject(newPopulation);

        for (int i = 0; i < (currentPopulation.Count / 2); i++)
        {
            Chromosome.Selection(ref fittestIndex, ref secondFittestIndex, tourneySelectNumb, currentPopulation);
            offspringChromosome = Chromosome.Crossover(currentPopulation, newPopulation);
            Chromosome.AddToPopulation(newPopulation, eliteIndex, offspringChromosome);

            fittestIndex = 0;
            secondFittestIndex = 0;
            leastFittestIndex = 0;
        }

        fittestIndex = 0;
        popLifeSpan = popLifeSpanInMin * 60;
        timeLimitReached = false;
        currentGeneration++;

        for (int i = 0; i < currentPopulation.Count; i++)
        {
            currentPopulation[i] = newPopulation[i];
        }
        newPopulation.Clear();

        foreach (AgentController agent in currentPopulation)
        {
            agent.gameObject.SetActive(false);
            agent.gameObject.transform.position = populationStartingPosition;
            agent.gameObject.SetActive(true);
        }

    }

    void Update()
    {
        if (!goalReached)
        {
            DecreaseTimer();
        }
        else
        {
            uim.PostFinalGeneration();
        }
    }
}
