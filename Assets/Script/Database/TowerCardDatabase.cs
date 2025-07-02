using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/TowerCardDatabase")]
public class TowerCardDatabase : ScriptableObject
{
    public List<TowerData> allTowers;

    public TowerData GetTowerByName(string name)
    {
        return allTowers.Find(t => t.towerName == name);
    }

    public List<TowerData> GetTowersByRarity(TowerData.TowerRarity rarity)
    {
        return allTowers.FindAll(t => t.rarity == rarity);
    }

    public TowerData GetRandomTowerByRarity(TowerData.TowerRarity rarity)
    {
        var towers = GetTowersByRarity(rarity);
        return towers[Random.Range(0, towers.Count)];
    }
    public List<TowerData> GetAllTowersByRarity(TowerData.TowerRarity rarity)
    {
        return allTowers.FindAll(t => t.rarity == rarity);
    }
    
    [ContextMenu("Auto Load All Towers")]
    public void LoadAllTowers()
    {
        allTowers.Clear();
        TowerData[] loaded = Resources.LoadAll<TowerData>("Towers");
        allTowers.AddRange(loaded);
    }
}
