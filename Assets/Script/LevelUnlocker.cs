using UnityEngine;
using Doozy.Runtime.UIManager.Components; // Import Doozy UIButton

public class LevelUnlocker : MonoBehaviour
{
    [SerializeField] private UIButton[] buttons;
    private int unlockedLevelsNumber;

    private void Start()
    {
        // Nếu chưa có dữ liệu unlock, thì mặc định mở khóa màn 1
        if (!PlayerPrefs.HasKey("UnlockedLevels"))
        {
            PlayerPrefs.SetInt("UnlockedLevels", 1);
            PlayerPrefs.Save();
        }

        unlockedLevelsNumber = PlayerPrefs.GetInt("UnlockedLevels");

        // Duyệt qua các nút và chỉ bật những nút đã unlock
        for (int i = 0; i < buttons.Length; i++)
        {
            bool isUnlocked = i < unlockedLevelsNumber;
            buttons[i].interactable = isUnlocked;
        }
    }
}
