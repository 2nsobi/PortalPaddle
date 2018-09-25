using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_Plus1 : MonoBehaviour
{
    BallController ballC;
    PaddleController paddle;
    AdManager ads;
    LevelGenerator LG;
    ObstacleSpawner obSpawner;
    OtherGameModesManager gameModeManager;

    Coroutine spawnBalls;
    Coroutine moveWallsC;

    int balls2Spawn;
    int tempBalls2Spawn;
    bool gameRunning = false;

    public static GameMode_Plus1 Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ballC = BallController.Instance;
        paddle = PaddleController.Instance;
        ads = AdManager.Instance;
        LG = LevelGenerator.Instance;
        obSpawner = ObstacleSpawner.Instance;
        gameModeManager = OtherGameModesManager.Instance;

        ballC.CreateQOfBalls();
    }

    private void OnEnable()
    {
        OtherGameModesManager.GameModeStarted += PlusOneStarted;
        ObstacleSpawner.ObstacleSet += ObstacleSet;
        Ball.AbsorbDone += AbsorbDone;
        Ball.AbsorbDoneAndRichochet += AbsorbDone;
        Ball.PlayerMissed += PlayerMissed;
    }

    private void OnDisable()
    {
        OtherGameModesManager.GameModeStarted -= PlusOneStarted;
        ObstacleSpawner.ObstacleSet -= ObstacleSet;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDone;
        Ball.PlayerMissed -= PlayerMissed;
    }

    IEnumerator SpawnBalls()
    {
        for(int i = 0; i < balls2Spawn; i++)
        {
            ballC.SpawnQuickBall();
            yield return new WaitForSeconds(0.85f);
        }
    }

    void PlusOneStarted()
    {
        gameRunning = true;

        balls2Spawn = 1;
        tempBalls2Spawn = balls2Spawn;

        ballC.SetBalls2Absorb(balls2Spawn);

        obSpawner.SpawnObstacle();

        moveWallsC = StartCoroutine(MoveWalls());
    }

    void ObstacleSet()
    {
        spawnBalls = StartCoroutine(SpawnBalls());
    }
    
    void AbsorbDone()
    {
        if (gameRunning)
        {
            tempBalls2Spawn--;

            if (tempBalls2Spawn == 0)
            {
                gameModeManager.Scored();

                balls2Spawn++;
                tempBalls2Spawn = balls2Spawn;

                ballC.SetBalls2Absorb(balls2Spawn);

                obSpawner.DespawnObstacle(true);
            }
        }
    }

    void PlayerMissed()
    {
        if (gameRunning)
        {
            gameRunning = false;
            StopCoroutine(spawnBalls);
            StopCoroutine(moveWallsC);
            gameModeManager.Missed();
        }
    }

    IEnumerator MoveWalls()
    {
        int num = Random.Range(2, 9);

        bool trigger = true;
        bool moveWallsNow = false;

        if(num % 2 == 0)
        {
            moveWallsNow = true;
        }

        if (moveWallsNow)
        {
            while (gameRunning)
            {
                obSpawner.MoveWalls(trigger);
                yield return new WaitForSeconds(15);
                trigger = !trigger;
            }
        }
        else
        {
            while (gameRunning)
            {
                yield return new WaitForSeconds(15);
                obSpawner.MoveWalls(trigger);
                trigger = !trigger;
            }
        }
    }
}
