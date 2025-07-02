using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LevelRewardData.CardReward))]
public class CardRewardDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 3; // RewardType + Quantity

        var type = (LevelRewardData.CardReward.RewardType)property.FindPropertyRelative("rewardType").enumValueIndex;
        if (type == LevelRewardData.CardReward.RewardType.SpecificTower) lines++;
        if (type == LevelRewardData.CardReward.RewardType.RandomByRarity) lines++;

        return EditorGUIUtility.singleLineHeight * lines + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;
        float y = position.y;

        // Vẽ label phần tử
        EditorGUI.LabelField(new Rect(position.x, y, position.width, lineHeight), label.text, EditorStyles.boldLabel);
        y += lineHeight + spacing;

        // Vẽ RewardType
        var typeProp = property.FindPropertyRelative("rewardType");
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), typeProp);
        y += lineHeight + spacing;

        var type = (LevelRewardData.CardReward.RewardType)typeProp.enumValueIndex;

        // Vẽ trường tương ứng
        if (type == LevelRewardData.CardReward.RewardType.SpecificTower)
        {
            var towerDataProp = property.FindPropertyRelative("towerData");
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), towerDataProp, new GUIContent("Tower Data"));
            y += lineHeight + spacing;
        }
        else if (type == LevelRewardData.CardReward.RewardType.RandomByRarity)
        {
            var rarityProp = property.FindPropertyRelative("rarity");
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), rarityProp, new GUIContent("Rarity"));
            y += lineHeight + spacing;
        }

        // Luôn vẽ quantity
        var quantityProp = property.FindPropertyRelative("quantity");
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), quantityProp);

        EditorGUI.EndProperty();
    }
}