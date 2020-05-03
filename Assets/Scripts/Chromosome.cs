using System.Collections.Generic;
using UnityEngine;

public class Gene
{

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

public class Chromosome
{
    public List<Gene> genes;
    static int fittestIndex = 0;
    static int secondFittestIndex = 0;
    static private int[] possibleInputs;

    public Chromosome()
    {
        genes = new List<Gene>();
        possibleInputs = GenerationManager.instance.possibleInputs;
    }

    static private int AddRandomInput()
    {
        int rand = Random.Range(0, possibleInputs.Length);
        return rand;
    }

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
    static public void Selection(ref int fittestIndex, ref int secondFittestIndex, int tourneySelectNumb, List<AgentController> currentPopulation)
    {
        int firstIndex = -1;
        fittestIndex = GetFitIndex(tourneySelectNumb, currentPopulation, ref firstIndex);
        secondFittestIndex = GetFitIndex(tourneySelectNumb, currentPopulation, ref firstIndex);
    }

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
