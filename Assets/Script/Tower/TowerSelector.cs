using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelector : MonoBehaviour
{
    public LayerMask towerLayer;
    public TowerUIManager uiManager;

    private void Awake()
    {
        if (uiManager == null)
        {
            // Tìm TowerUIManager kể cả khi GameObject đang inactive
            uiManager = FindFirstObjectByType<TowerUIManager>(FindObjectsInactive.Include);
            if (uiManager == null)
            {
                Debug.LogError("TowerUIManager not found in the scene.");
            }
        }
        towerLayer = LayerMask.GetMask("Turret");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Không làm gì nếu đang click vào UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 50f, towerLayer);

            if (hit.collider != null)
            {
                TowerInstance tower = hit.collider.GetComponent<TowerInstance>();
                if (tower != null)
                {
                    uiManager.ShowPanel(tower);
                }
            }
            else
            {
                uiManager.HidePanel();
            }
        }
    }
    public void UpdatePanel(TowerInstance tower)
    {
        if (uiManager != null)
        {
            uiManager.ShowPanel(tower);
        }
        else
        {
            Debug.LogWarning("TowerUIManager is not assigned.");
        }
    }
}