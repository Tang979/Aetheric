using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("Card UI Elements")]
    public UIButton cardButton, upgradeButton, unlockButton;
    public UISlider sliderUpgrade;
    public TextMeshProUGUI textUpgrade;

    [Header("Card Data")]
    TowerData towerData;
    public static Action<TowerCard> OnTowerUpgraded;

    public void SetupCard(TowerData towerData)
    {
        this.towerData = towerData;

        TowerCard card = GameManager.Instance.PlayerData.ownedTowerCards.Find(c => c.towerName == towerData.towerName);
        if (card == null)
        {
            Debug.LogWarning($"Không tìm thấy TowerCard với tên {towerData.towerName}");
        }
        var ownedCards = card.ownedCards;
        sliderUpgrade.maxValue = card.GetRequiredToUpgrade();
        sliderUpgrade.value = ownedCards;

        upgradeButton.onClickEvent.RemoveAllListeners();
        upgradeButton.onClickEvent.AddListener(() =>
        {
            if (UIManager.Instance.TryUpgradeTower(towerData, out card))
            {
                card.ownedCards -= (int)sliderUpgrade.maxValue;
                card.level++;
                card.cardsToUpgrade = TowerCard.GetUpgradeRequirement(card.level,towerData.rarity);
                GameManager.Instance.SavePlayerData();

                // Gửi sự kiện nâng cấp
                OnTowerUpgraded?.Invoke(card);
                // Gọi lại Setup để cập nhật UI
                SetupCard(towerData);

            }
            else
            {
                Debug.Log("Không đủ thẻ để nâng cấp.");
            }
        });
        textUpgrade.text = ownedCards.ToString() + " / " + sliderUpgrade.maxValue;
        cardButton.onClickEvent.RemoveAllListeners();
        cardButton.onClickEvent.AddListener(OnClick);
    }

    public void SetupCardTeam(TowerData towerData)
    {
        this.towerData = towerData;

        cardButton.onClickEvent.RemoveAllListeners();
        cardButton.onClickEvent.AddListener(OnClick);
    }

    public void SetupNotOwned(TowerData towerData)
    {
        this.towerData = towerData;
        var panelNotOwned = transform.Find("Not Owned");
        Debug.Log(towerData);
        panelNotOwned.gameObject.SetActive(true);
        TowerCard card = GameManager.Instance.PlayerData.ownedTowerCards.Find(c => c.towerName == towerData.towerName);
        sliderUpgrade.maxValue = GameManager.Instance.GetUpgradeRequirement(towerData.rarity);
        if (card == null) return;
        unlockButton.interactable = card.ownedCards >= card.GetRequiredToUnlock(); // Cho click nếu đủ
        unlockButton.onClickEvent.RemoveAllListeners();
        unlockButton.onClickEvent.AddListener(() => TryUnlock(card));

        
        Debug.Log(card);
        sliderUpgrade.value = card.ownedCards;
        textUpgrade.text = card.ownedCards.ToString() + " / " + sliderUpgrade.maxValue;
    }

    private void OnClick()
    {
        UIManager.Instance.ShowTowerDetail(towerData);
    }

    private void TryUnlock(TowerCard card)
    {
        int required = card.GetRequiredToUnlock();
        if (card.ownedCards >= required)
        {
            card.level = 1;
            card.ownedCards -= required;
            var panelNotOwned = transform.Find("Not Owned");
            panelNotOwned.gameObject.SetActive(false);
            TowerCardDisplayManager displayManager = FindFirstObjectByType<TowerCardDisplayManager>();
            if (displayManager != null)
            {
                displayManager.DisplayAllTowers();
                displayManager.LoadTeamUI();
            }
            GameManager.Instance.SavePlayerData();
            Debug.Log($"Đã mở khóa tháp: {card.towerName}");

            // Gọi lại Setup để cập nhật UI
            SetupCard(towerData);
        }
        else
        {
            Debug.Log("Chưa đủ số thẻ để mở khóa tháp.");
            // Có thể hiển thị popup nếu cần
        }
    }
}
