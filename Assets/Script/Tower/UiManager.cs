using UnityEngine;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine.UI;
using TMPro;
using Doozy.Runtime.UIManager.Components; // UIView

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIPopup towerDetailView, tempTeam;
    public Image iconImage;
    public GameObject teamCardPrefab;
    public UIButton equipButton, upgradeButton;
    public TextMeshProUGUI nameLabel, txtDame, txtDameAfter, txtSpeed, txtSpeedAfter;
    public TextMeshProUGUI descriptionLabel;

    public void Start()
    {
        Instance = this;
        towerDetailView.Hide();
        Debug.Log("Ẩn detail");
        tempTeam.Hide();
    }

    public void ShowTowerDetail(TowerData data)
    {
        towerDetailView.Show();
        Debug.Log("Hien detail");

        var card = GameManager.Instance.PlayerData.Team.Find(t => t == data.towerName);
        if (card != null)
        {
            var textEquip = equipButton.transform.Find("Label").transform.GetComponent<TextMeshProUGUI>();
            textEquip.text = "Unequip";

            equipButton.onClickEvent.RemoveAllListeners();
            equipButton.onClickEvent.AddListener(() =>
            GameManager.Instance.RemoveTowerFromTeam(data.towerName));
        }
        else
        {
            equipButton.onClickEvent.RemoveAllListeners();
            equipButton.onClickEvent.AddListener(() =>
            GameManager.Instance.TryAddTowerToTeam(data.towerName));

            var textEquip = equipButton.transform.Find("Label").transform.GetComponent<TextMeshProUGUI>();
            textEquip.text = "Equip";
        }

        upgradeButton.onClickEvent.RemoveAllListeners();
        upgradeButton.onClickEvent.AddListener(() =>
        {
            TowerCard card = GameManager.Instance.PlayerData.ownedTowerCards.Find(c => c.towerName == data.towerName);
            var ownedCards = card.ownedCards;
            if (TryUpgradeTower(data, out card))
            {
                ShowTowerDetail(data);
            }
        });

        iconImage.sprite = data.icon;
        nameLabel.text = data.name;
        descriptionLabel.text = data.descriptionSkill;
        var towerCard = GameManager.Instance.PlayerData.ownedTowerCards.Find(c => c.towerName == data.towerName);
        txtDame.text = "Dame: " + TowerData.GetDamage(data, towerCard.level);
        txtSpeed.text = "Speed: " + TowerData.GetAttackSpeed(data, towerCard.level);
        txtDameAfter.text = TowerData.GetDamage(data, towerCard.level + 1).ToString();
        txtSpeedAfter.text = TowerData.GetAttackSpeed(data, towerCard.level + 1).ToString();
    }

    public bool TryUpgradeTower(TowerData towerData, out TowerCard upgradedCard)
    {
        upgradedCard = GameManager.Instance.PlayerData.ownedTowerCards
            .Find(c => c.towerName == towerData.towerName);

        if (upgradedCard == null)
        {
            Debug.LogWarning($"Không tìm thấy thẻ {towerData.towerName} để nâng cấp");
            return false;
        }

        int requiredCards = upgradedCard.GetRequiredToUpgrade();

        if (upgradedCard.ownedCards >= requiredCards)
        {
            upgradedCard.ownedCards -= requiredCards;
            upgradedCard.level++;
            upgradedCard.cardsToUpgrade = TowerCard.GetUpgradeRequirement(upgradedCard.level, towerData.rarity);
            GameManager.Instance.SavePlayerData();

            // Gửi sự kiện nếu có UI cần nghe
            CardUI.OnTowerUpgraded?.Invoke(upgradedCard);

            Debug.Log($"Đã nâng cấp {upgradedCard.towerName} lên cấp {upgradedCard.level}");
            return true;
        }
        else
        {
            Debug.Log($"Không đủ thẻ để nâng cấp {upgradedCard.towerName}. Hiện có {upgradedCard.ownedCards}/{requiredCards}");
            return false;
        }
    }

    public void loadTempTeam(string tower)
    {
        tempTeam.Show();
        towerDetailView.Hide();
        var team = GameManager.Instance.PlayerData.Team;
        var newTower = tower;
        ClearChildren(tempTeam.transform);
        foreach (var teamCard in team)
        {
            var towerData = GameManager.Instance.towerCardDatabase.GetTowerByName(teamCard);
            var card = GameManager.Instance.PlayerData.ownedTowerCards.Find(t => t.towerName == towerData.towerName);
            var cardGO = Instantiate(teamCardPrefab, tempTeam.transform);
            var cardTeam = cardGO.GetComponent<CardUI>();
            var icon = cardGO.transform.Find("Vertical/Button - ShowTowerDetailPopup/Icon");
            icon.GetComponent<Image>().sprite = towerData.icon;

            var button = cardGO.transform.Find("Vertical/Button - ShowTowerDetailPopup").GetComponent<UIButton>();
            button.onClickEvent.RemoveAllListeners();
            button.onClickEvent.AddListener(() =>
            GameManager.Instance.ReplaceTowerInTeam(towerData.towerName, newTower));
            cardGO.transform.Find("Vertical/txt_level").GetComponent<TMP_Text>().text = $"Lv {card.level}";
        }
    }
    private void ClearChildren(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
