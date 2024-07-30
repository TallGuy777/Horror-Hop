using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public int Health; // Sức khỏe hiện tại của enemy
    private const int maxHealth = 100; // Sức khỏe tối đa
    private bool isDead = false; // Trạng thái chết của enemy
    private EnemyAnimations enemyAnimations; // Tham chiếu đến script EnemyAnimations để điều khiển animation
    public NavMeshAgent agent; // Tham chiếu đến NavMeshAgent để di chuyển
    public float TimeToScream = 1.5f; // Thời gian chờ cho animation Scream
    private GameObject playerRef; // Tham chiếu đến GameObject của player

    private void Awake()
    {
        enemyAnimations = GetComponent<EnemyAnimations>();
        Health = maxHealth; // Khởi tạo sức khỏe ban đầu
        playerRef = GameObject.FindGameObjectWithTag("Player"); // Tìm player bằng tag
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return; // Nếu enemy đã chết, không thực hiện gì nữa

        Health -= amount; // Giảm sức khỏe của enemy

        if (Health <= 0)
        {
            Death(); // Gọi hàm khi enemy chết
        }
        else if (Health <= 50) // Nếu máu dưới 50%, kích hoạt animation Scream và di chuyển theo player
        {
            if (enemyAnimations != null)
            {
                agent.enabled = false; // Tạm thời vô hiệu hóa NavMeshAgent
                enemyAnimations.ScreamAnimation(); // Kích hoạt animation Scream
                StartCoroutine(WaitForScreamAnimation());
            }
        }
    }

    private IEnumerator WaitForScreamAnimation()
    {
        yield return new WaitForSeconds(TimeToScream); // Chờ cho đến khi kết thúc animation Scream

        if (!isDead)
        {
            agent.enabled = true; // Kích hoạt lại NavMeshAgent
            if (agent.isActiveAndEnabled)
            {
                agent.SetDestination(playerRef.transform.position); // Di chuyển đến vị trí của player
            }
        }
    }

    private void Death()
    {
        isDead = true; // Đặt trạng thái chết

        if (enemyAnimations != null)
        {
            enemyAnimations.DeathAnimation(); // Kích hoạt animation Death
        }

        if (agent != null)
        {
            agent.enabled = false; // Vô hiệu hóa NavMeshAgent khi enemy chết
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false; // Vô hiệu hóa collider để không tương tác vật lý nữa
        }

        // Các hành động khác có thể thực hiện khi enemy chết
    }

    // Getter cho maxHealth để chỉ đọc giá trị từ bên ngoài
    public int MaxHealth
    {
        get { return maxHealth; }
    }

    // Kích hoạt animation Crawl trong EnemyAnimations
    public void TriggerCrawlAnimation()
    {
        if (enemyAnimations != null)
        {
            enemyAnimations.CrawlAnimation();
        }
    }

    // Có thể thêm các hàm khác cần thiết cho enemy, nhưng nhớ kiểm tra biến isDead để tránh thực hiện hành động khi enemy đã chết
    private void Update()
    {
        if (isDead)
            return; // Nếu enemy đã chết, không thực hiện gì nữa

        // Các hành động khác của enemy trong Update
    }
}
