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
    public TextMeshProUGUI damageText, speedText, rangeText;
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
        levelText.text = "Lv." + (tower.CurrentLevel + 1);
        icon.sprite = data.icon;
        damageText.text = $"{data.baseDamage}";
        rangeText.text = $"{data.attackRange}";
        // skillDesc.text = GetSkillDescription(data);
        upgradeButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();
        
        sellButton.onClick.AddListener(OnSellClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        if (tower.CanUpgrade(tower.CurrentLevel + 1))
        {
            upgradeButton.interactable = true;
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
            currentTower?.UpgradeTower();
            ShowPanel(currentTower); // Refresh lại sau khi nâng
    }

    public void DisableUpgradeButton()
    {
        upgradeButton.interactable = false;
    }

    public void OnSellClicked()
    {
        currentTower?.SellTower(); // nếu có chức năng bán
        HidePanel();
    }

    // string GetSkillDescription(TowerData data)
    // {
    //     return !string.IsNullOrEmpty(data.descriptionSkill)
    //         ? data.descriptionSkill
    //         : "No skill description available.";
    // }
}
