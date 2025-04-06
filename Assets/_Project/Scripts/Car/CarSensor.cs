using UnityEngine;

public class CarSensor : MonoBehaviour
{
    [Header("Sensor Settings")]
    public int sensorCount = 5; // Số lượng cảm biến (ví dụ: 5)
    public float sensorLength = 10f; // Độ dài tối đa của cảm biến
    public float angleRange = 90f; // Tổng góc quét (ví dụ: 90°)
    public LayerMask wallLayer; // Layer tường

    [Header("Debug")]
    public bool showGizmos = true;
    public float[] sensorOutputs; // Kết quả các cảm biến (dùng cho input NN)

    void FixedUpdate()
    {
        sensorOutputs = new float[sensorCount];

        float angleOffset = angleRange / (sensorCount - 1);

        for (int i = 0; i < sensorCount; i++)
        {
            // Tính góc quét từ -angleRange/2 đến +angleRange/2
            float angle = -angleRange / 2 + angleOffset * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * transform.up;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, sensorLength, wallLayer);

            if (hit.collider != null)
            {
                sensorOutputs[i] = hit.distance / sensorLength; // Chuẩn hoá khoảng cách từ 0-1
            }
            else
            {
                sensorOutputs[i] = 1f; // Không có tường -> khoảng cách tối đa
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || sensorOutputs == null) return;
        if (!Application.isPlaying) return;
        
        float angleOffset = angleRange / (sensorCount - 1);
        for (int i = 0; i < sensorCount; i++)
        {
            float angle = -angleRange / 2 + angleOffset * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * transform.up;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, dir * sensorLength * sensorOutputs[i]);
        }
    }

    // Gọi từ Neural Network để lấy input
    public float[] GetSensorData()
    {
        return sensorOutputs;
    }
}
