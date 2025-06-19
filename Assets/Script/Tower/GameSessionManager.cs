using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [SerializeField] private List<GameObject> selectedSpecialTowers; // Danh sách prefab tháp đặc biệt

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetSelectedSpecialTowers(List<GameObject> towerPrefabs) {
        selectedSpecialTowers = towerPrefabs;
    }

    public GameObject GetRandomSpecialTowerPrefab() {
        int index = Random.Range(0, selectedSpecialTowers.Count);
        return selectedSpecialTowers[index];
    }
}