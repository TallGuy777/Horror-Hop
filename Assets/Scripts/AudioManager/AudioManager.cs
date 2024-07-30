using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        audioSource.transform.position = position;
        audioSource.PlayOneShot(clip);
    }

    // Hàm để phát âm thanh của shotgun khi bắn
    public void PlayShotgunSound(Vector3 position)
    {
        // Thay thế "shotgunSound" bằng biến tham chiếu đến âm thanh của shotgun trong Inspector
        AudioClip shotgunSound = Resources.Load<AudioClip>("ShotgunSound"); // Đường dẫn âm thanh shotgun trong thư mục Resources
        if (shotgunSound != null)
        {
            PlaySound(shotgunSound, position);
        }
        else
        {
            Debug.LogWarning("Shotgun sound is not assigned or could not be loaded.");
        }
    }
}
