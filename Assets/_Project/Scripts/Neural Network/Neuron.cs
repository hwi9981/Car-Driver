using UnityEngine;

namespace Algorithm.NeuralNetwork
{
    public class Neuron
    {
        public float Value { get; set; }

        public Neuron()
        {
            Value = 0f;
        }

        // tương tự hàm sigmoid nhưng thay vì trong khoảng [0,1] thì trong khoảng [-1, 1]
        // tanh(x) = (e^x - e^(-x)) / (e^x + e^(-x))
        public static float ActivateFunction(float x) =>(float) System.Math.Tanh(x);
    }
}
