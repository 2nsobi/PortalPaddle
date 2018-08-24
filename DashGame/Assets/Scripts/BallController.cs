using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public Vector2 startPos;
    public float speed;
    float codeSpeed;
    public float deflectionSpeed;
    Ray2D ray;
    Vector3 rayOffsetVector = new Vector3(0, 0.147f); // used to offset ray a bit so that it does not start from the enemy's transfrom.position which is also the contactpoint
    TargetController target;
    bool shouldAbsorb;
    public float absorbSpeed;
    Animator animator;
    bool canAbsorb; //ball will only be absorbed after it is deflectd off of the paddle;
    GameObject ballSpawner;
    Animator spawnerAnimator;
    Transform targetTransform;
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
    GameManager.BallPrefab activeBall = null;

    public static BallController Instance;

    public delegate void BallDelegate();
    public static event BallDelegate PlayerMissed;
    public static event BallDelegate AbsorbDone; // specific event for TargetController so its animation matches up with the balls
    public static event BallDelegate AbsorbDoneAndRichochet;

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
        shouldAbsorb = false;
        rigidbody = GetComponent<Rigidbody2D>();
        ballSpawner = GameObject.Find("BallSpawner");
        spawnerAnimator = ballSpawner.GetComponent<Animator>();
        codeSpeed = speed;
        atCenter = false;
        invulnerable = false;
        Vector3 vector = new Vector2(0, GetComponent<CircleCollider2D>().radius * this.transform.localScale.x + 10);
        ShouldShrink = false;
        ShouldSpawn = false;

        Physics2D.IgnoreLayerCollision(8, 10);
        Physics2D.IgnoreLayerCollision(8, 12);
        Physics2D.IgnoreLayerCollision(8, 13);
        Physics2D.IgnoreLayerCollision(8, 0);
        Physics2D.IgnoreLayerCollision(8, 9);
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
    }

    public void SetBall(GameManager.BallPrefab ball)
    {
        if (activeBall == null)
        {
            activeBall = ball;
            activeBall.prefab.transform.SetParent(transform,true);
            activeBall.prefab.gameObject.SetActive(true);

            ballSprite = activeBall.ballSprite.GetComponent<SpriteRenderer>();
            CollisionEffect = activeBall.collisionEffect.GetComponent<ParticleSystem>();
            FallEffect = activeBall.fallEffect.GetComponent<ParticleSystem>();
            FirstImpact = activeBall.firstImpact.GetComponent<ParticleSystem>();

            animator = activeBall.animator;
            originalColor = activeBall.startColor;
            boostColor = activeBall.boostColor;
        }
        else
        {
            activeBall.prefab.transform.parent = null;
            activeBall.prefab.SetActive(false);

            activeBall = ball;
            activeBall.prefab.transform.SetParent(transform, true);
            activeBall.prefab.gameObject.SetActive(true);

            ballSprite = activeBall.ballSprite.GetComponent<SpriteRenderer>();
            CollisionEffect = activeBall.collisionEffect.GetComponent<ParticleSystem>();
            FallEffect = activeBall.fallEffect.GetComponent<ParticleSystem>();
            FirstImpact = activeBall.firstImpact.GetComponent<ParticleSystem>();

            animator = activeBall.animator;
            originalColor = activeBall.startColor;
            boostColor = activeBall.boostColor;
        }
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

    private void OnEnable()
    {
        GameManager.GameOverConfirmed += GameOverConfirmed;
        GameManager.GameStarted += GameStarted;
        LevelGenerator.TransitionDone += TransitionDone;
        GameManager.Revive += Revive;
    }

    private void OnDisable()
    {
        GameManager.GameOverConfirmed -= GameOverConfirmed;
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
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rigidbody.velocity = -transform.up.normalized * codeSpeed;
    }

    private void Update()
    {
        //if (this.transform.localScale == Vector3.zero) //moves the ball each time it shrinks
        //{
        //    this.transform.position = Vector2.right * 1000;
        //}

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

    private void FixedUpdate()
    {
        if (game.IsGameRunning)
        {
            ray = new Ray2D(transform.position + rayOffsetVector, -transform.up);

            if (atCenter)
            {
                this.transform.position = target.GetCurrentTargetPos;
            }

            if (shouldAbsorb)
            {
                Absorb();
            }

            animator.SetBool("ShouldSpawn", ShouldSpawn);
            animator.SetBool("ShouldShrink", ShouldShrink);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!cantCollide)
        {
            if (collision.gameObject.tag == "Paddle")
            {
                canAbsorb = true;
                Physics2D.IgnoreLayerCollision(10, 11, false);
                Physics2D.IgnoreLayerCollision(11, 12);        // this makes it so that the paddle cant hit the ball again before it hits another collider
            }
            else
            {
                Physics2D.IgnoreLayerCollision(11, 12,false);
            }

            if (firstCollision)
            {
                if (!atCenter && !shouldAbsorb)
                {
                    StartCoroutine(CameraShake(CameraShakeIntensity, CameraShakeDuration));
                }
                FirstCollision();
                firstCollision = false;
            }

            if (collision.gameObject.tag == "Wall")
            {
                wallHit = true;
            }

            if (!atCenter && !shouldAbsorb)
            {
                CollisionEffect.Play();
            }

            ContactPoint2D cp = collision.contacts[0]; // 0 indicates the first contact point between the colliders. Since there is only one contact point a higher index would cause a runtime error
            Vector2 reflectDir = Vector2.Reflect(ray.direction, cp.normal);

            float rotation = 90 + Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            codeSpeed = deflectionSpeed;
            rigidbody.velocity = -transform.up.normalized * codeSpeed;

        }

    }

    void FirstCollision()
    {
        FlashWhite();

        CameraShake(CameraShakeIntensity, CameraShakeDuration);

        ballSprite.color = boostColor;
        FallEffect.Stop();
        FallEffect.Play();
        animator.SetTrigger("Boost");
        FirstImpact.Play();
    }

    void FlashWhite()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canAbsorb)
        {
            targetHit = collision.gameObject.name;
            if (firstTriggerCollision)
            {
                if (collision.gameObject.layer == 8)
                {
                    Physics2D.IgnoreLayerCollision(10, 11);
                    ShouldSpawn = false;
                    invulnerable = true;
                    rigidbody.velocity = Vector2.zero;
                    rigidbody.angularVelocity = 0;
                    shouldAbsorb = true;
                    targetTransform = collision.transform;
                    ShouldShrink = true;
                    firstTriggerCollision = false;
                    cantCollide = true;

                    Debug.Log("ball should definetely be absorbing right now since\n shouldAbsorb = " + shouldAbsorb + ", and ontriggerenter2d has been called");
                    Debug.Log("also the trigger the ball hit was " + collision.gameObject.name);
                    Debug.Log("velocity of ball = " + rigidbody.velocity);
                }
            }
        }
        if (collision.gameObject.layer == 9)
        {
            if (!invulnerable)
            {
                ShouldSpawn = false;
                rigidbody.velocity = Vector2.zero;
                rigidbody.simulated = false;
                PlayerMissed();
            }
        }
    }

    void Absorb()
    {
        if (canAbsorb)
        {
            if (target.IsMoving())
            {
                this.transform.position = Vector2.MoveTowards(this.transform.position, target.GetCurrentTargetPos, Time.deltaTime * (target.getTravelSpeed + 1));
            }
            else
            {
                this.transform.position = Vector2.MoveTowards(this.transform.position, target.GetCurrentTargetPos, Time.deltaTime * absorbSpeed);
            }

            if (this.transform.position == target.GetCurrentTargetPos)
            {
                atCenter = true;
                shouldAbsorb = false;
            }

            if (!shouldAbsorb)
            {
                if (wallHit)
                {
                    AbsorbDoneAndRichochet();
                    return;
                }
                else
                {
                    AbsorbDone();
                    return;
                }
            }
        }
    }

    void GameStarted()
    {
        spawnerAnimator.SetTrigger("GameStarted");
        animator.SetTrigger("GameStarted");

        this.transform.position = startPos;
        ballSpawner.transform.position = startPos;
        canAbsorb = false;
        StartCoroutine(SpawnDelay());
        wallHit = false;
        Physics2D.IgnoreLayerCollision(10, 11);
        Physics2D.IgnoreLayerCollision(11, 12, false);
        ShouldSpawn = true;
        ShouldShrink = false;
        firstCollision = true;
        firstTriggerCollision = true;
    }

    void GameOverConfirmed()
    {
        ballSprite.color = originalColor;
        this.transform.position = Vector2.right * 1000;
        this.rigidbody.velocity = Vector2.zero;
        codeSpeed = speed;

        animator.SetTrigger("GameOver");
        rigidbody.simulated = true;
    }

    void TransitionDone()
    {
        ballSprite.color = originalColor;
        ShouldSpawn = true;
        ShouldShrink = false;
        spawnerAnimator.SetTrigger("GameStarted");
        Physics2D.IgnoreLayerCollision(10, 11);
        atCenter = false;
        invulnerable = false;
        RandomXPos = new Vector2(target.RandomSpawnAreaXRange, startPos.y);
        rigidbody.velocity = Vector2.zero;
        ballSpawner.transform.position = RandomXPos;
        this.transform.position = RandomXPos;
        canAbsorb = false;
        wallHit = false;
        codeSpeed = speed;
        firstCollision = true;
        firstTriggerCollision = true;
        cantCollide = false;
        Physics2D.IgnoreLayerCollision(11, 12, false);

        StartCoroutine(SpawnDelay());
    }

    void Revive()
    {
        rigidbody.simulated = true;
        ballSprite.color = originalColor;
        animator.SetTrigger("ImmediateSpawn");
        spawnerAnimator.SetTrigger("GameStarted");
        ShouldSpawn = true;
        ShouldShrink = false;
        Physics2D.IgnoreLayerCollision(10, 11);
        atCenter = false;
        invulnerable = false;
        RandomXPos = new Vector2(target.RandomSpawnAreaXRange, startPos.y);
        rigidbody.velocity = Vector2.zero;
        ballSpawner.transform.position = RandomXPos;
        this.transform.position = RandomXPos;
        canAbsorb = false;
        wallHit = false;
        codeSpeed = speed;
        firstCollision = true;
        firstTriggerCollision = true;
        cantCollide = false;
        Physics2D.IgnoreLayerCollision(11, 12, false);

        StartCoroutine(SpawnDelay());
    }

    public string GetTargetHit
    {
        get
        {
            return targetHit;
        }
    }

    IEnumerator CameraShake(float intensity, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float percentComplete = elapsedTime / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= Mathf.PerlinNoise(x, y) * intensity * damper;
            y *= Mathf.PerlinNoise(x, y) * intensity * damper;

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
