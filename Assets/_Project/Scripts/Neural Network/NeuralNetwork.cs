using System.Collections.Generic;

namespace Algorithm.NeuralNetwork
{
    public class NeuralNetwork
    {
        public List<Layer> Layers = new List<Layer>();
        public NeuralNetwork(int[] layerSizes)
        {
            for (int i = 0; i < layerSizes.Length - 1; i++)
            {
                Layer layer = new Layer(layerSizes[i], layerSizes[i + 1]);
                Layers.Add(layer);
            }
            Randomize();
        }

        public float[] FeedForward(float[] input)
        {
            float[] output = input;
            foreach (var layer in Layers)
                output = layer.FeedForward(output);
            return output;
        }

        public void Randomize()
        {
            foreach (var layer in Layers)
                layer.Randomize();
        }

        public NeuralNetwork Clone()
        {
            // (có thể thêm sau nếu bạn cần)
            return null;
        }
        public float[] EncodeToGene()
        {
            var genes = new List<float>();
            foreach (var layer in Layers)
            {
                int inCount = layer.Weights.GetLength(0);
                int outCount = layer.Weights.GetLength(1);
                for (int i = 0; i < inCount; i++)
                for (int j = 0; j < outCount; j++)
                    genes.Add(layer.Weights[i, j]);
                for (int j = 0; j < outCount; j++)
                    genes.Add(layer.Biases[j]);
            }
            return genes.ToArray();
        }

        public void DecodeFromGene(float[] gene)
        {
            int index = 0;
            foreach (var layer in Layers)
            {
                int inCount = layer.Weights.GetLength(0);
                int outCount = layer.Weights.GetLength(1);
                for (int i = 0; i < inCount; i++)
                for (int j = 0; j < outCount; j++)
                    layer.Weights[i, j] = gene[index++];
                for (int j = 0; j < outCount; j++)
                    layer.Biases[j] = gene[index++];
            }
        }

        public int GetGeneLength()
        {
            int length = 0;
            foreach (var layer in Layers)
            {
                int inCount = layer.Weights.GetLength(0);
                int outCount = layer.Weights.GetLength(1);
                length += inCount * outCount + outCount;
            }
            return length;
        }
    }
}
