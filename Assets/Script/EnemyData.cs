using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Create Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Base Info")]
    public string enemyName;
    public GameObject prefab;

    [Header("Stats")]
    public float moveSpeed = 2f;
    public int health = 100;
}
