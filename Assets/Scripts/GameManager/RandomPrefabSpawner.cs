using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnItem
    {
        public GameObject prefab; // Prefab cần spawn
        public List<Transform> spawnPoints; // Danh sách các vị trí spawn
        public int spawnCount; // Số lượng prefab cần spawn
    }

    public List<SpawnItem> spawnItems; // Danh sách các prefab và vị trí spawn tương ứng

    public bool spawnOnStart = true; // Có spawn ngay khi start game không

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnPrefabs();
        }
    }

    public void SpawnPrefabs()
    {
        foreach (SpawnItem item in spawnItems)
        {
            for (int i = 0; i < item.spawnCount; i++)
            {
                Transform spawnPoint = GetRandomSpawnPoint(item.spawnPoints);
                
                if (spawnPoint != null)
                {
                    Instantiate(item.prefab, spawnPoint.position, spawnPoint.rotation);
                }
            }
        }
    }

    private Transform GetRandomSpawnPoint(List<Transform> points)
    {
        if (points.Count == 0)
        {
            Debug.LogWarning("Không có vị trí spawn nào được thiết lập.");
            return null;
        }

        int randomIndex = Random.Range(0, points.Count);
        return points[randomIndex];
    }

    private void OnDrawGizmosSelected()
    {
        foreach (SpawnItem item in spawnItems)
        {
            foreach (Transform point in item.spawnPoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }
}
