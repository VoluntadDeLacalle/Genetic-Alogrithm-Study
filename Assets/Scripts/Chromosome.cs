using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Every chromosome contains a list of genes. Genes are the actions that each agent takes in order to try and 
/// find a solution. Each gene contains a list of inputs and a duration time for said inputs.
/// </summary>
public class Gene
{

    /// <summary>
    /// Creates and returns a deep copy of an instance of a gene class.
    /// </summary>
    /// <returns></returns>
    public Gene DeepClone()
    {
        var clonedObj = new Gene()
        {
            inputPairs = new Dictionary<int, bool>(this.inputPairs),
            pressTime = this.pressTime,
        };

        return clonedObj;
    }

    public Dictionary<int, bool> inputPairs;
    public float pressTime;
}

/// <summary>
/// Every agent has a chromosome. This is the backbone of a genetic algorithm. This class holds chromosome information
/// as well as functions that allow genes to be modified across generations.
/// </summary>
public class Chromosome
{
    public List<Gene> genes;
    static int fittestIndex = 0;
    static int secondFittestIndex = 0;
    static private int[] possibleInputs;

    /// <summary>
    /// Initializes values once an instance of the chromosome class is created.
    /// </summary>
    public Chromosome()
    {
        genes = new List<Gene>();
        possibleInputs = GenerationManager.instance.possibleInputs;
    }

    /// <summary>
    /// A function that returns an int from 0 to the amount of inputs in the GenerationManager's possibleInputs array.
    /// </summary>
    static private int AddRandomInput()
    {
        int rand = Random.Range(0, possibleInputs.Length);
        return rand;
    }

    /// <summary>
    /// This initializes the chromosome of an agent by adding the initial gene to the list of genes
    /// in the chromosome object. This will only happen for the initial population.
    /// </summary>
    public void InitializeChromosome()
    {
        Gene gene = new Gene();
        Dictionary<int, bool> inputPairs;
        inputPairs = new Dictionary<int, bool>();
        foreach (int keyCodes in possibleInputs)
        {
            inputPairs[keyCodes] = false;
        }

        int input = possibleInputs[AddRandomInput()];
        inputPairs[input] = !inputPairs[input];

        gene.inputPairs = inputPairs;
        gene.pressTime = Random.Range(1, GenerationManager.instance.maxGeneDurationTime);

        genes.Add(gene);
    }

    /// <summary>
    /// Adds a new gene to the list of genes in an agent's chromosome object. This function will only
    /// be run for the initial population, but it will continue to run until the duration of all the agent's
    /// genes in its chromosome add up to the population's life span in seconds.
    /// </summary>
    /// <param name="lastGeneAdded">Returns a bool by reference so the agent knows to stop adding genes.</param>
    public void AddGene(ref bool lastGeneAdded)
    {
        Gene gene = new Gene();
        Dictionary<int, bool> inputPairs;

        inputPairs = new Dictionary<int, bool>();
        foreach (int keyCodes in possibleInputs)
        {
            inputPairs[keyCodes] = genes[genes.Count - 1].inputPairs[keyCodes];

            if (keyCodes == (int)KeyCode.Space && inputPairs[keyCodes])
            {
                inputPairs[keyCodes] = false;
            }

        }

        int input = possibleInputs[AddRandomInput()];
        inputPairs[input] = !inputPairs[input];

        gene.inputPairs = inputPairs;
        gene.pressTime = Random.Range(1, GenerationManager.instance.maxGeneDurationTime);

        if (GenerationManager.instance.popLifeSpan - GenerationManager.instance.maxGeneDurationTime < 0)
        {
            GenerationManager.instance.maxGeneDurationTime = GenerationManager.instance.popLifeSpan;
            lastGeneAdded = true;
            Debug.Log("Last Gene added.");
        }

        genes.Add(gene);
    }

    /// <summary>
    /// Returns an int that represents the fittest agent in the current population. This function is used
    /// for Elitism.
    /// </summary>
    /// <param name="newPopulation">A copy of the current population of genes that changes as offspring are added.</param>
    static public int FindFittestSubject(List<AgentController> newPopulation)
    {
        float maxValue = newPopulation[0].distanceFromStart;
        int maxIndex = 0;
        for (int i = 0; i < newPopulation.Count; i++)
        {
            if (maxValue < newPopulation[i].distanceFromStart)
            {
                maxValue = newPopulation[i].distanceFromStart;
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    /// <summary>
    /// Returns an int that represents the index of the first and second chromosome chosen to "breed" in the
    /// Crossover function. The function itself is an implementation of a Tournament Selection.
    /// </summary>
    /// <param name="numbOfChallengers">The size of the pool of agents to be randomly chosen to compete in the Tournament Selection.</param>
    /// <param name="currentPopulation">The current population of agents.</param>
    /// <param name="firstIndex">An index, part of the Selection function. Used to prevent the same indices from crossing-over.</param>
    static private int GetFitIndex(int numbOfChallengers, List<AgentController> currentPopulation, ref int firstIndex)
    {
        List<int> tournament = new List<int>();

        for (int i = 0; i < numbOfChallengers; i++)
        {
            int randIndex = Random.Range(0, currentPopulation.Count);
            while (tournament.Contains(randIndex) || randIndex == firstIndex)
            {
                randIndex = Random.Range(0, currentPopulation.Count);
            }
            tournament.Add(randIndex);
            if (firstIndex == -1)
            {
                firstIndex = randIndex;
            }
        }

        float maxValue = currentPopulation[tournament[0]].distanceFromStart;
        int maxIndex = 0;
        for (int i = 0; i < tournament.Count; i++)
        {
            if (maxValue < currentPopulation[tournament[i]].distanceFromStart)
            {
                maxValue = currentPopulation[tournament[i]].distanceFromStart;
                maxIndex = tournament[i];
            }
        }

        return maxIndex;
    }

    /// <summary>
    /// Returns two indices, by reference, to be used in the Crossover function. Both indices are to be related to the current population.
    /// </summary>
    /// <param name="fittestIndex">The index chosen as the first parent chromosome.</param>
    /// <param name="secondFittestIndex">The index chosen as the second parent chromosome.</param>
    /// <param name="tourneySelectNumb">The size of the pool of agents to be randomly chosen to compete in the Tournament Selection.</param>
    /// <param name="currentPopulation">The current population of agents.</param>
    static public void Selection(ref int fittestIndex, ref int secondFittestIndex, int tourneySelectNumb, List<AgentController> currentPopulation)
    {
        int firstIndex = -1;
        fittestIndex = GetFitIndex(tourneySelectNumb, currentPopulation, ref firstIndex);
        secondFittestIndex = GetFitIndex(tourneySelectNumb, currentPopulation, ref firstIndex);
    }

    /// <summary>
    /// Takes the two parent chromosomes chosen by the Selection function from the current population and produces
    /// an offspring chromosome with the genes of both parents mixed together. This is an implementation of a
    /// single point crossover. The offspring chromosome is then returned when the function completes.
    /// </summary>
    /// <param name="currentPopulation">The current population of agents.</param>
    /// <param name="newPopulation">A copy of the current population of genes that changes as offspring are added.</param>
    static public Chromosome Crossover(List<AgentController> currentPopulation, List<AgentController> newPopulation)
    {
        Chromosome chromosome = new Chromosome();
        int chromosomeCount1 = currentPopulation[fittestIndex].chromosome.genes.Count;
        int chromosomeCount2 = currentPopulation[secondFittestIndex].chromosome.genes.Count;
        int smallestChromosomeCount = Mathf.Min(chromosomeCount1, chromosomeCount2);

        int rand = Random.Range(0, smallestChromosomeCount);

        for (int i = 0; i < rand; i++)
        {
            chromosome.genes.Add(newPopulation[fittestIndex].chromosome.genes[i].DeepClone());
        }

        for (int i = rand; i < chromosomeCount2 - rand; i++)
        {
            chromosome.genes.Add(newPopulation[secondFittestIndex].chromosome.genes[i].DeepClone());
        }

        Mutation(ref chromosome);

        return chromosome;
    }


    /// <summary>
    /// Mutates a chromosome's genes dependent on the GenerationManager's mutation percent and rate. It returns
    /// the mutated chromosome by reference. The implementation of the mutation focuses on randomly generating
    /// new inputs and durations of the parameter chromosome's genes. This function is called within the Crossover function.
    /// </summary>
    /// <param name="offspringChromosome">The offspring chromosome produced in the Crossover function.</param>
    static private void Mutation(ref Chromosome offspringChromosome)
    {
        int mutationCount = (int)(GenerationManager.instance.mutationPercentAcrossChromosome * offspringChromosome.genes.Count);
        int rand = Random.Range(0, 101);

        List<int> mutatedIndices = new List<int>();

        if (rand < (GenerationManager.instance.mutationChanceRate * 100))
        {
            Debug.Log("Mutated.");
            for (int i = 0; i < mutationCount; i++)
            {
                int randIndex = Random.Range(0, offspringChromosome.genes.Count);
                while (mutatedIndices.Contains(randIndex))
                {
                    randIndex = Random.Range(0, offspringChromosome.genes.Count);
                }

                int input = possibleInputs[AddRandomInput()];
                offspringChromosome.genes[randIndex].inputPairs[input] = !offspringChromosome.genes[randIndex].inputPairs[input];

                mutatedIndices.Add(randIndex);
            }
        }

        mutatedIndices.Clear();
        rand = Random.Range(0, 101);
        if (rand < (GenerationManager.instance.mutationChanceRate * 100))
        {
            Debug.Log("Mutated.");
            for (int i = 0; i < mutationCount; i++)
            {
                int randIndex = Random.Range(0, offspringChromosome.genes.Count);
                while (mutatedIndices.Contains(randIndex))
                {
                    randIndex = Random.Range(0, offspringChromosome.genes.Count);
                }

                offspringChromosome.genes[randIndex].pressTime = Random.Range(1, GenerationManager.instance.maxGeneDurationTime);

                mutatedIndices.Add(randIndex);
            }
        }

        mutatedIndices.Clear();
        float pressTimesSum = 0;
        for (int i = 0; i < offspringChromosome.genes.Count; i++)
        {
            pressTimesSum += offspringChromosome.genes[i].pressTime;
        }

        if (pressTimesSum > (GenerationManager.instance.popLifeSpanInMin / 60))
        {
            offspringChromosome.genes[offspringChromosome.genes.Count - 1].pressTime = pressTimesSum - (GenerationManager.instance.popLifeSpanInMin / 60);
        }
    }


    /// <summary>
    /// Adds the offspring chromosome to the new population list. Since the new population originally
    /// is a copy of the current population, the function takes an index for an elite subject, so that they 
    /// are not replaced by the offspring chromosome in the new population.
    /// </summary>
    /// <param name="newPopulation">A copy of the current population of genes that changes as offspring are added.</param>
    /// <param name="eliteIndex">An index that points to the most fit, or elite, agent in the current population.</param>
    /// <param name="offspringChromosome">The offspring chromosome from the Crossover function that will be added to the new population.</param>
    static public void AddToPopulation(List<AgentController> newPopulation, int eliteIndex, Chromosome offspringChromosome)
    {
        int rand = Random.Range(0, newPopulation.Count);
        while (rand == eliteIndex)
        {
            rand = Random.Range(0, newPopulation.Count);
        }

        int chromosomeCount1 = newPopulation[rand].chromosome.genes.Count;
        int chromosomeCount2 = offspringChromosome.genes.Count;

        if (chromosomeCount1 > chromosomeCount2 || chromosomeCount1 == chromosomeCount2)
        {
            for (int i = 0; i < chromosomeCount2; i++)
            {
                newPopulation[rand].chromosome.genes[i] = offspringChromosome.genes[i];
            }
        }
        else
        {
            for (int i = 0; i < chromosomeCount1; i++)
            {
                newPopulation[rand].chromosome.genes[i] = offspringChromosome.genes[i];
            }

            for (int i = chromosomeCount1; i < chromosomeCount2; i++)
            {
                newPopulation[rand].chromosome.genes.Add(offspringChromosome.genes[i]);
            }
        }
    }

}
