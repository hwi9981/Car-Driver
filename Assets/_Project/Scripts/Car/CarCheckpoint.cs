using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCheckpoint : MonoBehaviour
{
    // public TrackGenerator trackGenerator; // kéo trackGenerator từ scene vào đây
    public float checkpointRadius = 1.5f; // khoảng cách để tính là đã đến checkpoint
    public bool dragGizmos = true;
    [Header("Timeout")]
    
    public float timeoutDuration = 8f;
    public bool isTimedOut = false;
    
    [Header("Progress Info")]
    public float totalTimeToFinish = 0f;
    public bool finished = false;

    private List<Vector2> checkpoints;
    private int currentCheckpointIndex = 0;
    private int totalCheckpointsPassed = 0;
    
    private float timeSinceLastCheckpoint = 0f;

    void Start()
    {
        checkpoints = TrackGenerator.Instance.trackPoints;
    }

    public void ResetCheckPoint()
    {
        currentCheckpointIndex = 0;
        totalCheckpointsPassed = 0;
        timeSinceLastCheckpoint = 0f;
        isTimedOut = false;
    }


    void Update()
    {
        if (checkpoints == null || checkpoints.Count == 0 || isTimedOut) return;

        timeSinceLastCheckpoint += Time.deltaTime;

        Vector2 carPos = transform.position;
        Vector2 nextCheckpoint = checkpoints[(currentCheckpointIndex + 1) % checkpoints.Count];

        // Nếu xe đủ gần checkpoint tiếp theo
        if (Vector2.Distance(carPos, nextCheckpoint) < checkpointRadius)
        {
            currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Count;
            totalCheckpointsPassed++;
            timeSinceLastCheckpoint = 0f; // Reset thời gian

            Debug.Log($"Passed checkpoint {currentCheckpointIndex}, Total: {totalCheckpointsPassed}");
        }

        // Nếu timeout
        if (timeSinceLastCheckpoint >= timeoutDuration)
        {
            isTimedOut = true;
        }
    }


    public int GetTotalCheckpointsPassed() => totalCheckpointsPassed;
    void OnDrawGizmos()
    {
        if (!dragGizmos) return;
        if (checkpoints == null || checkpoints.Count == 0) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(checkpoints[(currentCheckpointIndex + 1) % checkpoints.Count], 0.3f);
    }
    public float GetProgressPercent()
    {
        return (float)currentCheckpointIndex / checkpoints.Count;
    }
}
