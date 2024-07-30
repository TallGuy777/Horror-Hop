using System;
using UnityEngine;

public class NoiseObject : MonoBehaviour
{
    // Các biến công khai
    public Vector3 collisionPosition;  // Vị trí va chạm
    public AudioClip dropMugSound;     // Âm thanh khi vật rơi
    public static event Action<Vector3> OnNoiseObjectCollision;  // Sự kiện khi đối tượng gây tiếng ồn va chạm

    // Xử lý khi đối tượng va chạm với một vật khác
    void OnCollisionEnter(Collision collision)
    {
        // Lấy thông tin điểm tiếp xúc đầu tiên
        ContactPoint contact = collision.contacts[0];
        collisionPosition = contact.point;

        // Hiển thị vị trí va chạm trong console
        Debug.Log("Object has collided at position: " + collisionPosition);

        // Kiểm tra xem va chạm có xảy ra với Player hoặc Ground không
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            // Kích hoạt sự kiện để thông báo cho các thành phần khác về vị trí va chạm
            OnNoiseObjectCollision?.Invoke(collisionPosition);

            // Phát âm thanh tại vị trí va chạm
            AudioManager.instance.PlaySound(dropMugSound, collisionPosition);
        }
    }
}
