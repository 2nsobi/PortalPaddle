using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_Clairvoyance : MonoBehaviour
{
    BallController ballC;
    TargetController targetC;
    OtherGameModesManager gameModeManager;
    ObstacleSpawner obSpawner;
    static CryptoRandom rng = new CryptoRandom();

    Color targetColor = new Color(0.9710112f,0,1,1);
    Coroutine randomizeBallSpeeds;
    Coroutine randomizeTargetSpeeds;

    bool firstPlay = true;
    bool gameRunning = false;

    public static GameMode_Clairvoyance Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ballC = BallController.Instance;
        targetC = TargetController.Instance;
        gameModeManager = OtherGameModesManager.Instance;
        obSpawner = ObstacleSpawner.Instance;
    }

    private void OnEnable()
    {
        OtherGameModesManager.GameModeStarted += ClairvoyanceStarted;
        ObstacleSpawner.ObstacleSet += ObstacleSet;
        Ball.AbsorbDone += AbsorbDone;
        Ball.AbsorbDoneAndRichochet += AbsorbDone;
        Ball.PlayerMissed += PlayerMissed;
    }

    private void OnDisable()
    {
        OtherGameModesManager.GameModeStarted -= ClairvoyanceStarted;
        ObstacleSpawner.ObstacleSet -= ObstacleSet;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDone;
        Ball.PlayerMissed -= PlayerMissed;
    }

    void ClairvoyanceStarted()
    {
        if (firstPlay)
        {
            targetC.SetTargetColor(targetColor);
            firstPlay = false;
        }

        gameRunning = true;

        randomizeBallSpeeds = StartCoroutine(RandomizeBallSpeeds());
        randomizeTargetSpeeds = StartCoroutine(RandomizeTargetSpeeds());

        obSpawner.AllowTargets2GrowShrink(true);
        obSpawner.SpawnObstacle();
    }

    IEnumerator RandomizeBallSpeeds()
    {
        while (gameRunning)
        {
            ballC.IncreaseDropSpeedImmediately(rng.Next(1,19), rng.Next(1,26));

            yield return new WaitForSeconds(rng.Next(2));
        }
    }

    IEnumerator RandomizeTargetSpeeds()
    {
        while (gameRunning)
        {
            targetC.IncreaseTravelSpeedImmediately(rng.Next(1,9));

            yield return new WaitForSeconds(rng.Next(2,4));
        }
    }

    void ObstacleSet()
    {
        ballC.SpawnQuickBallWithBallSpawner();
    }

    void AbsorbDone()
    {
        gameModeManager.Scored();

        obSpawner.DespawnObstacle(true);
    }

    void PlayerMissed()
    {
        gameModeManager.Missed();

        StopCoroutine(randomizeBallSpeeds);
        StopCoroutine(randomizeTargetSpeeds);

        gameRunning = false;
    }
}
