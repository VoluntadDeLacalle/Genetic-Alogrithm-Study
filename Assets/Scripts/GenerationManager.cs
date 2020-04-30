using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    public static GenerationManager instance = null;

    public float popLifeSpanInMin = 0;
    [HideInInspector]
    public float popLifeSpan = 0;
    public int populationAmount = 0;
    [HideInInspector]
    public List<GameObject> currentPopulation;
    [HideInInspector]
    public List<GameObject> newPopulation;

    public float timeScale = 1;
    [HideInInspector]
    public bool timeLimitReached = false;
    public int currentGeneration = 1;
    public bool goalReached = false;

    [HideInInspector]
    public int fittestIndex = 0;
    [HideInInspector]
    public int secondFittestIndex = 0;
    [HideInInspector]
    public int leastFittestIndex = 0;
    private int eliteIndex = 0;


    public int tourneySelectNumb = 3;
    [Range(0, 1)]
    public float mutationPercentAcrossChromosome = 0;
    [Range(0,1)]
    public float mutationChanceRate = 0;
    public float maxGeneDurationTime = 0;

    [HideInInspector]
    public int[] possibleInputs;
    private List<Gene> offspringChromosome;

    [HideInInspector]
    public Vector3 populationStartingPosition = new Vector3(0, 0.5f, 0);

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

        if (populationAmount < 3)
        {
            populationAmount = 3;
        }

        if (tourneySelectNumb < 3)
        {
            tourneySelectNumb = 3;
        }

        offspringChromosome = new List<Gene>();
        possibleInputs = new int[] { (int)KeyCode.RightArrow, (int)KeyCode.Space };
        Time.timeScale = timeScale;
    }

    void Start()
    {
        popLifeSpan = popLifeSpanInMin * 60;
    }

    void DecreaseTimer()
    {
        popLifeSpan -= Time.deltaTime;

        if (popLifeSpan <= 0 && !timeLimitReached)
        {
            timeLimitReached = true;
            NextGeneration();
            Debug.Log("Time Reached!");
        }
    }

    void NextGeneration()
    {
        newPopulation.Clear();
        for (int i = 0; i < currentPopulation.Count; i++)
        {
            newPopulation.Add(currentPopulation[i]);
        }

        eliteIndex = FindFittestSubject();

        for (int i = 0; i < (currentPopulation.Count / 2); i++)
        {
            Selection();
            Crossover();
            Mutation();
            AddToPopulation();

            fittestIndex = 0;
            secondFittestIndex = 0;
            leastFittestIndex = 0;
            offspringChromosome.Clear();
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

        foreach (GameObject agent in currentPopulation)
        {
            agent.SetActive(false);
            agent.transform.position = populationStartingPosition;
            agent.SetActive(true);
        }

    }

    public int AddRandomInput()
    {
        int rand = Random.Range(0, possibleInputs.Length);
        return rand;
    }

    public int FindFittestSubject()
    {
        float maxValue = newPopulation[0].GetComponent<CubeController>().distanceFromStart;
        int maxIndex = 0;
        for (int i = 0; i < newPopulation.Count; i++)
        {
            if (maxValue < newPopulation[i].GetComponent<CubeController>().distanceFromStart)
            {
                maxValue = newPopulation[i].GetComponent<CubeController>().distanceFromStart;
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    int GetFitIndex(int numbOfChallengers)
    {
        List<int> tournament = new List<int>();

        for (int i = 0; i < numbOfChallengers; i++)
        {
            int randIndex = Random.Range(0, currentPopulation.Count);
            while (tournament.Contains(randIndex))
            {
                randIndex = Random.Range(0, currentPopulation.Count);
            }
            tournament.Add(randIndex);
        }

        float maxValue = currentPopulation[tournament[0]].GetComponent<CubeController>().distanceFromStart;
        int maxIndex = 0;
        for (int i = 0; i < tournament.Count; i++)
        {
            if (maxValue < currentPopulation[tournament[i]].GetComponent<CubeController>().distanceFromStart)
            {
                maxValue = currentPopulation[tournament[i]].GetComponent<CubeController>().distanceFromStart;
                maxIndex = tournament[i];
            }
        }

        return maxIndex;
    }

    void Selection()
    {
        fittestIndex = GetFitIndex(tourneySelectNumb);
        secondFittestIndex = GetFitIndex(tourneySelectNumb);
    }

    void Crossover()
    {
        int chromosomeCount1 = currentPopulation[fittestIndex].GetComponent<CubeController>().chromosome.Count;
        int chromosomeCount2 = currentPopulation[secondFittestIndex].GetComponent<CubeController>().chromosome.Count;
        int smallestChromosomeCount = Mathf.Min(chromosomeCount1, chromosomeCount2);

        int rand = Random.Range(0, smallestChromosomeCount);

        for (int i = 0; i < rand; i++)
        {
            offspringChromosome.Add(newPopulation[fittestIndex].GetComponent<CubeController>().chromosome[i].DeepClone());
        }

        for (int i = rand; i < chromosomeCount2 - rand; i++)
        {
            offspringChromosome.Add(newPopulation[secondFittestIndex].GetComponent<CubeController>().chromosome[i].DeepClone());
        }
    }

    void Mutation()
    {
        int mutationCount = (int)(mutationPercentAcrossChromosome * offspringChromosome.Count);
        int rand = Random.Range(0, 101);

        List<int> mutatedIndices = new List<int>();

        if (rand < (mutationChanceRate * 100))
        {
            Debug.Log("Mutated.");
            for (int i = 0; i < mutationCount; i++)
            {
                int randIndex = Random.Range(0, offspringChromosome.Count);
                while (mutatedIndices.Contains(randIndex))
                {
                    randIndex = Random.Range(0, offspringChromosome.Count);
                }

                int input = possibleInputs[AddRandomInput()];
                offspringChromosome[randIndex].inputPairs[input] = !offspringChromosome[randIndex].inputPairs[input];

                mutatedIndices.Add(randIndex);
            }
        }

        mutatedIndices.Clear();
        rand = Random.Range(0, 101);
        if (rand < (mutationChanceRate * 100))
        {
            for (int i = 0; i < mutationCount; i++)
            {
                int randIndex = Random.Range(0, offspringChromosome.Count);
                while (mutatedIndices.Contains(randIndex))
                {
                    randIndex = Random.Range(0, offspringChromosome.Count);
                }

                int input = possibleInputs[AddRandomInput()];

                offspringChromosome[randIndex].pressTime = Random.Range(1, maxGeneDurationTime);

                mutatedIndices.Add(randIndex);
            }
        }

        mutatedIndices.Clear();
        float pressTimesSum = 0;
        for (int i = 0; i <offspringChromosome.Count; i++)
        {
            pressTimesSum += offspringChromosome[i].pressTime;
        }

        if (pressTimesSum > (popLifeSpanInMin / 60))
        {
            offspringChromosome[offspringChromosome.Count - 1].pressTime = pressTimesSum - (popLifeSpanInMin / 60);
        }
    }

    void AddToPopulation()
    {
        int rand = Random.Range(0, newPopulation.Count);
        while (rand == eliteIndex)
        {
            rand = Random.Range(0, newPopulation.Count);
        }

        int chromosomeCount1 = newPopulation[rand].GetComponent<CubeController>().chromosome.Count;
        int chromosomeCount2 = offspringChromosome.Count;

        if (chromosomeCount1 > chromosomeCount2 || chromosomeCount1 == chromosomeCount2)
        {
            for (int i = 0; i < chromosomeCount2; i++)
            {
                newPopulation[rand].GetComponent<CubeController>().chromosome[i] = offspringChromosome[i];
            }
        }
        else
        {
            for (int i = 0; i < chromosomeCount1; i++)
            {
                newPopulation[rand].GetComponent<CubeController>().chromosome[i] = offspringChromosome[i];
            }

            for (int i = chromosomeCount1; i < chromosomeCount2; i++)
            {
                newPopulation[rand].GetComponent<CubeController>().chromosome.Add(offspringChromosome[i]);
            }
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
            Debug.Log($"Simulation over! Generation: {currentGeneration}");
        }
    }
}
