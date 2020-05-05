# AI Algorithms - Genetic Algorithm

## What is a Genetic Algorithm?
A genetic algorithm is a search heuristic that was inspired by ideals of Charles Darwin and popularized by the works of John Holland. The algorithm follows a population of virtual automata, which I will refer to as "agents" from now on, through the process of natural selection, in which only the most fit agents of the population will pass their genes on to the next generation. This process continues until an agent from an unknown future generation finds a solution. 

## Implementation and Strategies

    The goal: Traverse an obstacle course in a set amount of time.
    The agents: Spheres capable of moving right and/or jumping.

One of the first things I learned is that there are many implementations of genetic algorithms. That's one of the major appeals of it in fact, its a very flexible algorithm. This is a double-edged sword, however, because with flexibility, comes a lot of trial and error. At every Genetic Algorithm's core are 5 components: **Fitness, Selection, Crossover, Mutation, and Accepting.** I will only be covering the Crossover and Selection functions, because those are where most of my trial and error with the code stemmed from.

A chromosome in genetic algorithm terms is a list of genes. Each gene will contain some information that will allow the agent to act in the current generation.

### Crossover - Single Point, Double Point, and Uniform
The concept of crossover comes from the biology term of the same name. It is when two chromosomes, in our case the two selected chromosomes from the current population, pass their genes on to an Offspring chromosome. This Offspring chromosome will be a combination of the two parents, and how it is combined is described below.

#### DoublePoint
I first tried double point, which chooses two random points to place in both parent's (A & B) chromosomes to be dividers. Usually Parent A will crossover the first section of its genes, then Parent B will crossover for the middle portion, and finally Parent A will crossover the last of its genes for the final portion. If done correctly, the offspring chromosome should be divided into three sections; two thirds Parent A, one third Parent B.

![Double Point Picture](https://github.com/VoluntadDeLacalle/Genetic-Alogrithm-Study/blob/master/Images/DoublePoint.png?raw=true)

Double Point crossover never seemed to produce fast results, if they ever would. A sidebar, I also didn't have an element of Elitism implemented at this point, so the generations would always backtrack, so I originally assumed this was due to crossover. 

#### Uniform
I then moved to a Uniform crossover; this implementation involves randomly choosing which genes to crossover from both parents (A & B) to the offspring chromosome.

![Uniform Picture](https://github.com/VoluntadDeLacalle/Genetic-Alogrithm-Study/blob/master/Images/Uniform.png?raw=true)

As I discovered with this crossover function, my program would often default to waiting on a random mutation to come and help it move along. The biggest problem with this is that it would progress slowly, very slowly. 

#### Single Point
Due to wanting the program to run faster, I finally landed on the Single Point crossover. The premise of this crossover function is the same as Double Point, but rather than choosing two points as dividers, it only chooses one.

![Single Point Picture](https://github.com/VoluntadDeLacalle/Genetic-Alogrithm-Study/blob/master/Images/SinglePoint.png?raw=true)

With this crossover implemented I have had success every time I run the simulation. The best case I have seen so far is the agent finding a solution in three generations while the worst is around 134 generations.

### Selection - Roulette Wheel and Tournament
Selection is the process in which we select the two agents that will foster an offspring into the new population for future generational use. Selection is a tricky thing to balance. As described below, the best parents don't always produce the best results.

#### Roulette Wheel
I decided to initially implement a roulette wheel selection into my program because I had heard of it during a class. To explain this implementation, imagine a pie chart whose values are relative to the fitness percentage of an agent. This pie chart will be our roulette wheel and the ball will be a random integer chosen from zero to the sum of the population's fitnesses.So, if agent had a greater fitness, it would hold a bigger slice in the pie chart, thus making it a more likely choice for our ball to land in.

![Roulette Wheel Picture](https://github.com/VoluntadDeLacalle/Genetic-Alogrithm-Study/blob/master/Images/RouletteWheel.png?raw=true)

This implementation worked fine for the most part, but it has a huge flaw. This implementation has a higher chance of converging faster. This means that the selection function has a higher chance of choosing the greater fitnesses, so the more fit will reproduce more often. After a couple of generations, this will result in all agents containing the exact same chromosome.

#### Tournament
To fix this problem, I landed on an implementation of tournament selection. The idea behind this is that n amount of agents are chosen from a population at random. These agents then have their fitnesses compared and the one with the highest fitness is selected to reproduce.

![Tournament Picture](https://github.com/VoluntadDeLacalle/Genetic-Alogrithm-Study/blob/master/Images/Tournament.png?raw=true)

With this selection function the program was able to run and consistently find a solution with little to no convergence in the process! By adding a little more randomness into the reproduction process, it balanced everything out.

## How is this Algorithm useful?
There is almost limitless potential for this algorithm. As I mentioned above, it has a very flexible implementation, so if you think that a genetic algorithm will help solve a problem, chances are it can! Some of the more general implementations include: automotive design, robotics, machine learning, gaming A.I., encryption and decryption, and business marketing!

## Research and Reference Materials

"A Guided Genetic Algorithm for the Planning in Lunar Lander Games" by Zhangbo Liu.

    https://pdfs.semanticscholar.org/23a1/1f6ed92d7d732ffcc1a65ccaa74d65604385.pdf

"Introduction to Genetic Algorithms" by Marek Obitko.

    https://www.obitko.com/tutorials/genetic-algorithms/index.php

"Introduction to Genetic Algorithms — Including Example Code" by Vijini Mallawaarachchi.

    https://towardsdatascience.com/introduction-to-genetic-algorithms-including-example-code-e396e98d8bf3

"15 Real-World Applications of Genetic Algorithms".

    https://www.brainz.org/15-real-world-applications-genetic-algorithms/
