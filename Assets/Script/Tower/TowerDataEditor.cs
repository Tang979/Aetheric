using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerData))]
public class TowerDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TowerData towerData = (TowerData)target;
        serializedObject.Update();
        towerData.towerId = EditorGUILayout.TextField("Tower ID", towerData.towerId);
        towerData.name = EditorGUILayout.TextField("Tower Name", towerData.name);
        towerData.attackType = (TowerData.AttackType)EditorGUILayout.EnumPopup("Attack Type", towerData.attackType);
        towerData.attackRange = EditorGUILayout.FloatField("Attack Range", towerData.attackRange);
        towerData.baseDamage = EditorGUILayout.FloatField("Base Damage", towerData.baseDamage);
        towerData.bulletPrefab = EditorGUILayout.ObjectField("Bullet Prefab", towerData.bulletPrefab, typeof(GameObject), false) as GameObject;

        switch (towerData.attackType)
        {
            case TowerData.AttackType.Projectile:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileConfig"), true);
                break;
            case TowerData.AttackType.Spray:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sprayConfig"), true);
                break;
        }
        serializedObject.ApplyModifiedProperties(); // Thêm dòng này để lưu các thay đổi    
    }

}
