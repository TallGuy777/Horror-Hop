using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSound : MonoBehaviour
{
    public AudioClip monsterScream;
    public AudioClip DeathScream;
    public AudioClip monsterFootStep1;
    public AudioClip monsterFootStep2;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Phương thức sẽ được gọi từ sự kiện animation
    public void PlayMonsterScream()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(monsterScream);
        }
    }
    public void PlayMonsterDeathScream()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(DeathScream);
        }
    }
    public void PlayMonsteFootStep1()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(monsterFootStep1);
        }
    }
    public void PlayMonsteFootStep2()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(monsterFootStep2);
        }
    }

}
