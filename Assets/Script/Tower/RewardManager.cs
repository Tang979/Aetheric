using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }
    public LevelRewardDatabase rewardDatabase;
    public TowerCardDatabase towerDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Nếu bạn muốn giữ khi chuyển scene
    }

    public List<(TowerData tower, int quantity)> GrantLevelRewards(int levelId)
    {
        List<(TowerData tower, int quantity)> grantedRewards = new();

        var rewardData = rewardDatabase.GetRewardForLevel(levelId);
        if (rewardData == null) return grantedRewards;

        // Bảng đếm phần thưởng, key: towerName
        Dictionary<string, int> rewardCounter = new();

        foreach (var reward in rewardData.cardRewards)
        {
            switch (reward.rewardType)
            {
                case LevelRewardData.CardReward.RewardType.SpecificTower:
                    if (!reward.towerData.isComingSoon)
                        AddToRewardCounter(reward.towerData.towerName, reward.quantity);
                    break;

                case LevelRewardData.CardReward.RewardType.RandomByRarity:
                    {
                        var candidates = towerDatabase.GetAllTowersByRarity(reward.rarity)
                                                      .Where(t => !t.isComingSoon)
                                                      .ToList();

                        if (candidates.Count == 0) break;

                        // Chọn ngẫu nhiên một số loại tháp giới hạn
                        var selectedTypes = candidates
                            .OrderBy(_ => Random.value)
                            .Take(reward.maxRandomTowerTypes)
                            .ToList();

                        for (int i = 0; i < reward.quantity; i++)
                        {
                            var randomTower = selectedTypes[Random.Range(0, selectedTypes.Count)];
                            AddToRewardCounter(randomTower.towerName, 1);
                        }
                        break;
                    }
            }
        }

        // Gộp và ghi dữ liệu
        foreach (var entry in rewardCounter)
        {
            string towerName = entry.Key;
            int quantity = entry.Value;

            GameManager.Instance.PlayerData.AddTowerCard(towerName, quantity);

            var towerData = towerDatabase.GetTowerByName(towerName);
            if (towerData != null)
                grantedRewards.Add((towerData, quantity));
        }

        GameManager.Instance.SavePlayerData();
        return grantedRewards;

        // Gộp phần thưởng vào bảng đếm
        void AddToRewardCounter(string towerName, int amount)
        {
            if (string.IsNullOrEmpty(towerName)) return;

            if (rewardCounter.ContainsKey(towerName))
                rewardCounter[towerName] += amount;
            else
                rewardCounter[towerName] = amount;
        }
    }
}
