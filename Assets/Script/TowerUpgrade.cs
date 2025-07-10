using UnityEngine;

[System.Serializable]
public class TowerUpgrade
{
    public int level = 1;
    public int fragments = 0;
    public int requiredFragments;

    public TowerUpgrade()
    {
        RecalculateCost(); // Tính cost ban đầu từ level = 1
    }

    public bool CanUpgrade() => fragments >= requiredFragments;

    public void RecalculateCost()
    {
        requiredFragments = Mathf.RoundToInt(0.5f * level * level + 4.5f * level);

    }

    public void Upgrade()
    {
        if (!CanUpgrade()) return;

        fragments -= requiredFragments;
        level++;
        RecalculateCost();
    }
}
