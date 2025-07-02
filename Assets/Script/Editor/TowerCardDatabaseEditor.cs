#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerCardDatabase))]
public class TowerCardDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TowerCardDatabase db = (TowerCardDatabase)target;

        if (GUILayout.Button("Auto Load All Towers"))
        {
            db.LoadAllTowers();
            EditorUtility.SetDirty(db); // Đánh dấu đã thay đổi
        }
    }
}
#endif
