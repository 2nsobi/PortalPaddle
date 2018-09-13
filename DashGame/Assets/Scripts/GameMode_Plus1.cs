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
    OtherGameModesManager gameModeManager;

    Coroutine spawnBalls;
    int balls2Spawn;
    int tempBalls2Spawn;
    bool gameOver = false;

    void Start()
    {
        ballC = BallController.Instance;
        paddle = PaddleController.Instance;
        ads = AdManager.Instance;
        LG = LevelGenerator.Instance;
        obSpawner = ObstacleSpawner.Instance;
        gameModeManager = OtherGameModesManager.Instance;

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
            yield return new WaitForSeconds(0.8f);
        }
    }

    void PlusOneStarted()
    {
        gameOver = false;

        balls2Spawn = 1;
        tempBalls2Spawn = balls2Spawn;

        ballC.SetBalls2Absorb(balls2Spawn);

        obSpawner.SpawnObstacle();
    }

    void ObstacleSet()
    {
        spawnBalls = StartCoroutine(SpawnBalls());
    }
    
    void AbsorbDone()
    {
        if (!gameOver)
        {
            tempBalls2Spawn--;

            if (tempBalls2Spawn == 0)
            {
                gameModeManager.Scored();

                balls2Spawn++;
                tempBalls2Spawn = balls2Spawn;

                ballC.SetBalls2Absorb(balls2Spawn);

                obSpawner.DespawnObstacle();
            }
        }
    }

    void PlayerMissed()
    {
        if (!gameOver)
        {
            gameOver = true;
            StopCoroutine(spawnBalls);
            gameModeManager.Missed();
        }
    }

    public int NumberOfBalls2Spawn
    {
        get
        {
            return balls2Spawn;
        }
    }
}
