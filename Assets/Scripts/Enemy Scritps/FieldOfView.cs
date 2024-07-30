using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
// Public variables
public float radius; // Bán kính tầm nhìn
[Range(0, 360)]
public float angle; // Góc tầm nhìn
public GameObject playerRef; // Tham chiếu đến đối tượng người chơi

public LayerMask targetMask;            // LayerMask cho các đối tượng mục tiêu
public LayerMask obstructionMask;       // LayerMask cho các vật cản

// Các biến để theo dõi trạng thái
public bool canSeePlayer;               // Có thể nhìn thấy người chơi
public bool isMoving;                   // Đang di chuyển

// Các biến để lưu vị trí cuối cùng
private Vector3 lastSeenPlayerPosition;         // Vị trí cuối cùng nhìn thấy người chơi
private Vector3 lastSeenNoisePosition;          // Vị trí cuối cùng nghe thấy tiếng ồn
private Vector3 lastPlayerDirection;            // Hướng cuối cùng của người chơi
private NavMeshAgent agent;             // Tham chiếu đến NavMeshAgent của đối tượng

// Biến để điều chỉnh khoảng cách di chuyển bổ sung sau khi mất dấu người chơi
public float extraStepDistance = 2f;    // Khoảng cách bổ sung

private bool isDead = false;            // Biến để theo dõi trạng thái sống/chết của enemy
private EnemyManager enemyManager;      // Tham chiếu đến EnemyManager để xử lý logic enemy

// Biến công khai để điều chỉnh tốc độ di chuyển trong Inspector
public float normalSpeed = 3.5f;        // Tốc độ di chuyển bình thường
public float increasedSpeed = 5.5f;     // Tốc độ di chuyển khi máu dưới 50%

private void Start()
{
    // Tìm và lưu tham chiếu đến đối tượng người chơi
    playerRef = GameObject.FindGameObjectWithTag("Player");

    // Lấy tham chiếu đến NavMeshAgent và EnemyManager của đối tượng
    agent = GetComponent<NavMeshAgent>();
    enemyManager = GetComponent<EnemyManager>();

    // Bắt đầu Coroutine để kiểm tra tầm nhìn
    StartCoroutine(FOVRoutine());

    // Đăng ký sự kiện cho các đối tượng tạo ra tiếng ồn và sự kiện bắn súng
    NoiseObject.OnNoiseObjectCollision += OnNoiseObjectCollisionHandler;
    GunManager.OnGunShot += OnGunShotHandler;

    // Đặt tốc độ di chuyển ban đầu là tốc độ bình thường
    agent.speed = normalSpeed;
}

private void OnDestroy()
{
    // Hủy đăng ký sự kiện khi hủy đối tượng
    NoiseObject.OnNoiseObjectCollision -= OnNoiseObjectCollisionHandler;
    GunManager.OnGunShot -= OnGunShotHandler;
}

// Coroutine để thực hiện kiểm tra tầm nhìn
private IEnumerator FOVRoutine()
{
    WaitForSeconds wait = new WaitForSeconds(0.01f);

    while (true)
    {
        yield return wait;

        // Kiểm tra tầm nhìn nếu enemy chưa chết
        if (!isDead)
        {
            FieldOfViewCheck();
        }
    }
}

// Hàm để kiểm tra tầm nhìn của enemy
private void FieldOfViewCheck()
{
    // Nếu enemy đã chết thì không kiểm tra nữa
    if (isDead) return;

    // Tìm tất cả các đối tượng nằm trong bán kính của enemy
    Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

    if (rangeChecks.Length != 0)
    {
        // Lấy transform của đối tượng gần nhất
        Transform target = rangeChecks[0].transform;
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Kiểm tra góc nhìn có trong khoảng cho phép hay không
        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Kiểm tra xem có vật cản nào giữa enemy và người chơi hay không
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                canSeePlayer = true;
                lastSeenPlayerPosition = target.position;
                lastPlayerDirection = directionToTarget;
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }
    else
    {
        canSeePlayer = false;
    }

    // Xử lý logic di chuyển của enemy
    HandleMovement();
}

// Hàm để xử lý di chuyển của enemy
private void HandleMovement()
{
    // Nếu enemy đã chết thì không xử lý di chuyển nữa
    if (isDead) return;

    // Kiểm tra xem NavMeshAgent có hoạt động không
    if (agent.isActiveAndEnabled)
    {
        // Nếu có thể nhìn thấy người chơi
        if (canSeePlayer)
        {
            // Nếu EnemyManager tồn tại và máu của enemy > 50, di chuyển bình thường
            if (enemyManager != null && enemyManager.Health > 50)
            {
                agent.speed = normalSpeed;
                agent.SetDestination(playerRef.transform.position);
                lastSeenNoisePosition = playerRef.transform.position;
            }
            // Nếu máu của enemy <= 50, di chuyển nhanh hơn và kích hoạt animation Crawl
            else if (enemyManager != null && enemyManager.Health <= 50)
            {
                agent.speed = increasedSpeed;
                agent.SetDestination(playerRef.transform.position);
                if (enemyManager != null)
                {
                    enemyManager.TriggerCrawlAnimation();
                }
            }
        }
        // Nếu không thấy người chơi nhưng có vị trí tiếng động cuối cùng
        else if (lastSeenNoisePosition != Vector3.zero)
        {
            agent.SetDestination(lastSeenNoisePosition);
            lastSeenNoisePosition = Vector3.zero;
        }
        // Nếu không thấy người chơi và cũng không có vị trí tiếng động, đi đến vị trí người chơi cuối cùng
        else if (lastSeenPlayerPosition != Vector3.zero)
        {
            Vector3 overshootPosition = lastSeenPlayerPosition + lastPlayerDirection * extraStepDistance;
            agent.SetDestination(overshootPosition);
            lastSeenPlayerPosition = Vector3.zero;
        }
    }

    // Cập nhật trạng thái di chuyển
    isMoving = agent.velocity.magnitude > 0.1f;
}

// Hàm xử lý khi nhận được sự kiện va chạm với tiếng ồn từ các đối tượng khác
private void OnNoiseObjectCollisionHandler(Vector3 collisionPosition)
{
    // Nếu enemy đã chết thì không xử lý tiếp
    if (isDead) return;

    // Nếu không thấy người chơi, di chuyển đến vị trí tiếng ồn
    if (!canSeePlayer)
    {
        lastSeenNoisePosition = collisionPosition;
        agent.SetDestination(collisionPosition);
    }

    // Cập nhật trạng thái di chuyển
    isMoving = agent.velocity.magnitude > 0.1f;
}

// Hàm xử lý khi nhận được sự kiện bắn súng từ người chơi
private void OnGunShotHandler(Vector3 gunShotPosition)
{
    // Nếu enemy đã chết thì không xử lý tiếp
    if (isDead) return;

    // Nếu không thấy người chơi, di chuyển đến vị trí bắn súng
    if (!canSeePlayer)
    {
        lastSeenNoisePosition = gunShotPosition;
        agent.SetDestination(gunShotPosition);
    }

    // Cập nhật trạng thái di chuyển
    isMoving = agent.velocity.magnitude > 0.1f;
}

// Hàm để đặt trạng thái chết cho enemy
public void SetDead()
{
    isDead = true;
    agent.enabled = false; // Vô hiệu hóa NavMeshAgent để dừng mọi hoạt động di chuyển
}
}