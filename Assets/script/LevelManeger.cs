using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;
    public Transform StartPoint;
    public Transform[] Path;

    void Awake()
    {
        main = this;
    }
}

