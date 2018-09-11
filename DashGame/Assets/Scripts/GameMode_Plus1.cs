using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_Plus1 : MonoBehaviour
{
    public float targetTravelSpeed;

    Transform wallW;
    Transform wallE;
    BallController ballC;
    PaddleController paddle;
    AdManager ads;
    LevelGenerator LG;
    ObstacleSpawner obSpawner;

    Coroutine spawnBalls;
    int balls2Spawn;
    int tempBalls2Spawn;
    float spawnHeight;
    Vector2 randomPos;

    void Start()
    {
        ballC = BallController.Instance;
        paddle = PaddleController.Instance;
        ads = AdManager.Instance;
        LG = LevelGenerator.Instance;
        obSpawner = ObstacleSpawner.Instance;

        wallW = transform.Find("wallW");
        wallE = transform.Find("wallE");

        spawnHeight = Camera.main.orthographicSize + 0.25f;
    }

    private void OnEnable()
    {
        OtherGameModesManager.GameModeStarted += PlusOneStarted;
        ObstacleSpawner.ObstacleSet += ObstacleSet;
    }

    private void OnDisable()
    {
        OtherGameModesManager.GameModeStarted -= PlusOneStarted;
        ObstacleSpawner.ObstacleSet -= ObstacleSet;
    }

    void PlusOneStarted()
    {
        obSpawner.SpawnObstacle();
    }

    public int NumberOfBalls2Spawn
    {
        get
        {
            return balls2Spawn;
        }
    }

    void ObstacleSet()
    {
        print("obstalce set");
    }
}
