using System.Collections.Generic;

[System.Serializable]
public class IndividualData
{
    public float[] genes;
}
[System.Serializable]
public class GenerationData
{
    public List<IndividualData> population = new List<IndividualData>();

    public GenerationData()
    {
        population = new List<IndividualData>();
    }
}

