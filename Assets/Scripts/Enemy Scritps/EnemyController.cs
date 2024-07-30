using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public GameObject[] movePositions;      // Mảng chứa các vị trí di chuyển
    public float waitTimeMin = 3f;          // Thời gian chờ tối thiểu
    public float waitTimeMax = 5f;          // Thời gian chờ tối đa

    private NavMeshAgent agent;             // Tham chiếu đến NavMeshAgent của enemy
    private bool isMovingRandomly;          // Đang di chuyển ngẫu nhiên hay không

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        // Kiểm tra xem enemy đang di chuyển ngẫu nhiên hay không để cập nhật trạng thái isMovingRandomly
        isMovingRandomly = agent.velocity.magnitude > 0.1f;
    }

    IEnumerator WanderRoutine()
{
    while (true)
    {
        // Check if the agent is active and has a valid destination
        if (agent.enabled && agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            // Agent is actively moving towards a valid destination
            yield return null;
        }
        else
        {
            // Choose a random position to move towards
            GameObject randomPosition = movePositions[Random.Range(0, movePositions.Length)];
            
            // Check if the agent is enabled before setting destination
            if (agent.enabled)
            {
                agent.SetDestination(randomPosition.transform.position);
            }
            
            // Wait for agent to reach destination or until it becomes inactive
            while (agent.enabled && agent.isOnNavMesh && agent.pathPending)
            {
                yield return null;
            }

            // Wait for a random time before choosing the next destination
            yield return new WaitForSeconds(Random.Range(waitTimeMin, waitTimeMax));
        }
    }
}


    // Hàm để bắt đầu di chuyển ngẫu nhiên từ class FieldOfView
    public void StartRandomMovement()
    {
        StartCoroutine(WanderRoutine());
    }

    // Hàm để ngừng di chuyển ngẫu nhiên từ class FieldOfView
    public void StopRandomMovement()
    {
        StopCoroutine(WanderRoutine());
    }
}
