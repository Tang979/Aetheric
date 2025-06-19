using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerSpawnManager : MonoBehaviour
{
    public static TowerSpawnManager Instance { get; private set; }

    private List<TowerSlot> slots = new();
    public GameObject basicTowerPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        slots = new List<TowerSlot>(FindObjectsByType<TowerSlot>(FindObjectsSortMode.None));

    }

    public void SpawnTower()
    {
        TowerSlot slot = GetRandomFreeSlot();
        if (slot == null)
        {
            Debug.LogWarning("No free tower slots available.");
            return;
        }
        else
        {
            slot.PlaceTower(basicTowerPrefab);
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
