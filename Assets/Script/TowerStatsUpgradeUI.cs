using UnityEngine;
using TMPro;

public class TowerStatsUpgradeUI : MonoBehaviour
{
    [Header("Before Stats")]
    public TMP_Text txtRange;
    public TMP_Text txtAT;
    public TMP_Text txtDame;

    [Header("After Stats")]
    public TMP_Text txtRangeAfter;
    public TMP_Text txtATAfter;
    public TMP_Text txtDameAfter;

    // Dữ liệu gốc
    private int range = 3;
    private float attackSpeed = 0.8f;
    private int damage = 20;

    private void Start()
    {
        UpdateUI();
    }

    public void UpgradeStats()
    {
        // ➤ Không đọc lại từ UI
        range += 1;
        attackSpeed += 1f;
        damage += 1;

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Before = hiện tại
        txtRange.text = "Range: " + range.ToString();
        txtAT.text = "Attack Speed: " + attackSpeed.ToString("0.0") + "s";
        txtDame.text = "Dame: " + damage.ToString();


        // After = preview cho lần nâng tiếp theo (+1)
        txtRangeAfter.text = (range + 1).ToString();
        txtATAfter.text = (attackSpeed + 1f).ToString("0.0") + "s";
        txtDameAfter.text = (damage + 1).ToString();
    }
}
