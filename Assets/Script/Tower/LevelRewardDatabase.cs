using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelRewardDatabase", menuName = "Level/Create Reward Database")]
public class LevelRewardDatabase : ScriptableObject
{
    public List<LevelRewardData> allRewards;

    public LevelRewardData GetRewardForLevel(int levelId)
    {
        return allRewards.Find(r => r.levelId == levelId);
    }
}