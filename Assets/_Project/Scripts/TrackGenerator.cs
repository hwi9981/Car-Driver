using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TrackGenerator : MonoBehaviour
{
    public static TrackGenerator Instance;
    [Header("Track Shape")]
    public int numberOfPoints = 12;
    public float radius = 10f;
    [Range(0, 1)] public float randomness = 0.3f;

    [Header("Track Width")]
    public float trackWidth = 1f;
    public EdgeCollider2D leftEdge;
    public EdgeCollider2D rightEdge;

    [Header("Smoothing")]
    public bool smooth = true;
    public int resolutionPerSegment = 10;
    
    [Header("Start/Finish Line")]
    public GameObject startLinePrefab;
    private GameObject startLineInstance;
    
    public List<Vector2> trackPoints = new List<Vector2>();
    
    public GameObject GetStartLine() => startLineInstance;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        GenerateTrack();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void GenerateTrack()
    {
        List<Vector2> centerPoints = GenerateRandomCirclePoints();
        List<Vector2> smoothCenter = smooth
            ? GenerateSmoothCurve(centerPoints, resolutionPerSegment)
            : new List<Vector2>(centerPoints);

        // Đảm bảo khép kín
        if (smoothCenter[0] != smoothCenter[smoothCenter.Count - 1])
            smoothCenter.Add(smoothCenter[0]);

        // Cập nhật LineRenderer
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = smoothCenter.Count;
        lineRenderer.widthMultiplier = trackWidth;
        lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 1);

        for (int i = 0; i < smoothCenter.Count; i++)
        {
            lineRenderer.SetPosition(i, smoothCenter[i]);
        }

        // Tính toán viền
        List<Vector2> leftPoints = new List<Vector2>();
        List<Vector2> rightPoints = new List<Vector2>();

        for (int i = 0; i < smoothCenter.Count; i++)
        {
            Vector2 prev = smoothCenter[(i - 1 + smoothCenter.Count) % smoothCenter.Count];
            Vector2 next = smoothCenter[(i + 1) % smoothCenter.Count];
            Vector2 dir = (next - prev).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x); // pháp tuyến

            Vector2 center = smoothCenter[i];
            leftPoints.Add(center + normal * (trackWidth / 2f));
            rightPoints.Add(center - normal * (trackWidth / 2f));
        }

        // Gán vào EdgeColliders
        leftEdge.points = leftPoints.ToArray();
        rightEdge.points = rightPoints.ToArray();
        
        PlaceStartLine(smoothCenter);
        // Đảm bảo điểm đầu tiên gần với StartLine nhất
        Vector2 startPos = startLineInstance.transform.position;
        Vector2 forward = -startLineInstance.transform.right; // hướng mũi tên của startLine

        smoothCenter = RotateListToStartAtClosest(smoothCenter, startPos, forward);

        trackPoints = new List<Vector2>(smoothCenter);
    }

    List<Vector2> GenerateRandomCirclePoints()
    {
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfPoints;
            float randRadius = radius * (1 - randomness * 0.5f + Random.value * randomness);
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randRadius;
            points.Add(point);
        }

        return points;
    }

    List<Vector2> GenerateSmoothCurve(List<Vector2> points, int resolution)
    {
        List<Vector2> result = new List<Vector2>();
        int count = points.Count;

        for (int i = 0; i < count; i++)
        {
            Vector2 p0 = points[(i - 1 + count) % count];
            Vector2 p1 = points[i];
            Vector2 p2 = points[(i + 1) % count];
            Vector2 p3 = points[(i + 2) % count];

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                Vector2 point = 0.5f * (
                    2f * p1 +
                    (-p0 + p2) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
                );
                result.Add(point);
            }
        }

        return result;
    }
    void PlaceStartLine(List<Vector2> path)
    {
        if (startLinePrefab == null || path == null || path.Count < 2) return;

        // Chọn ngẫu nhiên 1 điểm (tránh 0 để có vector hướng)
        int index = Random.Range(1, path.Count - 1);

        Vector2 pos = path[index];
        Vector2 forward = (path[index + 1] - path[index - 1]).normalized;

        float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

        // Xóa vạch cũ nếu có
        if (startLineInstance != null)
            Destroy(startLineInstance);

        // Tạo vạch mới
        startLineInstance = Instantiate(startLinePrefab, pos, Quaternion.Euler(0, 0, angle), transform);
    }
    
    void OnDrawGizmos()
    {
        if (trackPoints == null || trackPoints.Count == 0)
            return;

        Gizmos.color = Color.yellow;
        float size = 0.2f;

        for (int i = 0; i < trackPoints.Count; i++)
        {
            Gizmos.DrawSphere(trackPoints[i], size);

            // Vẽ line nối giữa các track point để dễ theo dõi
            Vector2 next = trackPoints[(i + 1) % trackPoints.Count];
            Gizmos.DrawLine(trackPoints[i], next);
        }
    }

    List<Vector2> RotateListToStartAtClosest(List<Vector2> points, Vector2 startPos, Vector2 forwardDir)
    {
        int closestIndex = 0;
        float minDist = float.MaxValue;

        for (int i = 0; i < points.Count; i++)
        {
            float dist = Vector2.Distance(points[i], startPos);
            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }

        // Xoay danh sách sao cho closestIndex là đầu tiên
        List<Vector2> rotated = new List<Vector2>();
        for (int i = 0; i < points.Count; i++)
        {
            rotated.Add(points[(i + closestIndex) % points.Count]);
        }

        // Kiểm tra hướng đi có cùng chiều với startLine không
        Vector2 dir = (rotated[1] - rotated[0]).normalized;
        float dot = Vector2.Dot(dir, forwardDir.normalized);

        // Nếu ngược chiều → đảo ngược danh sách
        if (dot < 0)
        {
            rotated.Reverse();
        }

        return rotated;
    }


}
