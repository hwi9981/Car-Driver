using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DropdownFileLoader : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public GameController gameController; // Gắn tham chiếu từ Inspector

    void Start()
    {
        RefreshDropdown();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void RefreshDropdown()
    {
        var files = GeneticSaveLoadSystem.GetSavedFiles();
        dropdown.ClearOptions();
        dropdown.AddOptions(files);
    }

    void OnDropdownValueChanged(int index)
    {
        string fileName = dropdown.options[index].text;
        var data = GeneticSaveLoadSystem.LoadGenerationData(fileName);
        if (data != null)
        {
            gameController.LoadGenesToPopulation(data);
        }
    }
}