using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("Bullet Pool Settings")]
    private Dictionary<GameObject, Queue<GameObject>> poolDict = new();

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetBullet(GameObject bulletPrefab)
    {
        if (!poolDict.ContainsKey(bulletPrefab))
        {
            poolDict[bulletPrefab] = new Queue<GameObject>();
        }

        var pool = poolDict[bulletPrefab];

        GameObject bullet;

        if (pool.Count > 0)
        {
            bullet = pool.Dequeue();
        }
        else
        {
            bullet = Instantiate(bulletPrefab);
            // 👇 Nếu bạn dùng Bullet.cs và cần gán prefabOrigin
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.prefabOrigin = bulletPrefab;
            }
        }

        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet, GameObject prefabType)
    {
        bullet.SetActive(false);

        if (!poolDict.ContainsKey(prefabType))
        {
            poolDict[prefabType] = new Queue<GameObject>();
        }

        poolDict[prefabType].Enqueue(bullet);
    }
}
