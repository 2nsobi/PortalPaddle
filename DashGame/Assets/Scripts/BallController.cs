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
    public Animator tutorialToggleAnimC;

    TargetController targetC;
    GameObject ballSpawner;
    LevelGenerator LG;
    GameManager game;
    ObstacleSpawner obSpawner;
    OtherGameModesManager gameModeManager;
    AudioManager audioManager;

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
    Ball[] balls;
    float tempSpeed, tempBoostSpeed;
    bool isGray = false;
    Material grayScaleMat;
    int currentGameMode = 0;
    bool fade2GameMode = false;
    float offScreenSpawnHeight;
    float spawnHeight;
    Queue<Ball> ballsQ = new Queue<Ball>();
    int balls2Absorb = 0;
    bool playedOnce = false;
    OtherGameModesManager.pageState currentPage2Fade2;
    OtherGameModesManager.gameMode currentGameMode2Start;
    bool dontStartGameMode = false;
    bool noSound;
    bool tutorialDisabled;

    List<float> times = new List<float>();
    float startTime;

    public static BallController Instance;

    public delegate void BallCDelegate();
    public static event BallCDelegate KillYourself;

    public delegate void BallCDelegate2(float tSpeed, float bSpeed);
    public static event BallCDelegate2 ChangeSpeedImmediately;

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

        tutorialDisabled = PlayerPrefsX.GetBool("tutorialDisabled");
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
        audioManager = AudioManager.Instance;

        whiteFlashCGPanel = whiteFlashCG.GetComponentInChildren<Image>();

        selectedBallIndex = ZPlayerPrefs.GetInt("ballInUse");

        offScreenSpawnHeight = Camera.main.orthographicSize + 0.25f;

        noSound = PlayerPrefsX.GetBool("noSound");
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

    public void IncreaseDropSpeedImmediately(float speed, float deflectSpeed) //original speed is 3, and original deflect speed is 16
    {
        try
        {
            ChangeSpeedImmediately(speed, deflectSpeed);
        }
        catch (System.NullReferenceException) { }
    }

    public void SetBall(int index)
    {
        balls[selectedBallIndex].gameObject.SetActive(false);

        selectedBallIndex = index;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            pauseAllCoroutines = true;
            ZPlayerPrefs.SetInt("ballInUse", selectedBallIndex);
            PlayerPrefsX.SetBool("tutorialDisabled", tutorialDisabled);
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationQuit()
    {
        ZPlayerPrefs.SetInt("ballInUse", selectedBallIndex);
        PlayerPrefsX.SetBool("tutorialDisabled", tutorialDisabled);
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        LevelGenerator.TransitionDone += TransitionDone;
        GameManager.Revive += Revive;
        Tutorial.NowStartGame += NowStartGame;
    }

    private void OnDisable()
    {
        ZPlayerPrefs.SetInt("ballInUse", selectedBallIndex);
        PlayerPrefsX.SetBool("tutorialDisabled", tutorialDisabled);

        GameManager.GameStarted -= GameStarted;
        LevelGenerator.TransitionDone -= TransitionDone;
        GameManager.Revive -= Revive;
        Tutorial.NowStartGame -= NowStartGame;
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
                if (!noSound)
                {
                    AudioListener.volume -= Time.deltaTime * 4;

                    if (whiteFlashCG.alpha >= 1 && AudioListener.volume <= 0)
                    {
                        whiteFlashCG.alpha = 1;
                        AudioListener.volume = 0;

                        targetC.ResetTargets(); //sent to targetcontroller // make sure to reset targets before calling goback2startlvl for LG so that targets aren't still in use while parented to an inactive obstacle
                        LG.GoBack2StartLvl(); //sent to levelgenerator

                        startSpeed = initialSpeed;
                        grayScaleMat.SetFloat("_EffectAmount", 0);
                        isGray = false;

                        fadeBack = true;
                    }
                }
                else
                {
                    if (whiteFlashCG.alpha >= 1)
                    {
                        whiteFlashCG.alpha = 1;

                        targetC.ResetTargets(); //sent to targetcontroller // make sure to reset targets before calling goback2startlvl for LG so that targets aren't still in use while parented to an inactive obstacle
                        LG.GoBack2StartLvl(); //sent to levelgenerator

                        startSpeed = initialSpeed;
                        grayScaleMat.SetFloat("_EffectAmount", 0);
                        isGray = false;

                        fadeBack = true;
                    }
                }
            }
            else
            {
                whiteFlashCG.alpha -= Time.deltaTime * 4;
                if (!noSound)
                {
                    AudioListener.volume += Time.deltaTime * 4;

                    if (whiteFlashCG.alpha <= 0 && AudioListener.volume >= 1)
                    {
                        whiteFlashCG.alpha = 0;
                        AudioListener.volume = 1;

                        fade2Black = false;
                        fadeBack = false;
                    }
                }
                else
                {
                    if (whiteFlashCG.alpha <= 0)
                    {
                        whiteFlashCG.alpha = 0;
                        fade2Black = false;
                        fadeBack = false;
                    }
                }
            }
        }

        if (fade2GameMode)
        {
            if (!fadeBack)
            {
                whiteFlashCG.alpha += Time.deltaTime * 4;
                if (!noSound)
                {
                    if (dontStartGameMode)
                    {
                        audioManager.LetCurrentMusicIgnoreFadeOut(true);
                    }
                    else
                    {
                        audioManager.LetCurrentMusicIgnoreFadeOut(false);
                    }

                    AudioListener.volume -= Time.deltaTime * 4;

                    if (whiteFlashCG.alpha >= 1 && AudioListener.volume <= 0)
                    {
                        whiteFlashCG.alpha = 1;

                        AudioListener.volume = 0;

                        if (!dontStartGameMode)
                        {
                            gameModeManager.SetGameMode(currentGameMode2Start); //make sure to set the game mode before setting any pagestates or backgrounds
                            audioManager.StopOGGMusicRadio();
                        }

                        obSpawner.SetGameModeBackground();
                        gameModeManager.SetPageState(currentPage2Fade2);

                        startSpeed = initialSpeed;
                        targetC.ResetTargets(); // make sure to reset targets before calling endgame for obspawner so that targets aren't still in use while parented to an inactive obstacle
                        obSpawner.EndGame();

                        grayScaleMat.SetFloat("_EffectAmount", 0);
                        obSpawner.InvertDeadeyeBackground(true);
                        isGray = false;

                        if (currentGameMode == 1)
                        {
                            if (playedOnce)
                            {
                                DisableBallsInQ();
                            }
                            playedOnce = true;
                        }

                        if (currentGameMode == 0)
                        {
                            audioManager.StopMusic();
                        }

                        fadeBack = true;
                    }
                }
                else
                {
                    if (whiteFlashCG.alpha >= 1)
                    {
                        whiteFlashCG.alpha = 1;

                        if (!dontStartGameMode)
                        {
                            gameModeManager.SetGameMode(currentGameMode2Start); //make sure to set the game mode before setting any pagestates or backgrounds
                        }
                        obSpawner.SetGameModeBackground();
                        gameModeManager.SetPageState(currentPage2Fade2);

                        startSpeed = initialSpeed;
                        targetC.ResetTargets(); // make sure to reset targets before calling endgame for obspawner so that targets aren't still in use while parented to an inactive obstacle
                        obSpawner.EndGame();

                        grayScaleMat.SetFloat("_EffectAmount", 0);
                        obSpawner.InvertDeadeyeBackground(true);
                        isGray = false;

                        if (currentGameMode == 1)
                        {
                            if (playedOnce)
                            {
                                DisableBallsInQ();
                            }
                            playedOnce = true;
                        }

                        fadeBack = true;
                    }
                }
            }
            else
            {
                whiteFlashCG.alpha -= Time.deltaTime * 4;
                if (!noSound)
                {
                    AudioListener.volume += Time.deltaTime * 4;

                    if (whiteFlashCG.alpha <= 0 && AudioListener.volume >= 1)
                    {
                        whiteFlashCG.alpha = 0;

                        AudioListener.volume = 1;

                        gameModeManager.StartGameMode();
                        fade2GameMode = false;
                        fadeBack = false;

                        if (dontStartGameMode)
                        {
                            audioManager.LetCurrentMusicIgnoreFadeOut(false);
                        }
                    }
                }
                else
                {
                    if (whiteFlashCG.alpha <= 0)
                    {
                        whiteFlashCG.alpha = 0;
                        gameModeManager.StartGameMode();
                        fade2GameMode = false;
                        fadeBack = false;
                    }
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

        times.Clear();
    }

    IEnumerator BallSpawnerSounds()
    {
        audioManager.PlayMiscSound("portalSpawn");
        yield return new WaitForSeconds(1.45f); //it takes about 1.4 seconds for the ballspawner portal to start shrinking
        audioManager.PlayMiscSound("portalShrink");
    }

    void GameStarted()
    {
        isGray = false;

        tempSpeed = startSpeed;
        tempBoostSpeed = boostSpeed;

        if (tutorialDisabled)
        {
            balls[selectedBallIndex].gameObject.SetActive(true);
            balls[selectedBallIndex].Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, startPos, Quaternion.Euler(0, 0, 0), true);

            spawnerAnimator.SetTrigger("GameStarted");
            ballSpawner.transform.position = startPos;
            StartCoroutine(BallSpawnerSounds());
        }
        else
        {
            game.EnableTutorial();
        }

        startTime = Time.time;
        print(startTime);
    }

    void NowStartGame()
    {
        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, startPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = startPos;
        StartCoroutine(BallSpawnerSounds());
    }

    void TransitionDone()
    {
        print("current time: + " + Time.time);
        times.Add(Time.time - startTime);

        startTime = Time.time;

        print("time it took for that lvl: " + times[times.Count - 1]);
        float sum = 0;
        foreach (float t in times)
        {
            sum += t;
        }
        print("average time: " + (sum / times.Count));

        //-----------------------------------------------------

        RandomXPos = new Vector2(targetC.RandomSpawnAreaXRange, startPos.y);

        tempSpeed = startSpeed;
        tempBoostSpeed = boostSpeed;
        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
        StartCoroutine(BallSpawnerSounds());
    }

    void Revive()
    {
        RandomXPos = new Vector2(targetC.RandomSpawnAreaXRange, startPos.y);

        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
        StartCoroutine(BallSpawnerSounds());
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

    public bool TurnGray()
    {
        isGray = !isGray;
        if (isGray)
        {
            grayScaleMat.SetFloat("_EffectAmount", 1);
            if (obSpawner)
            {
                obSpawner.InvertDeadeyeBackground();
            }
        }
        else
        {
            grayScaleMat.SetFloat("_EffectAmount", 0);
            if (obSpawner)
            {
                obSpawner.InvertDeadeyeBackground(true);
            }
        }

        return isGray;
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
                    ball2Spawn.Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true, currentGameMode == 2);

                    ballsQ.Enqueue(ball2Spawn);

                    return true;
                }
            }
            ballsQ.Enqueue(ball2Spawn);
            return false;
        }
        ball2Spawn.gameObject.SetActive(true);
        ball2Spawn.Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true, currentGameMode == 2);

        ballsQ.Enqueue(ball2Spawn);

        return true;
    }

    public bool SpawnQuickBallWithBallSpawner()
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

                    spawnerAnimator.SetTrigger("GameStarted");
                    ballSpawner.transform.position = startPos;
                    StartCoroutine(BallSpawnerSounds());

                    ballsQ.Enqueue(ball2Spawn);

                    return true;
                }
            }
            ballsQ.Enqueue(ball2Spawn);
            return false;
        }
        ball2Spawn.gameObject.SetActive(true);
        ball2Spawn.Spawn(tempSpeed, tempBoostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
        StartCoroutine(BallSpawnerSounds());

        ballsQ.Enqueue(ball2Spawn);

        return true;
    }

    public void SetGameMode(OtherGameModesManager.gameMode gameMode)
    {
        switch (gameMode)
        {
            case OtherGameModesManager.gameMode.PlusOne:
                currentGameMode = 1;

                playedOnce = false;
                spawnHeight = offScreenSpawnHeight;
                break;

            case OtherGameModesManager.gameMode.Deadeye:
                currentGameMode = 2;

                playedOnce = false;
                spawnHeight = offScreenSpawnHeight;
                break;

            case OtherGameModesManager.gameMode.Clairvoyance:
                currentGameMode = 3;

                playedOnce = false;
                spawnHeight = startPos.y;
                break;

            case OtherGameModesManager.gameMode.None:
                currentGameMode = 0;

                playedOnce = false;
                spawnHeight = startPos.y;
                break;
        }
    }

    public void Fade2GameMode(OtherGameModesManager.pageState page2Fade2, OtherGameModesManager.gameMode gameMode2Start, bool dontStartGM = false)
    {
        currentPage2Fade2 = page2Fade2;
        currentGameMode2Start = gameMode2Start;

        dontStartGameMode = dontStartGM;

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

    public void DisableBallsInQ()
    {
        try
        {
            KillYourself();
        }
        catch (System.NullReferenceException) { }
    }

    public void ShrinkTarget(int targetIndex)
    {
        if (currentGameMode == 1)
        {
            if (balls2Absorb > 1)
            {
                balls2Absorb--;
            }
            else
            {
                targetC.ShrinkTarget2(targetIndex); // ShrinkTarget2 used for other game modes to make transitions less weird
            }
        }
        else
        {
            targetC.ShrinkTarget(targetIndex);
        }
    }

    public void SetBalls2Absorb(int balls)
    {
        balls2Absorb = balls;
    }

    public void SetNoSound(bool noSound)
    {
        this.noSound = noSound;
    }

    public void SetTutorial(bool turnOff)
    {
        if (turnOff)
        {
            tutorialDisabled = true;
        }
        else
        {
            tutorialDisabled = false;
        }
    }

    public void ToggleTutorial()
    {
        audioManager.PlayUISound("switchPageLouder");

        tutorialDisabled = !tutorialDisabled;
        if (tutorialDisabled)
        {
            tutorialToggleAnimC.SetTrigger("turnOff");
        }
        else
        {
            tutorialToggleAnimC.SetTrigger("turnOn");
        }
    }

    public void SetTutorialToggle()
    {
        if (tutorialDisabled)
        {
            tutorialToggleAnimC.SetTrigger("off");
        }
    }
}
