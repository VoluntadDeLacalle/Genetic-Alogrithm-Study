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

    public Chromosome()
    {
        genes = new List<Gene>();
    }

    static public int FindFittestSubject(List<GameObject> newPopulation)
    {
        float maxValue = newPopulation[0].GetComponent<AgentController>().distanceFromStart;
        int maxIndex = 0;
        for (int i = 0; i < newPopulation.Count; i++)
        {
            if (maxValue < newPopulation[i].GetComponent<AgentController>().distanceFromStart)
            {
                maxValue = newPopulation[i].GetComponent<AgentController>().distanceFromStart;
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    static private int GetFitIndex(int numbOfChallengers, List<GameObject> currentPopulation, ref int firstIndex)
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

        float maxValue = currentPopulation[tournament[0]].GetComponent<AgentController>().distanceFromStart;
        int maxIndex = 0;
        for (int i = 0; i < tournament.Count; i++)
        {
            if (maxValue < currentPopulation[tournament[i]].GetComponent<AgentController>().distanceFromStart)
            {
                maxValue = currentPopulation[tournament[i]].GetComponent<AgentController>().distanceFromStart;
                maxIndex = tournament[i];
            }
        }

        return maxIndex;
    }
    static public void Selection(ref int fittestIndex, ref int secondFittestIndex, int tourneySelectNumb, List<GameObject> currentPopulation)
    {
        int firstIndex = -1;
        fittestIndex = GetFitIndex(tourneySelectNumb, currentPopulation, ref firstIndex);
        secondFittestIndex = GetFitIndex(tourneySelectNumb, currentPopulation, ref firstIndex);
    }

    static public Chromosome Crossover(List<GameObject> currentPopulation, List<GameObject> newPopulation)
    {
        Chromosome chromosome = new Chromosome();
        int chromosomeCount1 = currentPopulation[fittestIndex].GetComponent<AgentController>().chromosome.genes.Count;
        int chromosomeCount2 = currentPopulation[secondFittestIndex].GetComponent<AgentController>().chromosome.genes.Count;
        int smallestChromosomeCount = Mathf.Min(chromosomeCount1, chromosomeCount2);

        int rand = Random.Range(0, smallestChromosomeCount);

        for (int i = 0; i < rand; i++)
        {
            chromosome.genes.Add(newPopulation[fittestIndex].GetComponent<AgentController>().chromosome.genes[i].DeepClone());
        }

        for (int i = rand; i < chromosomeCount2 - rand; i++)
        {
            chromosome.genes.Add(newPopulation[secondFittestIndex].GetComponent<AgentController>().chromosome.genes[i].DeepClone());
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

                int input = GenerationManager.instance.possibleInputs[GenerationManager.instance.AddRandomInput()];
                offspringChromosome.genes[randIndex].inputPairs[input] = !offspringChromosome.genes[randIndex].inputPairs[input];

                mutatedIndices.Add(randIndex);
            }
        }

        mutatedIndices.Clear();
        rand = Random.Range(0, 101);
        if (rand < (GenerationManager.instance.mutationChanceRate * 100))
        {
            for (int i = 0; i < mutationCount; i++)
            {
                int randIndex = Random.Range(0, offspringChromosome.genes.Count);
                while (mutatedIndices.Contains(randIndex))
                {
                    randIndex = Random.Range(0, offspringChromosome.genes.Count);
                }

                int input = GenerationManager.instance.possibleInputs[GenerationManager.instance.AddRandomInput()];

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

    static public void AddToPopulation(List<GameObject> newPopulation, int eliteIndex, Chromosome offspringChromosome)
    {
        int rand = Random.Range(0, newPopulation.Count);
        while (rand == eliteIndex)
        {
            rand = Random.Range(0, newPopulation.Count);
        }

        int chromosomeCount1 = newPopulation[rand].GetComponent<AgentController>().chromosome.genes.Count;
        int chromosomeCount2 = offspringChromosome.genes.Count;

        if (chromosomeCount1 > chromosomeCount2 || chromosomeCount1 == chromosomeCount2)
        {
            for (int i = 0; i < chromosomeCount2; i++)
            {
                newPopulation[rand].GetComponent<AgentController>().chromosome.genes[i] = offspringChromosome.genes[i];
            }
        }
        else
        {
            for (int i = 0; i < chromosomeCount1; i++)
            {
                newPopulation[rand].GetComponent<AgentController>().chromosome.genes[i] = offspringChromosome.genes[i];
            }

            for (int i = chromosomeCount1; i < chromosomeCount2; i++)
            {
                newPopulation[rand].GetComponent<AgentController>().chromosome.genes.Add(offspringChromosome.genes[i]);
            }
        }
    }

}
