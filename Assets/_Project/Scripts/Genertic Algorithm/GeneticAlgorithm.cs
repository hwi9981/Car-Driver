using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Algorithm.GeneticAlgorithm
{
    public class GeneticAlgorithm<T>
    {
        public List<DNA<T>> Population { get; private set; }
        public int Generation { get; private set; }
        public float MutationRate { get; set; }
        
        public float BestFitness { get; private set; }
        public T[] BestGenes { get; private set; }
        
        private List<DNA<T>> _newPopulation = new List<DNA<T>>();
        private float _fitnessSum = 0;

        public GeneticAlgorithm(int populationSize, int dnaSize, float mutationRate, Func<T> getRandomGene, Func<int, float> fitnessFunction, Func<T[]> getRandomLongGene = null)
        {
            Generation = 1;
            MutationRate = mutationRate;
            Population = new List<DNA<T>>(populationSize);
            BestGenes = new T[dnaSize];
            for (int i = 0; i < populationSize; i++)
            {
                var dna = new DNA<T>(dnaSize, getRandomGene, fitnessFunction);
                
                if (getRandomLongGene != null)
                {
                    dna.CreateGenes(getRandomLongGene());
                }
                else
                {
                    dna.CreateRandomGenes();
                }
                Population.Add(dna);
            }
        }
        public void CreatePopulation(List<DNA<T>> population)
        {
            Population = new List<DNA<T>>(population);
        }

        public void NewGeneration()
        {
            if (Population.Count <= 0)
            {
                Debug.Log("Population is empty, cannot create new generation");
                return;
            }
            CalculateFitness();

            var sortedPopulation = Population.OrderByDescending(x => x.Fitness).ToList();
            
            _newPopulation.Clear();
            for (int i = 0; i < Population.Count; i++)
            {
                if (i < 5 && Population.Count > 5)
                {
                    _newPopulation.Add(sortedPopulation[i]);
                }
                else
                {
                    // ChooseParent(out var parent1, out var parent2);
                    var parent1 = ChooseParent();
                    var parent2 = ChooseParent();
                    // var child = parent1;
                    var child = parent1.Crossover(parent2);
                    child.Mutate(MutationRate);
                    _newPopulation.Add(child);
                }
            }
            Population = new List<DNA<T>>(_newPopulation);
            Generation++;
        }

        public void CalculateFitness()
        {
            _fitnessSum = 0;
            DNA<T> best = Population[0];
            for (int i = 0; i < Population.Count; i++)
            {
                _fitnessSum += Population[i].CalculateFitness(i);
                if (Population[i].Fitness > best.Fitness)
                {
                    best = Population[i];
                }
            }
            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }
        // private DNA<T> ChooseParent()
        // {
        //     // high fitness has higher chance to be chosen
        //     var randomValue = Random.value * _fitnessSum;
        //     for (int i = 0; i < Population.Count; i++)
        //     {
        //         if (randomValue < Population[i].Fitness)
        //         {
        //             return Population[i];
        //         }
        //         randomValue -= Population[i].Fitness;
        //     }
        //     return null;
        // }
        private DNA<T> ChooseParent()
        {
            var randomValue = Random.value * _fitnessSum;
            for (int i = 0; i < Population.Count; i++)
            {
                if (randomValue < Population[i].Fitness)
                {
                    return Population[i];
                }
                randomValue -= Population[i].Fitness;
            }

            // Nếu vòng lặp không return được ai (do sai số float chẳng hạn), thì chọn cá thể tốt nhất
            DNA<T> best = Population[0];
            for (int i = 1; i < Population.Count; i++)
            {
                if (Population[i].Fitness > best.Fitness)
                {
                    best = Population[i];
                }
            }
            return best;
        }


        void ChooseParent(out DNA<T> parent1, out DNA<T> parent2)
        {
            parent1 = null;
            parent2 = null;
            // Tournament selection
            int selectionCount = 5;
            if (Population.Count > 5)
            {
                // select a random group then choose 2 best from that group
                var tournament = Population.OrderBy(x => Random.value)
                    .Take(selectionCount)
                    .OrderByDescending(x => x.Fitness)
                    .ToList();
                parent1 = tournament[0];
                parent2 = tournament[1];
            }
            else
            {
                parent1 = Population[Random.Range(0, Population.Count)];
                parent2 = Population[Random.Range(0, Population.Count)];
            }
        }
        
    }
}
