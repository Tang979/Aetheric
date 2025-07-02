using UnityEngine;
using TMPro;

public class TowerCardDisplayManager : MonoBehaviour
{
    [SerializeField] private TowerCardDatabase database;

    [Header("Container theo độ hiếm")]
    [SerializeField] private Transform ownedContainer;
    [SerializeField] private Transform commonContainer;
    [SerializeField] private Transform rareContainer;
    [SerializeField] private Transform epicContainer;
    [SerializeField] private Transform legendaryContainer;

    [Header("UI hiển thị đội hình")]
    [SerializeField] private Transform defaultSlot;
    [SerializeField] private Transform horizontalContainer;

    [Header("Prefab thẻ")]
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private GameObject teamCardPrefab;

    private void Start()
    {
        DisplayAllTowers();
        LoadTeamUI();

        // Đăng ký sự kiện nâng cấp
        CardUI.OnTowerUpgraded += RefreshCardUI;
    }

    private void OnDestroy()
    {
        CardUI.OnTowerUpgraded -= RefreshCardUI;
    }

    public void DisplayAllTowers()
    {
        ClearChildren(commonContainer);
        ClearChildren(rareContainer);
        ClearChildren(epicContainer);
        ClearChildren(legendaryContainer);
        ClearChildren(ownedContainer);

        var ownedCards = GameManager.Instance.PlayerData.ownedTowerCards;

        foreach (var tower in database.allTowers)
        {
            bool isOwned = ownedCards.Exists(card => card.towerName == tower.towerName && card.level > 0);
            Transform targetContainer = isOwned ? ownedContainer : GetContainerByRarity(tower.rarity);
            if (targetContainer == null) continue;

            var cardGO = Instantiate(towerCardPrefab, targetContainer);

            var icon = cardGO.transform.Find("Vertical/Button - Upgrade/Icon").GetComponent<UnityEngine.UI.Image>();

            if (tower.isComingSoon)
            {
                icon.sprite = tower.icon;
                var panelCommingSoon = cardGO.transform.Find("Comming Soon");
                panelCommingSoon.gameObject.SetActive(true);
            }
            else if (isOwned)
            {
                icon.sprite = tower.icon;
                icon.color = Color.white;

                if (isOwned)
                {
                    var cardUI = cardGO.GetComponent<CardUI>();
                    cardUI.SetupCard(tower);

                    var lvText = cardGO.transform.Find("Vertical/txt_level").GetComponent<TMP_Text>();
                    if (lvText != null)
                        lvText.text = $"Lv {GetTowerLevel(tower.towerName)}";
                }
            }
            else
            {
                icon.sprite = tower.icon;
                var lvText = cardGO.transform.Find("Vertical/txt_level").GetComponent<TMP_Text>();
                if (lvText != null)
                        lvText.text = $"Lv {GetTowerLevel(tower.towerName)}";
                var cardUI = cardGO.GetComponent<CardUI>();
                cardUI.SetupNotOwned(tower);
                Debug.Log("panel not owned");
            }


        }
    }

    public void LoadTeamUI()
    {
        ClearChildren(horizontalContainer);
        ClearChildren(defaultSlot);

        var team = GameManager.Instance.PlayerData.Team;

        // Basic tower
        string basicId = "Basic";
        var basicTower = database.GetTowerByName(basicId);
        var basicCard = Instantiate(teamCardPrefab, defaultSlot);
        var rect = basicCard.GetComponent<RectTransform>();
        SetAnchorBottomRight(rect);

        var card = basicCard.GetComponent<CardUI>();
        card.SetupCardTeam(basicTower);
        basicCard.transform.Find("Vertical/Button - Upgrade/Icon").GetComponent<UnityEngine.UI.Image>().sprite = basicTower.icon;
        basicCard.transform.Find("Vertical/txt_level").GetComponent<TMP_Text>().text = $"Lv {GetTowerLevel(basicId)}";

        // Team towers
        foreach (var teamCard in team)
        {
            var towerData = database.GetTowerByName(teamCard);
            var cardGO = Instantiate(teamCardPrefab, horizontalContainer);
            var cardTeam = cardGO.GetComponent<CardUI>();
            cardTeam.SetupCardTeam(towerData);
            cardGO.transform.Find("Vertical/Button - Upgrade/Icon").GetComponent<UnityEngine.UI.Image>().sprite = towerData.icon;
            cardGO.transform.Find("Vertical/txt_level").GetComponent<TMP_Text>().text = $"Lv {GetTowerLevel(towerData.towerName)}";
        }
    }

    public static void SetAnchorBottomRight(RectTransform rt)
    {
        // Lưu lại thông tin trước khi thay đổi
        Vector3 worldPos = rt.position;
        Vector2 size = rt.rect.size;

        // Đổi anchor và pivot
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);

        // Gán lại vị trí để giữ đúng vị trí ban đầu
        rt.position = worldPos;
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;
    }

    private void RefreshCardUI(TowerCard towerCard)
    {
        Debug.Log($"Đang làm mới UI cho tháp: {towerCard.towerName}");

        // Cập nhật toàn bộ UI nếu bạn muốn đơn giản
        DisplayAllTowers();
        LoadTeamUI();

        // Nếu cần tối ưu hơn (chỉ cập nhật đúng thẻ trong owned/team), mình có thể viết thêm
    }

    private Transform GetContainerByRarity(TowerData.TowerRarity rarity)
    {
        return rarity switch
        {
            TowerData.TowerRarity.Common => commonContainer,
            TowerData.TowerRarity.Rare => rareContainer,
            TowerData.TowerRarity.Epic => epicContainer,
            TowerData.TowerRarity.Legendary => legendaryContainer,
            _ => null
        };
    }

    private int GetTowerLevel(string towerName)
    {
        var card = GameManager.Instance.PlayerData.ownedTowerCards.Find(t => t.towerName == towerName);
        return card != null ? card.level : 0;
    }

    private void ClearChildren(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
