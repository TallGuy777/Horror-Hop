using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    // Danh sách các vị trí điểm đến
    public List<Transform> waypoints = new List<Transform>();

    // Hàm để lấy ra một vị trí điểm đến ngẫu nhiên
    public Vector3 GetRandomWaypoint()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints available.");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, waypoints.Count);
        return waypoints[randomIndex].position;
    }
}
