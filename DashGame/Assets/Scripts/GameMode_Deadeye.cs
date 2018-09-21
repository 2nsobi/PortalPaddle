using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_Deadeye : MonoBehaviour
{

    OtherGameModesManager gameModeManager;
    ObstacleSpawner obSpawner;
    BallController ballC;
    TargetController targetC;
    static CryptoRandom rng = new CryptoRandom();

    Coroutine startNextRound;
    Coroutine ballDropDelay;

    bool firstPlay = true;

    public static GameMode_Deadeye Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameModeManager = OtherGameModesManager.Instance;
        obSpawner = ObstacleSpawner.Instance;
        ballC = BallController.Instance;
        targetC = TargetController.Instance;
    }

    private void OnEnable()
    {
        OtherGameModesManager.GameModeStarted += DeadeyeStarted;
        Ball.AbsorbDone += AbsorbDone;
        Ball.AbsorbDoneAndRichochet += AbsorbDone;
        Ball.PlayerMissed += PlayerMissed;

        firstPlay = true;
    }

    private void OnDisable()
    {
        OtherGameModesManager.GameModeStarted -= DeadeyeStarted;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDone;
        Ball.PlayerMissed -= PlayerMissed;
    }

    void DeadeyeStarted()
    {
        if (firstPlay)
        {
            targetC.SetTargetColor(Color.red);
            firstPlay = false;
        }

        ballC.IncreaseDropSpeed(18,20);//(20, 22);

        ballDropDelay = StartCoroutine(BallDropDelay());
    }

    IEnumerator BallDropDelay()
    {
        targetC.SpawnTarget(transform, false, false);
        yield return new WaitForSeconds(RandomFloat(0, 5.1)); //target spawn anim is 1 second long
        ballC.SpawnQuickBall();
    }

    IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(0.5f); //target shrink anim is 0.3 seconds long

        ballDropDelay = StartCoroutine(BallDropDelay());
    }

    float RandomFloat(double min, double max)
    {
        return (float)(min + rng.NextDouble() * (max - min));
    }

    void AbsorbDone()
    {
        gameModeManager.Scored();

        startNextRound = StartCoroutine(StartNextRound());
    }

    void PlayerMissed()
    {
        gameModeManager.Missed();
    }
}
