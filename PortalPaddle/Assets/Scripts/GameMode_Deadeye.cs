using System.Collections;
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
    Coroutine moveWallsC;

    bool firstPlay = true;
    bool gameRunning = false;

    public static GameMode_Deadeye Instance;

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
        gameModeManager = OtherGameModesManager.Instance;
        obSpawner = ObstacleSpawner.Instance;
        ballC = BallController.Instance;
        targetC = TargetController.Instance;
    }

    private void OnEnable()
    {
        OtherGameModesManager.GameModeStarted += DeadeyeStarted;
        Ball.AbsorbDone += AbsorbDone;
        Ball.AbsorbDoneAndRichochet += AbsorbDoneAndRichochet;
        Ball.PlayerMissed += PlayerMissed;

        firstPlay = true;
    }

    private void OnDisable()
    {
        OtherGameModesManager.GameModeStarted -= DeadeyeStarted;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDoneAndRichochet;
        Ball.PlayerMissed -= PlayerMissed;
    }

    void DeadeyeStarted()
    {
        gameRunning = true;

        if (firstPlay)
        {
            targetC.SetTargetColor(Color.red);
            firstPlay = false;
        }

        ballC.IncreaseDropSpeed(16,22);//(20, 22);

        ballDropDelay = StartCoroutine(BallDropDelay());

        moveWallsC = StartCoroutine(MoveWalls());
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

    void AbsorbDoneAndRichochet()
    {
        gameModeManager.DoubleScored();

        startNextRound = StartCoroutine(StartNextRound());
    }

    void PlayerMissed()
    {
        gameModeManager.Missed();

        gameRunning = false;

        StopCoroutine(moveWallsC);
    }

    IEnumerator MoveWalls()
    {
        int num = Random.Range(2, 9);

        bool trigger = true;
        bool moveWallsNow = false;

        if (num % 2 == 0)
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
