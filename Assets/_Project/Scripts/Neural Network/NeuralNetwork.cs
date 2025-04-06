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
    }
}
