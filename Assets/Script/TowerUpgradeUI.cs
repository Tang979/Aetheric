using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour
{
    public TowerUpgrade upgradeData = new TowerUpgrade();

    [Header("UI References")]
    public TMP_Text levelText;
    public TMP_Text fragmentText;
    public Button upgradeButton;

    private void Start()
    {
        RefreshUI();
        upgradeButton.onClick.AddListener(OnUpgradeClick);
    }

    private void Update()
    {
        // Tự động cập nhật màu nút
        upgradeButton.image.color = upgradeData.CanUpgrade() ? Color.green : Color.gray;
    }

    void OnUpgradeClick()
    {
        if (!upgradeData.CanUpgrade()) return;

        upgradeData.Upgrade();
        RefreshUI();
    }

    void RefreshUI()
    {
        levelText.text = $"Level {upgradeData.level}";
        fragmentText.text = $"{upgradeData.fragments}/{upgradeData.requiredFragments}";
    }

    // Có thể gọi từ nơi khác để cộng mảnh
    public void AddFragments(int amount)
    {
        upgradeData.fragments += amount;
        RefreshUI();
    }
    // Dùng cho mục đích debug, có thể gọi từ Unity Editor
    public void DebugAddOneFragment()
    {
        AddFragments(1);
    }

}
