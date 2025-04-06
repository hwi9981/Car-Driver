using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithm.NeuralNetwork
{
    public class Layer : MonoBehaviour
    {
        public Neuron[] Neurons;
        public float[,] Weights;   // [fromNeuron, toNeuron]
        public float[] Biases;

        private int inputCount;
        private int outputCount;

        public Layer(int inputCount, int outputCount)
        {
            this.inputCount = inputCount;
            this.outputCount = outputCount;

            Neurons = new Neuron[outputCount];
            Weights = new float[inputCount, outputCount];
            Biases = new float[outputCount];

            for (int i = 0; i < outputCount; i++)
                Neurons[i] = new Neuron();

            Randomize();
        }

        public void Randomize()
        {
            for (int i = 0; i < inputCount; i++)
            for (int j = 0; j < outputCount; j++)
                Weights[i, j] = UnityEngine.Random.Range(-1f, 1f);

            for (int j = 0; j < outputCount; j++)
                Biases[j] = UnityEngine.Random.Range(-1f, 1f);
        }

        public float[] FeedForward(float[] inputs)
        {
            float[] outputs = new float[outputCount];

            for (int j = 0; j < outputCount; j++)
            {
                float sum = 0f;
                for (int i = 0; i < inputCount; i++)
                    sum += inputs[i] * Weights[i, j];

                sum += Biases[j];
                outputs[j] = Neuron.ActivateFunction(sum);

                Neurons[j].Value = outputs[j];
            }

            return outputs;
        }
    }
}
