using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    public void PlaceTower(GameObject towerPrefab)
    {
        if (IsOccupied) return;
        GameObject tower = Instantiate(towerPrefab, transform.position, Quaternion.identity, transform);
        IsOccupied = true;
    }
    public void ClearSlot()
    {
        IsOccupied = false;
        // Optionally destroy tower con
        if (transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);
    }
}
