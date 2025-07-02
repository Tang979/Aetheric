using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerSpawnManager : MonoBehaviour
{
    public static TowerSpawnManager Instance { get; private set; }

    private List<TowerSlot> slots = new();
    private const int coinSummon = 40; // Số tiền cần để triệu hồi tháp
    public GameObject basicTowerPrefab;

    public int GetCoinSummon() => coinSummon;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        slots = new List<TowerSlot>(FindObjectsByType<TowerSlot>(FindObjectsSortMode.None));
    }

    public void SpawnTower()
    {
        if (LevelManager.main.currentGold < coinSummon)
        {
            Debug.LogWarning("Not enough gold to summon a tower.");
            return;
        }
        TowerSlot slot = GetRandomFreeSlot();
        if (slot == null)
        {
            Debug.LogWarning("No free tower slots available.");
            return;
        }
        else
        {
            slot.PlaceTower(basicTowerPrefab);
            LevelManager.main.currentGold -= coinSummon;
            UILevelManager.instance.UpdateMoney();
            Debug.Log("Tower spawned at slot: " + slot.name);
        }
    }

    public TowerSlot GetRandomFreeSlot()
    {
        var freeSlots = slots.Where(s => !s.IsOccupied).ToList();
        if (freeSlots.Count == 0) return null;
        return freeSlots[Random.Range(0, freeSlots.Count)];
    }
}
