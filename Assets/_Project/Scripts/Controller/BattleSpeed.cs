using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSpeed : MonoBehaviour
{
    [SerializeField] private Button speedButton;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private int maxSpeedDisplay = 3; // x1, x2, x3, x4
    [SerializeField] private float speedChangeInterval = 0.5f;
    private int currentSpeedIndex = 0;

    void Start()
    {
        UpdateSpeed();
    }

    public void ChangeSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % maxSpeedDisplay;
        UpdateSpeed();
    }

    void UpdateSpeed()
    {
        float actualSpeed = 1f + (currentSpeedIndex * speedChangeInterval);
        Time.timeScale = actualSpeed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // scale theo timeScale

        if (speedButton != null && speedText != null)
        {
            speedText.text = $"x{currentSpeedIndex + 1}";
        }
    }

}