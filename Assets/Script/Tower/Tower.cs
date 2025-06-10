using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LayerMask enemyLayer; // lớp của kẻ thù để quét
    public float scanInterval = 0.5f; // Interval for enemy scanning

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Empty update method
    }
    
}
