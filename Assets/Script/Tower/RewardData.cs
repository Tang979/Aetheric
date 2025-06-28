using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RewardData", menuName = "Reward/RewardData")]
public class RewardData : ScriptableObject
{
    public List<RewardEntry> rewards;
}

[System.Serializable]
public class RewardEntry
{
    public RewardType rewardType;
    public int amount;
    public string towerId; // Dùng cho thẻ tháp cùng loại hoặc xác định tháp cụ thể
    public bool isRandom;  // Nếu true thì lấy ngẫu nhiên
}

public enum RewardType
{
    Coin,
    Gem,
    TowerCard
}