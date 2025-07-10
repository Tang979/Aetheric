using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource MusicSource; // Nguồn âm thanh cho nhạc nền
    [SerializeField] AudioSource SFXSource; // Nguồn âm thanh cho hiệu ứng âm thanh

    public AudioClip Background;

    public void Start()
    {
            MusicSource.clip = Background;
            MusicSource.Play(); // Bắt đầu phát nhạc nền
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip); // Phát hiệu ứng âm thanh
        }
        else
        {
            Debug.LogWarning("SFXSource or clip is null");
        }
    }
}
