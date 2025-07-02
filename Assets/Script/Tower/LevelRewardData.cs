using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelRewardData", menuName = "Level/Create Level Reward")]
public class LevelRewardData : ScriptableObject
{
    public int levelId;
    public List<CardReward> cardRewards;

    [System.Serializable]
    public class CardReward
    {
        public enum RewardType
        {
            SpecificTower,
            RandomByRarity
        }

        public RewardType rewardType;
        public TowerData towerData; // dùng cho SpecificTower
        public TowerData.TowerRarity rarity; // dùng cho RandomByRarity
        public int quantity;
        public int maxRandomTowerTypes = 3; // Giới hạn số loại tháp được random
    }

    private LevelRewardData GetRewardForCurrentLevel()
    {
        LevelRewardData[] allRewards = Resources.LoadAll<LevelRewardData>("LevelRewards");
        return System.Array.Find(allRewards, r => r.levelId == levelId);
    }
}