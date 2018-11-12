using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_Plus1 : MonoBehaviour
{
    BallController ballC;
    PaddleController paddle;
    LevelGenerator LG;
    ObstacleSpawner obSpawner;
    OtherGameModesManager gameModeManager;

    Coroutine spawnBalls;
    Coroutine moveWallsC;

    int balls2Spawn;
    int tempBalls2Spawn;
    bool gameRunning = false;
    float dropDelay;

    public static GameMode_Plus1 Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ballC = BallController.Instance;
        paddle = PaddleController.Instance;
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
        Ball.AbsorbDoneAndRichochet += AbsorbDoneAndRichochet;
        Ball.PlayerMissed += PlayerMissed;
    }

    private void OnDisable()
    {
        OtherGameModesManager.GameModeStarted -= PlusOneStarted;
        ObstacleSpawner.ObstacleSet -= ObstacleSet;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDoneAndRichochet;
        Ball.PlayerMissed -= PlayerMissed;
    }

    IEnumerator SpawnBalls()
    {
        for(int i = 0; i < balls2Spawn; i++)
        {
            ballC.SpawnQuickBall();
            yield return new WaitForSeconds(dropDelay);
        }
        dropDelay -= 0.1f;
        if(dropDelay < 1)
        {
            dropDelay = 1;
        }
    }

    void PlusOneStarted()
    {
        gameRunning = true;

        balls2Spawn = 1;
        tempBalls2Spawn = balls2Spawn;

        dropDelay = 2.1f; //remember to account for first drop where it is only one ball so drop delay doesnt actually matter

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

            gameModeManager.Scored();

            if (tempBalls2Spawn == 0)
            {
                balls2Spawn++;
                tempBalls2Spawn = balls2Spawn;

                ballC.SetBalls2Absorb(balls2Spawn);

                obSpawner.DespawnObstacle(true);
            }
        }
    }

    void AbsorbDoneAndRichochet()
    {
        if (gameRunning)
        {
            tempBalls2Spawn--;

            gameModeManager.DoubleScored();

            if (tempBalls2Spawn == 0)
            {
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
