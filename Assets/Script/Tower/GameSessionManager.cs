using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [SerializeField] private List<GameObject> selectedSpecialTowers; // Danh sách prefab tháp đặc biệt

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        SetSpecialTowersFromTeam(GameManager.Instance.PlayerData.Team, GameManager.Instance.towerCardDatabase);
    }

    public void SetSpecialTowersFromTeam(List<string> teamTowerIds, TowerCardDatabase database)
    {
        selectedSpecialTowers.Clear();

        foreach (var towerId in teamTowerIds)
        {
            TowerData data = database.GetTowerByName(towerId);
            if (data != null && data.specialTowerPrefab != null)
            {
                selectedSpecialTowers.Add(data.specialTowerPrefab);
            }
        }
    }

    public GameObject GetRandomSpecialTowerPrefab()
    {
        int index = Random.Range(0, selectedSpecialTowers.Count);
        return selectedSpecialTowers[index];
    }
}