using UnityEngine;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine.UI;
using TMPro; // UIView

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public UIView towerDetailView;
    public Image iconImage;
    public TextMeshProUGUI nameLabel, txtDame, txtSpeed;
    public TextMeshProUGUI descriptionLabel;

    public void ShowTowerDetail(TowerData data)
    {
        iconImage.sprite = data.icon;
        nameLabel.text = data.name;
        descriptionLabel.text = data.descriptionSkill;
        txtDame.text = "Dame: "+data.baseDamage;
        txtSpeed.text = "Speed: "+data.attackSpeed;

        towerDetailView.Show(); // Doozy: mở UIView
    }
}
