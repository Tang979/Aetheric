using System;
using UnityEngine;

[RequireComponent(typeof(TargetingSystem))]
public class TowerInstance : MonoBehaviour
{
    public TowerData data;
    public Transform firePoint;

    private ITowerAttack attackLogic;
    private int currentLevel = 0;
    private int maxLevel = 5; // Giới hạn cấp độ tối đa

    private ITowerSkill[] skillModules;
    public int CurrentLevel => currentLevel;

    public void SetLevel(int level)
    {
        currentLevel = level;
    }

    private void Awake()
    {
        InitAttackLogic();
    }

    void InitAttackLogic()
    {
        // Xóa logic cũ nếu có
        if (attackLogic is MonoBehaviour old) Destroy(old);

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

    public void UpgradeTower()
    {
        currentLevel++;

        if (currentLevel == 2 && data.isBasicTower)
        {
            // Chuyển đổi từ tháp cơ bản sang tháp đặc biệt
            Transform parent = transform.parent;
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            GameObject newTowerPrefab = GameSessionManager.Instance.GetRandomSpecialTowerPrefab();

            Destroy(this.gameObject);
            GameObject newTower = Instantiate(newTowerPrefab, pos, rot, parent);
            newTower.GetComponent<TowerInstance>().SetLevel(currentLevel);
            TowerSelector selector = GetComponent<TowerSelector>();
            selector.UpdatePanel(newTower.GetComponent<TowerInstance>());
        }
    }

    void Update()
    {
        attackLogic?.Tick(Time.deltaTime);
    }

    internal bool CanUpgrade(int level)
    {
        return level < maxLevel && level >= currentLevel;
    }

    internal void SellTower()
    {
        var towerSlot = GetComponentInParent<TowerSlot>();
        towerSlot.ClearSlot();
        Destroy(gameObject);
    }
}