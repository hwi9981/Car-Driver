using Algorithm.NeuralNetwork;
using UnityEngine;

public class CarBrain : MonoBehaviour
{
    public NeuralNetwork neuralNetwork;
    public CarSensor carSensor;
    public CarController carController;

    void Start()
    {
        var inputLayerNodeCount = carSensor.sensorCount + 1;// +1 for current car speed
        var hiddenLayerNodeCount = inputLayerNodeCount;
        var outputLayerNodeCount = 2; // one node for steering and one node for acceleration
        var structure = new int[] { inputLayerNodeCount, hiddenLayerNodeCount, outputLayerNodeCount };
        
        neuralNetwork = new NeuralNetwork(structure);
        
    }
    // dựa vao input để đưa ra quyết định
    void FixedUpdate()
    {
        // B1: Lấy input từ sensor và tốc độ hiện tại
        float[] sensorData = carSensor.GetSensorData();
        float[] inputs = new float[sensorData.Length + 1];

        // Sao chép dữ liệu từ sensorData vào inputs
        for (int i = 0; i < sensorData.Length; i++)
        {
            inputs[i] = sensorData[i];
        }
        
        // Thêm speed vào cuối mảng inputs
        inputs[sensorData.Length] = carController.GetSpeed();
        // B2: Đưa input vào mạng để lấy output
        float[] outputs = neuralNetwork.FeedForward(inputs);

        // B3: Điều khiển xe dựa vào output
        float steering = outputs[0];      // [-1, 1] trái/phải
        float acceleration = outputs[1];  // [-1, 1] lùi/tiến

        // carController.SetInput(steering, acceleration); 
        carController.SetAcceleration(acceleration);
        carController.SetSteering(steering);
        
        // TO DO replace this with GA
        neuralNetwork.Randomize();
        
    }


    #region Genertic Algorithm
    // communicate with GA
    public void EncodeToGene(NeuralNetwork neuralNetwork)
    {
        var layers = neuralNetwork.Layers;
    }

    public void DecodeFromGene()
    {
        
    }
    #endregion

    
    
}
