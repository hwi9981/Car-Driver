using System;
using System.Collections;
using System.Collections.Generic;
using Algorithm.GeneticAlgorithm;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public float speed = 1f;
    [Header("Settings:")]
    public int populationSize = 100;
    public float mutationRate = 0.05f;

    [Header("References:")] 
    public CarBrain carPrefab;
    public TrackGenerator trackGenerator;
    
    private GeneticAlgorithm<float> _ga;

    private List<CarBrain> _carPopulation = new List<CarBrain>();
    private void Start()
    {
        var startLine = trackGenerator.GetStartLine();
        carPrefab.gameObject.SetActive(false);
        _carPopulation.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            var car = Instantiate(carPrefab, startLine.transform.position, Quaternion.Euler(0, 0, startLine.transform.rotation.eulerAngles.z + 90), transform);
            car.gameObject.SetActive(true);
            car.InitNN();
            car.RandomizeNN();
            car.Run();
            car.OnHitWall += CheckAllCarActive;
            _carPopulation.Add(car);
        }

        _ga = new GeneticAlgorithm<float>(populationSize, _carPopulation[0].CaculateDNASize(), mutationRate, GetRandomWeight, EvaluateFitness);
        // _ga.NewGeneration();
    }

    void Update()
    {
        Time.timeScale = speed;
    }

    private void CheckAllCarActive()
    {
        if (!HasCarActive())
        {
            // save best car
            // restart game
            _ga.NewGeneration();
            UpdateIndiviualNN();
            NewRun();
        }
    }

    private bool HasCarActive()
    {
        for (int i = 0; i < _carPopulation.Count; i++)
        {
            if (_carPopulation[i].IsActive() && _carPopulation[i].carController.GetSpeed() > 0.1f)
                return true;
        }
        return false;
    }

    // for mutation
    float GetRandomWeight() => Random.Range(-1f, 1f);
    // Caculate fitness
    private float EvaluateFitness(int index)
    {
        // float score = 0;
        // var car = _carPopulation[index];
        // return Random.Range(0, 100);
        // return score;

        return _carPopulation[index].carCheckpoint.GetTotalCheckpointsPassed();
    }

    void UpdateIndiviualNN()
    {
        for (int i = 0; i < _carPopulation.Count; i++)
        {
            _carPopulation[i].DecodeFromGene(_ga.Population[i].Genes);
        }
    }
    void NewRun()
    {
        for (int i = 0; i < _carPopulation.Count; i++)
        {
            var starLine = trackGenerator.GetStartLine();
            _carPopulation[i].transform.position = starLine.transform.position;
            _carPopulation[i].transform.rotation = Quaternion.Euler(0, 0, starLine.transform.rotation.eulerAngles.z + 90);
            _carPopulation[i].Run();
        }
    }
}
