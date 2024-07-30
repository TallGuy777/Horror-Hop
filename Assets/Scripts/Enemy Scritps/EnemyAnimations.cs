using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    private Animator animator; // Tham chiếu đến Animator của enemy
    private FieldOfView fov; // Tham chiếu đến script FieldOfView để lấy trạng thái di chuyển của enemy

    private void Start()
    {
        animator = GetComponent<Animator>(); // Lấy Animator từ GameObject của enemy
        fov = GetComponent<FieldOfView>(); // Lấy script FieldOfView từ GameObject của enemy
    }

    private void Update()
    {
        if (fov != null)
        {
            animator.SetBool("isMoving", fov.isMoving); // Đặt trạng thái di chuyển của enemy vào Animator
        }
    }

    public void DeathAnimation()
    {
        animator.SetTrigger("Death"); // Kích hoạt animation Death
    }

    public void ScreamAnimation()
    {
        animator.SetTrigger("Scream"); // Kích hoạt animation Scream
    }

    public void CrawlAnimation()
    {
        animator.SetTrigger("Crawl"); // Kích hoạt animation Crawl
    }
}
