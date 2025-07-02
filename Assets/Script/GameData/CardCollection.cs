using System;
using System.Collections.Generic;

[Serializable]
public class CardCollection
{
    public List<TowerCard> ownedCards = new();

    public TowerCard GetCard(string id)
    {
        return ownedCards.Find(c => c.towerName == id);
    }

    /// <summary>
    /// Thêm thẻ vào bộ sưu tập, khởi tạo nếu chưa có.
    /// </summary>
    public void AddCard(TowerData towerData, int amount)
    {
        var card = GetCard(towerData.towerName);
        if (card == null)
        {
            int unlock = TowerCard.GetUnlockRequirement(towerData.rarity);
            int upgrade = TowerCard.GetUpgradeRequirement(1, towerData.rarity);
            card = new TowerCard(towerData.towerName, unlock, upgrade);
            ownedCards.Add(card);
        }

        card.ownedCards += amount;
    }

    /// <summary>
    /// Thử mở khóa nếu đủ thẻ.
    /// </summary>
    public bool TryUnlock(TowerData towerData)
    {
        var card = GetCard(towerData.towerName);
        if (card != null && !card.IsUnlocked && card.ownedCards >= card.cardsToUnlock)
        {
            card.level = 1;
            card.ownedCards -= card.cardsToUnlock;
            card.cardsToUpgrade = TowerCard.GetUpgradeRequirement(card.level, towerData.rarity);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Thử nâng cấp nếu đã mở khóa và đủ thẻ.
    /// </summary>
    public bool TryUpgrade(TowerData towerData)
    {
        var card = GetCard(towerData.towerName);
        if (card != null && card.IsUnlocked && card.ownedCards >= card.cardsToUpgrade)
        {
            card.ownedCards -= card.cardsToUpgrade;
            card.level++;
            card.cardsToUpgrade = TowerCard.GetUpgradeRequirement(card.level, towerData.rarity);
            return true;
        }
        return false;
    }
}
