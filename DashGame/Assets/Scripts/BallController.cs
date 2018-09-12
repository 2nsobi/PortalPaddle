using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public float absorbSpeed;
    public Vector2 startPos;
    public float initialSpeed;
    public float boostSpeed;
    public float CameraShakeIntensity;
    public float CameraShakeDuration;
    public GameObject[] ballPrefabs;

    TargetController targetC;
    GameObject ballSpawner;
    LevelGenerator LG;
    GameManager game;
    ObstacleSpawner obSpawner;
    OtherGameModesManager gameModeManager;

    float startSpeed;
    Animator spawnerAnimator;
    Vector2 RandomXPos;
    Camera mainCam;
    readonly Vector3 originalCamPos = new Vector3(0, 0, -50);
    public CanvasGroup whiteFlashCG;
    Image whiteFlashCGPanel;
    bool flash = false;
    bool fade2Black = false;
    bool fadeBack = false;
    bool pauseAllCoroutines = false;
    int selectedBallIndex;
    Queue<Ball> qOfBalls;
    Ball[] balls;
    float tempSpeed, tempBoostSpeed;
    bool isGray = false;
    Material grayScaleMat;
    OtherGameModesManager.gameMode currentGameMode;
    bool fade2GameMode = false;
    float offScreenSpawnHeight;
    float spawnHeight;
    Queue<Ball> ballsQ = new Queue<Ball>();

    public static BallController Instance;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;

        ballSpawner = GameObject.Find("BallSpawner");
        spawnerAnimator = ballSpawner.GetComponent<Animator>();
        DontDestroyOnLoad(ballSpawner);

        startSpeed = initialSpeed;

        balls = new Ball[ballPrefabs.Length];

        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            GameObject ballPref = Instantiate(ballPrefabs[i]);
            balls[i] = ballPref.GetComponent<Ball>();
            balls[i].gameObject.SetActive(false);

            if (i == 0)
            {
                grayScaleMat = balls[i].gameObject.GetComponentInChildren<SpriteRenderer>().sharedMaterial;
            }
        }
        grayScaleMat.SetFloat("_EffectAmount", 0);
    }

    private void Start()
    {
        //whenever you are retrieving a singleton of another class make sure it is after the singleton is creaeted in that class
        //So pretty much always create a singleton in awake and then retrieve it in start
        targetC = TargetController.Instance;
        game = GameManager.Instance;
        LG = LevelGenerator.Instance;
        obSpawner = ObstacleSpawner.Instance;
        gameModeManager = OtherGameModesManager.Instance;

        whiteFlashCGPanel = whiteFlashCG.GetComponentInChildren<Image>();

        selectedBallIndex = ZPlayerPrefs.GetInt("ballInUse");

        offScreenSpawnHeight = Camera.main.orthographicSize + 0.25f;
    }

    public int Link2BallItem(string name)
    {
        for (int i = 0; i < balls.Length; i++)
        {
            if ((name.Substring(0, name.Length - 1) + "(Clone)").Equals(balls[i].name))
            {
                return i;
            }
        }
        return 0;
    }

    public void IncreaseDropSpeed(float speed, float deflectSpeed) //original speed is 3, and original deflect speed is 16
    {
        startSpeed = speed;
        boostSpeed = deflectSpeed;
    }

    public void SetBall(int index)
    {
        balls[selectedBallIndex].gameObject.SetActive(false);

        selectedBallIndex = index;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            pauseAllCoroutines = true;
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            pauseAllCoroutines = true;
            ZPlayerPrefs.SetInt("ballInUse", selectedBallIndex);
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationQuit()
    {
        ZPlayerPrefs.SetInt("ballInUse", selectedBallIndex);
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        LevelGenerator.TransitionDone += TransitionDone;
        GameManager.Revive += Revive;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        LevelGenerator.TransitionDone -= TransitionDone;
        GameManager.Revive -= Revive;
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(1);
        while (pauseAllCoroutines || game.Paused)
        {
            yield return null;
        }

    }

    private void Update()
    {
        if (flash)
        {
            whiteFlashCG.alpha -= Time.deltaTime * 3;
            if (whiteFlashCG.alpha <= 0)
            {
                whiteFlashCG.alpha = 0;
                flash = false;
            }
        }

        if (fade2Black)
        {
            if (!fadeBack)
            {
                whiteFlashCG.alpha += Time.deltaTime * 4;
                if (whiteFlashCG.alpha >= 1)
                {
                    whiteFlashCG.alpha = 1;
                    LG.GoBack2StartLvl(); //sent to levelgenerator
                    targetC.ResetTargets(); //sent to targetcontroller
                    startSpeed = initialSpeed;
                    grayScaleMat.SetFloat("_EffectAmount", 0);
                    fadeBack = true;
                }
            }
            else
            {
                whiteFlashCG.alpha -= Time.deltaTime * 4;
                if (whiteFlashCG.alpha <= 0)
                {
                    whiteFlashCG.alpha = 0;
                    fade2Black = false;
                    fadeBack = false;
                }
            }
        }

        if (fade2GameMode)
        {
            if (!fadeBack)
            {
                whiteFlashCG.alpha += Time.deltaTime * 4;
                if (whiteFlashCG.alpha >= 1)
                {
                    whiteFlashCG.alpha = 1;
                    obSpawner.SetGameModeBackground();
                    gameModeManager.SetPageState(OtherGameModesManager.pageState.Game);
                    startSpeed = initialSpeed;
                    fadeBack = true;
                }
            }
            else
            {
                whiteFlashCG.alpha -= Time.deltaTime * 4;
                if (whiteFlashCG.alpha <= 0)
                {
                    whiteFlashCG.alpha = 0;
                    gameModeManager.StartOtherGameMode();
                    fade2GameMode = false;
                    fadeBack = false;
                }
            }
        }
    }

    public void FlashWhite()
    {
        whiteFlashCGPanel.color = Color.white;
        flash = true;
        whiteFlashCG.alpha = 1;
    }

    public void Fade2Black() //comes from gamemanager
    {
        whiteFlashCGPanel.color = Color.black;
        fade2Black = true;
        whiteFlashCG.alpha = 0;
    }

    void GameStarted()
    {
        isGray = false;

        tempSpeed = startSpeed;
        tempBoostSpeed = boostSpeed;
        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, startPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = startPos;
    }

    void TransitionDone()
    {
        RandomXPos = new Vector2(targetC.RandomSpawnAreaXRange, startPos.y);

        tempSpeed = startSpeed;
        tempBoostSpeed = boostSpeed;
        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
    }

    void Revive()
    {
        RandomXPos = new Vector2(targetC.RandomSpawnAreaXRange, startPos.y);

        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
    }

    public void CameraShake()
    {
        StartCoroutine(ShakeCamera());
    }

    public IEnumerator ShakeCamera()
    {
        float elapsedTime = 0;

        while (elapsedTime < CameraShakeDuration)
        {
            elapsedTime += Time.deltaTime;

            float percentComplete = elapsedTime / CameraShakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= Mathf.PerlinNoise(x, y) * CameraShakeIntensity * damper;
            y *= Mathf.PerlinNoise(x, y) * CameraShakeIntensity * damper;

            mainCam.transform.localPosition = new Vector3(x, y, originalCamPos.z);

            yield return null;
        }

        mainCam.transform.position = originalCamPos;
    }

    public void TurnGray()
    {
        isGray = !isGray;
        if (isGray)
        {
            grayScaleMat.SetFloat("_EffectAmount", 1);
        }
        else
        {
            grayScaleMat.SetFloat("_EffectAmount", 0);
        }
    }

    public bool SpawnQuickBall()
    {
        tempSpeed = startSpeed;
        tempBoostSpeed = boostSpeed;

        RandomXPos = new Vector2(targetC.RandomSpawnAreaXRange, spawnHeight);

        Ball ball2Spawn = ballsQ.Dequeue();

        while (ball2Spawn.isActiveAndEnabled)
        {
            ballsQ.Enqueue(ball2Spawn);

            for (int i = 0; i < ballsQ.Count; i++)
            {
                ball2Spawn = ballsQ.Dequeue();

                if (!ball2Spawn.isActiveAndEnabled)
                {
                    ball2Spawn.gameObject.SetActive(true);
                    ball2Spawn.Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

                    ballsQ.Enqueue(ball2Spawn);

                    return true;
                }
            }
            ballsQ.Enqueue(ball2Spawn);
            return false;
        }
        ball2Spawn.gameObject.SetActive(true);
        ball2Spawn.Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        ballsQ.Enqueue(ball2Spawn);

        return true;
    }

    public void SetGameMode(OtherGameModesManager.gameMode gameMode)
    {
        switch (gameMode)
        {
            case OtherGameModesManager.gameMode.PlusOne:
                currentGameMode = OtherGameModesManager.gameMode.PlusOne;

                spawnHeight = offScreenSpawnHeight;
                break;

            case OtherGameModesManager.gameMode.Deadeye:
                currentGameMode = OtherGameModesManager.gameMode.Deadeye;

                spawnHeight = offScreenSpawnHeight;
                break;

            case OtherGameModesManager.gameMode.Clairvoyance:
                currentGameMode = OtherGameModesManager.gameMode.Clairvoyance;

                spawnHeight = startPos.y;
                break;

            case OtherGameModesManager.gameMode.None:
                currentGameMode = OtherGameModesManager.gameMode.None;

                spawnHeight = startPos.y;
                break;
        }
    }

    public void Fade2GameMode()
    {
        whiteFlashCGPanel.color = Color.black;
        whiteFlashCG.alpha = 0;
        fade2GameMode = true;
    }

    public void CreateQOfBalls()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject go = Instantiate(ballPrefabs[selectedBallIndex]);

            go.SetActive(false);

            ballsQ.Enqueue(go.GetComponent<Ball>());
        }
    }
}
