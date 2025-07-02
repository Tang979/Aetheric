using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
    public GameObject panel;
    public Transform rewardContainer;
    public GameObject rewardSlotPrefab;

    public void Show(List<(TowerData tower, int quantity)> rewards)
    {
        panel.SetActive(true);

        // Xoá cũ
        foreach (Transform child in rewardContainer)
            Destroy(child.gameObject);

        // Tạo mới
        foreach (var reward in rewards)
        {
            var slot = Instantiate(rewardSlotPrefab, rewardContainer);
            slot.transform.Find("Icon").GetComponent<Image>().sprite = reward.tower.icon;
            slot.transform.Find("Quantity").GetComponent<TextMeshPro>().text = $"x{reward.quantity}";
        }
    }
}