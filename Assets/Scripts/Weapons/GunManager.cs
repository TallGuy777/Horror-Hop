using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] private TMP_Text BulletText;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject Fire;
    [SerializeField] private GameObject HitPoint;
    [SerializeField] private Transform FireEffect;
    [SerializeField] private int Damage;
    [SerializeField] private float CoolDown;
    [SerializeField] private bool CanShoot;

    public AudioClip gunshotSound; // Âm thanh bắn súng
    public static event Action<Vector3> OnGunShot; // Sự kiện phát ra khi nổ súng

    private AudioSource audioSource;
    private MagazineManager magazineManager;

    [Obsolete]
    private void Awake()
    {
        // Tìm và lấy MagazineManager trong scene
        magazineManager = FindObjectOfType<MagazineManager>();
        if (magazineManager == null)
        {
            Debug.LogError("Không tìm thấy MagazineManager trong scene.");
        }

        // Tìm các thành phần bằng tag hoặc tên
        GameObject bulletTextObject = GameObject.FindWithTag("BulletText");
        if (bulletTextObject != null)
        {
            BulletText = bulletTextObject.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("BulletText với tag 'BulletTextTag' không được tìm thấy.");
        }

        GameObject firePointObject = GameObject.FindWithTag("FirePoint");
        if (firePointObject != null)
        {
            FirePoint = firePointObject.transform;
        }
        else
        {
            Debug.LogError("FirePoint với tag 'FirePointTag' không được tìm thấy.");
        }

        UpdateBulletText(); // Cập nhật số lượng đạn ban đầu lên UI
        CanShoot = true;

        audioSource = GetComponent<AudioSource>(); // Lấy AudioSource từ cùng GameObject
    }

    void Update()
    {
        // Nhấn phím E để xóa đối tượng "bullet" khi raycast vào
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(FirePoint.position, transform.TransformDirection(Vector3.forward), out hit, 100))
            {
                if (hit.collider.CompareTag("ShotgunBullet"))
                {
                    magazineManager.AddBullets(1); // Thêm một viên đạn vào khẩu súng
                    UpdateBulletText(); // Cập nhật số lượng đạn sau khi nhặt đạn
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Bullet removed.");
                }
            }
        }

        // Nhấn chuột trái để bắn
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (CanShoot == false || !magazineManager.HasBullets())
                return;
            StartCoroutine(CoolDownTime());
        }
    }

    IEnumerator CoolDownTime()
    {
        Shooting();
        CanShoot = false;
        yield return new WaitForSeconds(CoolDown);
        CanShoot = true;
    }

    void Shooting()
    {
        magazineManager.ConsumeBullet(); // Bắn mất một viên đạn từ khẩu súng
        UpdateBulletText(); // Cập nhật số lượng đạn sau khi bắn

        // Phát âm thanh bắn súng
        if (audioSource != null && gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }

        // Phát sự kiện nổ súng với vị trí FirePoint
        if (OnGunShot != null)
        {
            OnGunShot.Invoke(FirePoint.position);
        }

        RaycastHit hit;
        if (Physics.Raycast(FirePoint.position, transform.TransformDirection(Vector3.forward), out hit, 100))
        {
            Debug.DrawRay(FirePoint.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("We shoot");

            // Tạo hiệu ứng bắn
            GameObject a = Instantiate(Fire, FireEffect.position, Quaternion.identity);

            // Add AudioSource to fire effect clone if not present
            AudioSource cloneAudioSource = a.GetComponent<AudioSource>();
            if (cloneAudioSource == null)
            {
                cloneAudioSource = a.AddComponent<AudioSource>();
            }

            // Assign the gunshot sound to the clone's AudioSource and play it
            cloneAudioSource.clip = gunshotSound;
            cloneAudioSource.Play();

            GameObject b = Instantiate(HitPoint, hit.point, Quaternion.identity);
            Destroy(a, 2f);
            Destroy(b, 1f);

            // Xử lý sát thương cho Enemy nếu trúng mục tiêu là Enemy
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.TakeDamage(Damage);
            }
        }
    }

    public void UpdateBulletText()
    {
        BulletText.text = magazineManager.MagSize + "/" + magazineManager.MaxMagSize.ToString();
    }
}
