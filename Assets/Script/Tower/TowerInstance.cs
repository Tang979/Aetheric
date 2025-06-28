using System;
using UnityEngine;

public class TowerInstance : MonoBehaviour
{
    public TowerData data;
    private int level = 1;
    public void SetLevel(int newLevel)
    {
        level = newLevel;
    }
    public int Level => level;
    private int maxLevel = 5;

    public float CurrentDamage { get; private set; }
    public float CurrentAttackSpeed { get; private set; }
    public float CurrentTickRate { get; private set; }

    private int valueTower;
    private ITowerAttack attackLogic;

    public void SetValueTower(int newValue)
    {
        valueTower = newValue;
    }
    public int ValueTower => valueTower;

    private void Start()
    {
        if (valueTower <= 0)
        valueTower = UpgradeCostCalculator.GetUpgradeCost(level, data.rarity);
        InitAttackLogic();
        UpdateStats();
    }

    void InitAttackLogic()
    {
        switch (data.attackType)
        {
            case TowerData.AttackType.Projectile:
                attackLogic = gameObject.AddComponent<ProjectileAttack>();
                break;
            case TowerData.AttackType.Spray:
                attackLogic = gameObject.AddComponent<SprayAttack>();
                break;
        }

        attackLogic?.Init(this);
    }

    private void Update()
    {
        attackLogic?.Tick(Time.deltaTime);
    }

    public void Upgrade()
    {
        int cost = GetUpgradeCost();
        if (LevelManager.main.TrySpendGold(cost))
        {
            level++;
            if (level == 3 && data.isBasicTower)
            {
                // Chuyển đổi từ tháp cơ bản sang tháp đặc biệt
                Transform parent = transform.parent;
                Vector3 pos = transform.position;
                Quaternion rot = transform.rotation;

                GameObject newTowerPrefab = GameSessionManager.Instance.GetRandomSpecialTowerPrefab();

                GameObject newTower = Instantiate(newTowerPrefab, pos, rot, parent);
                newTower.GetComponent<TowerInstance>().SetLevel(level);
                valueTower += GetUpgradeCost();
                newTower.GetComponent<TowerInstance>().SetValueTower(valueTower);
                Debug.Log("Value Tower after upgrade: " + valueTower);
                newTower.GetComponent<TowerInstance>().UpdateStats();
                Destroy(this.gameObject);
                TowerSelector selector = GetComponent<TowerSelector>();
                selector.UpdatePanel(newTower.GetComponent<TowerInstance>());
                return;
            }
            valueTower += GetUpgradeCost();
            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        CurrentDamage = data.baseDamage * (1 + 0.2f * (level - 1));

        switch (data.attackType)
        {
            case TowerData.AttackType.Projectile:
                CurrentAttackSpeed = data.projectileConfig.attackSpeed * Mathf.Pow(1.1f, level - 1);
                break;
            case TowerData.AttackType.Spray:
                CurrentTickRate = data.sprayConfig.tickRate * Mathf.Pow(0.95f, level - 1);
                break;
        }
    }

    public int GetUpgradeCost()
    {
        var rarity = TowerData.GetStats(data.rarity);
        return rarity.GetUpgradeCost(level);
    }

    public int GetSellValue()
    {
        return Mathf.RoundToInt(valueTower * 0.7f);
    }

    public void Sell()
    {
        var towerSlot = GetComponentInParent<TowerSlot>();
        int refund = GetSellValue();
        towerSlot.ClearSlot();
        LevelManager.main.AddGold(refund);
        Destroy(gameObject);
    }

    public bool isMaxLevel()
    {
        return level >= maxLevel;
    }
    internal bool CanUpgrade(int level)
    {
        return level < maxLevel;
    }
}
