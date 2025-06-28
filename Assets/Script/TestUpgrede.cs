using UnityEngine;

public class TestUpgrade : MonoBehaviour
{
    public TowerUpgradeUI fireUI;
    public TowerUpgradeUI electricUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) fireUI.AddFragments(1);
        if (Input.GetKeyDown(KeyCode.E)) electricUI.AddFragments(1);
    }
}

