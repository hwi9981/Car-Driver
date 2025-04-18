using System;
using System.Collections;
using System.Collections.Generic;
using Algorithm.GeneticAlgorithm;
using TMPro;
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
    
    [Header("For Debug")]
    public TMP_Text generationText;
    public TMP_Text mutationText;
    public TMP_Text bestCarCheckpointText;
    public TMP_Text bestCarTimeText;
    
    
    private GeneticAlgorithm<float> _ga;

    private List<CarBrain> _carPopulation = new List<CarBrain>();
    
    private int _bestCheckPoint = 0;
    private float _bestTime = 0f;
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
        
        foreach (var car in _carPopulation)
        {
            if (_bestCheckPoint < car.carCheckpoint.GetTotalCheckpointsPassed())
            {
                _bestCheckPoint = car.carCheckpoint.GetTotalCheckpointsPassed();
            }

            if (car.carCheckpoint.IsLapFinished())
            {
                if (_bestTime > car.carCheckpoint.GetTotalLapTime() || _bestTime == 0)
                {
                    _bestTime = car.carCheckpoint.GetTotalLapTime();
                }
            }
        }
        UpdateProgressAlgorithm();
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

    public void ForceNextRun()
    {
        _ga.NewGeneration();
        UpdateIndiviualNN();
        NewRun();
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
    // private float EvaluateFitness(int index)
    // {
    //     return _carPopulation[index].carCheckpoint.GetTotalCheckpointsPassed();
    // }
    private float EvaluateFitness(int index)
    {
        var car = _carPopulation[index].carCheckpoint;

        int passed = car.GetTotalCheckpointsPassed();
        float time = car.GetTotalLapTime(); // hoặc thời gian đã chạy nếu chưa hoàn thành

        if (car.IsTimedOut())
            return 0.01f;

        float fitness = passed / (time + 0.001f);

        if (car.IsLapFinished())
        {
            fitness += 20f; // bonus cho lap hoàn thành
        }

        return fitness;
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

    #region Debug Algorithm

    void UpdateProgressAlgorithm()
    {
        generationText.text = _ga.Generation.ToString();
        mutationText.text = mutationRate.ToString();
        bestCarCheckpointText.text = _bestCheckPoint.ToString();
        bestCarTimeText.text = _bestTime.ToString("F2");
    }

    #endregion

    #region Save and Load

    public void SaveCurrent()
    {
        // var filename = "data_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".json";
        // SaveGenesToJson(filename);
        
        // Lấy số lượng lần lưu trữ đã thực hiện từ PlayerPrefs
        int saveCount = PlayerPrefs.GetInt("SaveCount", 0) + 1;

        // Tạo tên file mới
        var filename = saveCount + ".json";

        // Lưu trữ tên file mới vào PlayerPrefs
        PlayerPrefs.SetInt("SaveCount", saveCount);
        PlayerPrefs.Save();

        // Gọi phương thức lưu trữ gen
        SaveGenesToJson(filename);
    }

    public void SaveGenesToJson(string fileName = "saved_genes.json")
    {
        var data = new GenerationData();
        foreach (var t in _carPopulation)
        {
            data.population.Add(new IndividualData() { genes = t.EncodeToGene() });
        }
        GeneticSaveLoadSystem.SaveGenerationData(fileName, data);
    }
    public void LoadGenesToPopulation(GenerationData data)
    {
        for (int i = 0; i < Mathf.Min(_carPopulation.Count, data.population.Count); i++)
        {
            // _carPopulation[i].DecodeFromGene(data.population[i].genes);
            // newData.Add(data.population[i].genes);
            _ga.Population[i].Genes = data.population[i].genes;
        }
        ForceNextRun();
    }

    

    #endregion
}
