using UnityEngine;

public class ZoneAttack : MonoBehaviour,ITowerAttack
{
    private TowerInstance towerInstance;

    // Init method to set the tower instance
    public void Init(TowerInstance towerInstance)
    {
        this.towerInstance = towerInstance;
    }

    // Tick method to handle the attack logic
    public void Tick(float deltaTime)
    {
        // Implement zone attack logic here
        // For example, check for enemies in range and apply damage over time
        Debug.Log("Zone attack logic executed.");
    }
}
