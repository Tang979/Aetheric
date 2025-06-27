using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TowerUIManager : MonoBehaviour
{
    [Header("UI Panel Chính")]
    public GameObject panel;
    public TextMeshProUGUI towerName, levelText, skillDesc;
    public Image icon;
    public TextMeshProUGUI damageText, speedText, rangeText, upgradeCostText, sellValueText;
    public Button upgradeButton, sellButton;

    private TowerInstance currentTower;

    public static TowerUIManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ShowPanel(TowerInstance tower)
    {
        currentTower = tower;
        var data = tower.data;

        towerName.text = data.towerName.ToString();
        levelText.text = "Lv." + (tower.Level);
        icon.sprite = data.icon;
        damageText.text = $"{tower.CurrentDamage}";
        rangeText.text = $"{tower.data.attackRange}";
        switch (data.attackType)
        {
            case TowerData.AttackType.Projectile:
                speedText.text = $"{tower.data.projectileConfig.attackSpeed}";
                break;
            case TowerData.AttackType.Spray:
                speedText.text = $"{tower.data.sprayConfig.tickRate}";
                break;
            default:
                speedText.text = "N/A";
                break;
        }
        sellValueText.text = $"Sell {tower.GetSellValue()}";

        // skillDesc.text = GetSkillDescription(data);
        upgradeButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();

        sellButton.onClick.AddListener(OnSellClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        if (tower.CanUpgrade(tower.Level))
        {
            upgradeCostText.text = $"Upgrade {currentTower.GetUpgradeCost()}";
            upgradeButton.interactable = true;
        }
        else if (tower.isMaxLevel())
        {
            upgradeButton.interactable = false;
            upgradeCostText.text = "Max Level";
        }
        else
        {
            upgradeButton.interactable = false;
        }

        // upgradeButton.onClick.AddListener(() => UpdatePanel(tower));

        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
        currentTower = null;
    }

    public void OnUpgradeClicked()
    {
        Debug.Log("Upgrade button clicked");
        currentTower?.Upgrade();
        ShowPanel(currentTower); // Refresh lại sau khi nâng
    }

    public void DisableUpgradeButton()
    {
        upgradeButton.interactable = false;
    }

    public void OnSellClicked()
    {
        currentTower?.Sell(); // nếu có chức năng bán
        HidePanel();
    }

    // string GetSkillDescription(TowerData data)
    // {
    //     return !string.IsNullOrEmpty(data.descriptionSkill)
    //         ? data.descriptionSkill
    //         : "No skill description available.";
    // }
}
