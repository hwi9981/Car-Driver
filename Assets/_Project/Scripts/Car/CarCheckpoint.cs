using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCheckpoint : MonoBehaviour
{
    public float checkpointRadius = 1.5f;
    public bool dragGizmos = true;

    [Header("Timeout Settings")]
    public float timeoutDuration = 8f;
    public bool isTimedOut = false;

    [Header("Lap Info")]
    public float currentCheckpointTime = 0f;    // Thời gian cho checkpoint hiện tại
    public float totalLapTime = 0f;             // Thời gian hoàn thành 1 vòng
    public bool finishedLap = false;

    private List<Vector2> checkpoints;
    private int currentCheckpointIndex = 0;
    private int totalCheckpointsPassed = 0;

    void Start()
    {
        checkpoints = TrackGenerator.Instance.trackPoints;
    }

    public void ResetCheckPoint()
    {
        currentCheckpointIndex = 0;
        totalCheckpointsPassed = 0;
        currentCheckpointTime = 0f;
        totalLapTime = 0f;
        isTimedOut = false;
        finishedLap = false;
    }

    void Update()
    {
        if (checkpoints == null || checkpoints.Count == 0 || isTimedOut || finishedLap) return;

        float deltaTime = Time.deltaTime;

        currentCheckpointTime += deltaTime;
        totalLapTime += deltaTime;

        Vector2 carPos = transform.position;
        Vector2 nextCheckpoint = checkpoints[(currentCheckpointIndex + 1) % checkpoints.Count];

        if (Vector2.Distance(carPos, nextCheckpoint) < checkpointRadius)
        {
            currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Count;
            totalCheckpointsPassed++;

            Debug.Log($"✅ Passed checkpoint {currentCheckpointIndex}, time: {currentCheckpointTime:F2}s");

            currentCheckpointTime = 0f; // Reset cho checkpoint tiếp theo

            // Nếu vừa hoàn thành checkpoint cuối cùng (về lại 0) ⇒ hoàn thành vòng
            if (currentCheckpointIndex == 0)
            {
                finishedLap = true;
                // Debug.Log($"🏁 Lap completed! Total lap time: {totalLapTime:F2}s");
            }
        }

        if (currentCheckpointTime >= timeoutDuration)
        {
            isTimedOut = true;
        }
    }

    // Getter
    public float GetCurrentCheckpointTime() => currentCheckpointTime;
    public float GetTotalLapTime() => totalLapTime;
    public bool IsLapFinished() => finishedLap;
    public bool IsTimedOut() => isTimedOut;
    public int GetTotalCheckpointsPassed() => totalCheckpointsPassed;

    void OnDrawGizmos()
    {
        if (!dragGizmos || checkpoints == null || checkpoints.Count == 0) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(checkpoints[(currentCheckpointIndex + 1) % checkpoints.Count], 0.3f);
    }

    public float GetProgressPercent()
    {
        return (float)currentCheckpointIndex / checkpoints.Count;
    }
}
