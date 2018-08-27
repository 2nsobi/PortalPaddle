using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public Vector2 startPos;
    public float initialSpeed;
    public float boostSpeed;
    Ray2D ray;
    Vector3 rayOffsetVector = new Vector3(0, 0.147f); // used to offset ray a bit so that it does not start from the enemy's transfrom.position which is also the contactpoint
    TargetController target;
    bool shouldAbsorb;
    public float absorbSpeed;
    Animator animator;
    bool canAbsorb; //ball will only be absorbed after it is deflectd off of the paddle;
    GameObject ballSpawner;
    Animator spawnerAnimator;
    bool wallHit;
    Vector2 RandomXPos;
    GameManager game;
    bool atCenter;
    bool invulnerable;
    ParticleSystem CollisionEffect;
    ParticleSystem FallEffect;
    ParticleSystem FirstImpact;
    string targetHit; //Name of the target that was hit;
    bool ShouldSpawn;
    bool ShouldShrink;
    Camera mainCam;
    readonly Vector3 originalCamPos = new Vector3(0, 0, -50);
    public float CameraShakeIntensity;
    public float CameraShakeDuration;
    bool firstCollision; //first collision with paddle
    bool firstTriggerCollision; //for first collision with a target
    LevelGenerator LG;
    SpriteRenderer ballSprite;
    Color originalColor;
    Color boostColor;
    public float rotationSpeed;
    public CanvasGroup whiteFlashCG;
    Image whiteFlashCGPanel;
    bool flash = false;
    bool fade2Black = false;
    bool fadeBack = false;
    bool cantCollide = false;
    bool pauseAllCoroutines = false;
    int selectedBallIndex;
    public GameObject[] ballPrefabs;
    Ball[] balls;

    public static BallController Instance;

    public delegate void BallDelegate();
    public static event BallDelegate PlayerMissed;
    public static event BallDelegate AbsorbDone; // specific event for TargetController so its animation matches up with the balls
    public static event BallDelegate AbsorbDoneAndRichochet;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
        ballSpawner = GameObject.Find("BallSpawner");
        spawnerAnimator = ballSpawner.GetComponent<Animator>();

        Physics2D.IgnoreLayerCollision(8, 10);
        Physics2D.IgnoreLayerCollision(8, 12);
        Physics2D.IgnoreLayerCollision(8, 13);
        Physics2D.IgnoreLayerCollision(8, 0);
        Physics2D.IgnoreLayerCollision(8, 9);

        balls = new Ball[ballPrefabs.Length];

        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            GameObject ballPref = Instantiate(ballPrefabs[i]);
            balls[i] = ballPref.GetComponent<Ball>();
            balls[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        //whenever you are retrieving a singleton of another class make sure it is after the singleton is creaeted in that class
        //So pretty much always create a singleton in awake and then retrieve it in start
        target = TargetController.Instance;
        wallHit = false;
        game = GameManager.Instance;
        LG = LevelGenerator.Instance;

        whiteFlashCGPanel = whiteFlashCG.GetComponentInChildren<Image>();

        selectedBallIndex = ZPlayerPrefs.GetInt("ballInUse");
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

    public void SetBall(int index)
    {
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
                    target.ResetTargets(); //sent to targetcontroller
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
        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(initialSpeed, boostSpeed, absorbSpeed, startPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = startPos;
    }

    void TransitionDone()
    {
        RandomXPos = new Vector2(target.RandomSpawnAreaXRange, startPos.y);

        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(initialSpeed, boostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
    }

    void Revive()
    {
        RandomXPos = new Vector2(target.RandomSpawnAreaXRange, startPos.y);

        balls[selectedBallIndex].gameObject.SetActive(true);
        balls[selectedBallIndex].Spawn(initialSpeed, boostSpeed, absorbSpeed, RandomXPos, Quaternion.Euler(0, 0, 0), true);

        spawnerAnimator.SetTrigger("GameStarted");
        ballSpawner.transform.position = RandomXPos;
    }

    public void SetTargetHit(string target)
    {
        targetHit = target;
    }

    public string GetTargetHit
    {
        get
        {
            return targetHit;
        }
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

            while (pauseAllCoroutines || game.Paused)
            {
                yield return null;
            }

            yield return null;
        }

        mainCam.transform.position = originalCamPos;
    }

}
