using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // Mixer âm thanh để điều chỉnh âm lượng
    [SerializeField] private Slider musicSlider; // Thanh trượt cho âm lượng nhạc nền


    public void Start()
    {
        if(PlayerPrefs.HasKey("MusicVolume"))
        {
            LoadVolume(); // Tải âm lượng đã lưu từ PlayerPrefs khi bắt đầu
        }
        else
        {
            SetMusicVolume(); // Cập nhật âm lượng nhạc nền với giá trị mặc định
        }
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value; // Lấy giá trị từ thanh trượt
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20); // Chuyển đổi giá trị thanh trượt sang dB và cập nhật vào AudioMixer
        PlayerPrefs.SetFloat("MusicVolume", volume); // Lưu giá trị âm lượng vào PlayerPrefs
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume"); // Lấy giá trị âm lượng đã lưu từ PlayerPrefs
            SetMusicVolume(); // Cập nhật âm lượng nhạc nền

        }
    }
}
