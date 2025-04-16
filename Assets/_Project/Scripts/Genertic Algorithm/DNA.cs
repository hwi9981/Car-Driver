using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Jigsaw_Puzzle.Scripts
{
    public class DNA<T>
    {
        public T[] Genes { get; private set; }
        public float Fitness { get; private set; }
    
        private Func<T> _getRandomGene;
        private Func<int, float> _fitnessFunction; 
        public DNA(int size, Func<T> getRandomGene, Func<int, float> fitnessFunction)
        {
            Genes = new T[size];
            _getRandomGene = getRandomGene;
            _fitnessFunction = fitnessFunction;
        }
    
        public void CreateRandomGenes()
        {
            if (_getRandomGene == null)
            {
                Debug.LogError("getRandomGene is null");
                return;
            }
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = _getRandomGene();
            }
        }
        public void CreateGenes(T[] genes)
        {
            if (Genes.Length != genes.Length)
            {
                Debug.LogError("Genes length is not equal to the length of the given array");
                return;
            }
            Genes = genes;
        }
        public float CalculateFitness(int index)
        {
            Fitness = _fitnessFunction(index);
            return Fitness;
        }
    
        // public DNA<T> Crossover(DNA<T> otherParent)
        // {
        //     DNA<T> child = new DNA<T>(Genes.Length, _getRandomGene, _fitnessFunction);
        //     for (int i = 0; i < Genes.Length; i++)
        //     {
        //         child.Genes[i] = Random.value > 0.5f ? Genes[i] : otherParent.Genes[i];
        //     }
        //     return child;
        // }
        
        //fix crossover
        public DNA<T> Crossover(DNA<T> otherParent)
        {
            DNA<T> child = new DNA<T>(Genes.Length, _getRandomGene, _fitnessFunction);
            HashSet<T> usedGenes = new HashSet<T>();
            bool[] isAssigned = new bool[Genes.Length];

            // Bước 1: Duyệt qua từng vị trí, chọn gene từ cha hoặc mẹ nếu chưa bị trùng
            for (int i = 0; i < Genes.Length; i++)
            {
                T selectedGene = Random.value > 0.5f ? Genes[i] : otherParent.Genes[i];
                if (!usedGenes.Contains(selectedGene))
                {
                    child.Genes[i] = selectedGene;
                    usedGenes.Add(selectedGene);
                    isAssigned[i] = true;
                }
                // nếu bị trùng thì để trống, lát sẽ gán
            }

            // Bước 2: Tìm danh sách gene còn thiếu
            List<T> missingGenes = new List<T>();
            foreach (var gene in Genes) // hoặc otherParent.Genes
            {
                if (!usedGenes.Contains(gene))
                {
                    missingGenes.Add(gene);
                }
            }

            // Bước 3: Fill vào các vị trí trống
            int missingIndex = 0;
            for (int i = 0; i < Genes.Length; i++)
            {
                if (!isAssigned[i])
                {
                    child.Genes[i] = missingGenes[missingIndex];
                    missingIndex++;
                }
            }

            return child;
        }




        // public void Mutate(float mutationRate)
        // {
        //     for (int i = 0; i < Genes.Length; i++)
        //     {
        //         if (Random.value < mutationRate)
        //         {
        //             Genes[i] = _getRandomGene();
        //         }
        //     }
        // }

        // fix mutate
        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (Random.value < mutationRate)
                {
                    // Chọn một vị trí khác ngẫu nhiên để swap
                    int j = Random.Range(0, Genes.Length);
                    // Swap Genes[i] và Genes[j]
                    (Genes[i], Genes[j]) = (Genes[j], Genes[i]);
                }
            }
        }



    }

}
