using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_Plus1 : MonoBehaviour
{
    Transform wallW;
    Transform wallE;
    BallController ballC;
    PaddleController paddle;
    AdManager ads;
    LevelGenerator LG;
    ObstacleSpawner obSpawner;

    Coroutine spawnBalls;
    int balls2Spawn;

    void Start()
    {
        ballC = BallController.Instance;
        paddle = PaddleController.Instance;
        ads = AdManager.Instance;
        LG = LevelGenerator.Instance;
        obSpawner = ObstacleSpawner.Instance;

        ballC.CreateQOfBalls();

        wallW = transform.Find("wallW");
        wallE = transform.Find("wallE");
    }

    private void OnEnable()
    {
        OtherGameModesManager.GameModeStarted += PlusOneStarted;
        ObstacleSpawner.ObstacleSet += ObstacleSet;
        Ball.AbsorbDone += AbsorbDone;
        Ball.AbsorbDoneAndRichochet += AbsorbDone;
    }

    private void OnDisable()
    {
        OtherGameModesManager.GameModeStarted -= PlusOneStarted;
        ObstacleSpawner.ObstacleSet -= ObstacleSet;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDone;
    }

    IEnumerator SpawnBalls()
    {
        for(int i = 0; i < balls2Spawn; i++)
        {
            print("basd");
            ballC.SpawnQuickBall();
            yield return new WaitForSeconds(0.6f);
        }
    }

    void PlusOneStarted()
    {
        balls2Spawn = 6;

        obSpawner.SpawnObstacle();
    }

    void ObstacleSet()
    {
        StartCoroutine(SpawnBalls());
    }
    
    void AbsorbDone()
    {

    }

    public int NumberOfBalls2Spawn
    {
        get
        {
            return balls2Spawn;
        }
    }
}
