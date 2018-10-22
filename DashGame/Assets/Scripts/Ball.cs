using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Color32 startColor; // start color should usually be this slightly grayish white : EAEAEA
    public Color32 boostColor;
    public GameObject ghost;
    public string firstImpactSound;
    public bool defaultSounds;

    SpriteRenderer ballSprite;
    ParticleSystem collisionEffect;
    ParticleSystem fallEffect;
    ParticleSystem firstImpact;
    ParticleSystem boostEffect;
    TrailRenderer hostTrail;
    Animator animator;
    GameManager game;
    AudioManager audioManager;
    Ray2D ray;
    Vector3 rayOffsetVector = new Vector3(0, 0.147f); // used to offset ray a bit so that it does not start from the enemy's transfrom.position which is also the contactpoint
    Rigidbody2D rigidbody;
    TargetController target;
    bool shouldBoost = false;
    bool invulnerable = false;
    bool wallHit = false;
    bool shouldAbsorb = false;
    bool canAbsorb; //ball will only be absorbed after it is deflectd off of the paddle;
    BallController ballC;
    bool firstCollision = true; //first collision with paddle
    float initialVelocity, boostVelocity, absorbSpeed;
    bool firstTriggerCollision = true; //for first collision with a target
    GameObject ghostBall1;
    GameObject ghostBall2;
    TrailRenderer ghost1Trail, ghost2Trail;
    Animator ghost1AnimC, ghost2AnimC;
    int targetHitIndex;
    bool wrappingEnabled = false;
    bool wrappingRightNow = false;
    float cameraRadius;
    float ballRadius = 0.147f; //got from measuring ball collider radius
    bool spawnedIn = false;
    TrailRenderer tempTrail, tempTrail2;
    bool wrappedAround = false;
    ParticleSystem.MainModule[] mainMods;
    bool noTrail = false;
    bool noFirstImpact = false;
    SpriteRenderer specialSprite;
    SpriteRenderer ghost1Sprite, ghost2Sprite;
    SpriteRenderer ghost1SpecialSprite, ghost2SpecialSprite;
    bool hasSpecialSprite = true;
    bool callPlayerMissed = false;
    bool isTargetHitMoving = false;
    float targetTravelSpeed;
    bool insideCollider = false;
    string collisionTag;
    int ballLayer = 11;
    int ignoreObstaclesLayer = 16;
    int ignoreEverythingLayer = 17;
    int ballButNoPaddleLayer = 19; //same as balllayer except wont collide with paddle
    Vector2 failSafeVelocity;
    string impactSound;
    bool noFISound = false;
    bool playingDeadeye = false;
    string origImpactSound;
    int index;
    int impactSoundNum, FISoundNum;

    public delegate void BallDelegate();
    public static event BallDelegate PlayerMissed;
    public static event BallDelegate AbsorbDone; // specific event for TargetController so its animation matches up with the balls
    public static event BallDelegate AbsorbDoneAndRichochet;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        mainMods = new ParticleSystem.MainModule[3];

        ballSprite = transform.Find("BallSprite").GetComponent<SpriteRenderer>();
        try
        {
            specialSprite = ballSprite.transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        catch (UnityException)
        {
            hasSpecialSprite = false;
        }
        collisionEffect = transform.Find("CollisionEffect").GetComponent<ParticleSystem>();
        mainMods[0] = collisionEffect.main;
        try
        {
            fallEffect = transform.Find("FallEffect").GetComponent<ParticleSystem>();
        }
        catch (System.NullReferenceException)
        {

        }
        try
        {
            firstImpact = transform.Find("FirstImpact").GetComponent<ParticleSystem>();
            mainMods[1] = firstImpact.main;
        }
        catch (System.NullReferenceException)
        {
            noFirstImpact = true;
        }
        try
        {
            hostTrail = transform.Find("Trail").GetComponent<TrailRenderer>();
        }
        catch (System.NullReferenceException)
        {
            noTrail = true;
        }
        try
        {
            mainMods[2] = transform.Find("BoostEffect").GetComponent<ParticleSystem>().main;
        }
        catch (System.NullReferenceException) { }

        ghostBall1 = Instantiate(ghost, Vector2.right * 600, Quaternion.Euler(0, 0, 0));
        ghost1Sprite = ghostBall1.transform.Find("BallSprite").GetComponent<SpriteRenderer>();
        if (hasSpecialSprite)
        {
            ghost1SpecialSprite = ghost1Sprite.transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        ghostBall1.SetActive(false);
        ghost1AnimC = ghostBall1.GetComponent<Animator>();
        if (!noTrail)
        {
            ghost1Trail = ghostBall1.transform.Find("Trail").GetComponent<TrailRenderer>();
        }

        ghostBall2 = Instantiate(ghost, Vector2.right * 600, Quaternion.Euler(0, 0, 0));
        ghost2Sprite = ghostBall2.transform.Find("BallSprite").GetComponent<SpriteRenderer>();
        if (hasSpecialSprite)
        {
            ghost2SpecialSprite = ghost2Sprite.transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        ghostBall2.SetActive(false);
        ghost2AnimC = ghostBall2.GetComponent<Animator>();
        if (!noTrail)
        {
            ghost2Trail = ghostBall2.transform.Find("Trail").GetComponent<TrailRenderer>();
        }

        rigidbody = GetComponent<Rigidbody2D>();

        if (Camera.main.aspect < 0.5625)
        {
            cameraRadius = 5.0f * (9.0f / 16.0f); //portrait aspect ratios smaller than 9:16 will have same game radius as 9:16 aspect ratios
        }
        else
        {
            cameraRadius = (Camera.main.aspect * Camera.main.orthographicSize);
        }

        impactSound = gameObject.name.Substring(0, gameObject.name.Length - 7);
        if(firstImpactSound.Length == 0)
        {
            noFISound = true;
        }

        if (defaultSounds)
        {
            impactSound = "YellowBall";
            firstImpactSound = "YellowBallFI";

            noFISound = false;
        }

        origImpactSound = impactSound;
    }

    private void Start()
    {
        game = GameManager.Instance;
        target = TargetController.Instance;
        ballC = BallController.Instance;
        audioManager = AudioManager.Instance;

        impactSoundNum = audioManager.BallImpactSound(impactSound);
        if (!noFISound)
        {
            FISoundNum = audioManager.BallFISound(firstImpactSound);
        }
    }

    private void OnEnable()
    {
        BallController.KillYourself += DieImmediately;
        BallController.ChangeSpeedImmediately += ChangeSpeedImmediately;

        rigidbody.simulated = true;
        SwitchSpriteColor(false);
        shouldAbsorb = false;
        invulnerable = false;
        shouldBoost = false;
        canAbsorb = false;
        firstCollision = true;
        firstTriggerCollision = true;
        gameObject.layer = ignoreObstaclesLayer;
        wrappingEnabled = false;
        insideCollider = false;

        SetAnimTrigs("Boost", true);
        SetAnimTrigs("ImmediateShrink", true);
    }

    private void OnDisable()
    {
        BallController.KillYourself -= DieImmediately;
        BallController.ChangeSpeedImmediately -= ChangeSpeedImmediately;
    }

    void SwitchSpriteColor(bool toBoostColor)
    {
        if (!toBoostColor)
        {
            ballSprite.color = startColor;
            ghost1Sprite.color = startColor;
            ghost2Sprite.color = startColor;
            if (hasSpecialSprite)
            {
                specialSprite.enabled = true;
                ghost1SpecialSprite.enabled = true;
                ghost2SpecialSprite.enabled = true;
            }
        }
        else
        {
            ballSprite.color = boostColor;
            ghost1Sprite.color = boostColor;
            ghost2Sprite.color = boostColor;

            if (hasSpecialSprite)
            {
                specialSprite.enabled = false;
                ghost1SpecialSprite.enabled = false;
                ghost2SpecialSprite.enabled = false;
            }
        }
    }

    public void Spawn(float initialVel, float boostVel, float absorbSpd, Vector2 position, Quaternion rotation, bool wrapping, bool deadeye = false)
    {
        transform.position = position;
        transform.rotation = rotation;

        initialVelocity = initialVel;
        boostVelocity = boostVel;
        absorbSpeed = absorbSpd;

        if (wrapping)
        {
            wrappingEnabled = true;

            ghostBall1.SetActive(true);
            ghostBall2.SetActive(true);
        }
        else
        {
            ghostBall1.SetActive(false);
            ghostBall2.SetActive(false);

        }

        playingDeadeye = deadeye;

        StartCoroutine(DropDelay());
    }

    void SetAnimTrigs(string trigger, bool reset = false)
    {
        if (!reset)
        {
            if (wrappingEnabled)
            {
                animator.SetTrigger(trigger);
                ghost1AnimC.SetTrigger(trigger);
                ghost2AnimC.SetTrigger(trigger);
            }
            else
            {
                animator.SetTrigger(trigger);
            }
        }
        else
        {
            if (wrappingEnabled)
            {
                animator.ResetTrigger(trigger);
                ghost1AnimC.ResetTrigger(trigger);
                ghost2AnimC.ResetTrigger(trigger);
            }
            else
            {
                animator.ResetTrigger(trigger);
            }
        }
    }

    void SetAnimBools(string boolName, bool value)
    {
        if (wrappingEnabled)
        {
            animator.SetBool(boolName, value);
            ghost1AnimC.SetBool(boolName, value);
            ghost2AnimC.SetBool(boolName, value);
        }
        else
        {
            animator.SetBool(boolName, value);
        }
    }

    void PositionGhosts()
    {
        if (rigidbody.velocity.y >= 0)
        {
            ghostBall1.transform.position = new Vector2(transform.position.x - cameraRadius * 2, transform.position.y + 1);  //ghost1 is always to the left

            ghostBall2.transform.position = new Vector2(transform.position.x + cameraRadius * 2, transform.position.y + 1); //ghost2 is always to the right
        }
        else
        {
            ghostBall1.transform.position = new Vector2(transform.position.x - cameraRadius * 2, transform.position.y - 1);  //ghost1 is always to the left

            ghostBall2.transform.position = new Vector2(transform.position.x + cameraRadius * 2, transform.position.y - 1); //ghost2 is always to the right
        }
    }

    void SwapWithGhost()
    {
        wrappedAround = true;

        if (OnScreen(ghostBall1.transform.position))
        {
            if (!noTrail)
            {
                hostTrail.transform.parent = null;
                ghost1Trail.transform.parent = null;
                ghost2Trail.transform.parent = null;
                ghost2Trail.Clear();
                ghost2Trail.gameObject.SetActive(false);
            }

            transform.position = ghostBall1.transform.position;
            if (rigidbody.velocity.y >= 0)
            {
                ghostBall1.transform.position = new Vector2(transform.position.x - cameraRadius * 2, transform.position.y + 1);  //ghost1 is always to the left

                ghostBall2.transform.position = new Vector2(transform.position.x + cameraRadius * 2, transform.position.y + 1); //ghost2 is always to the right
            }
            else
            {
                ghostBall1.transform.position = new Vector2(transform.position.x - cameraRadius * 2, transform.position.y - 1);  //ghost1 is always to the left

                ghostBall2.transform.position = new Vector2(transform.position.x + cameraRadius * 2, transform.position.y - 1); //ghost2 is always to the right
            }

            if (!noTrail)
            {
                tempTrail = hostTrail;
                tempTrail2 = ghost2Trail;

                ghost1Trail.transform.SetParent(transform, true);
                ghost1Trail.transform.localPosition = Vector2.zero;
                hostTrail = ghost1Trail;

                tempTrail.transform.SetParent(ghostBall2.transform, true);
                tempTrail.transform.localPosition = Vector2.zero;
                ghost2Trail = tempTrail;

                tempTrail2.transform.SetParent(ghostBall1.transform, false);
                tempTrail2.transform.localPosition = Vector2.zero;
                ghost1Trail = tempTrail2;
                ghost1Trail.transform.localPosition = Vector2.zero;
                ghost1Trail.gameObject.SetActive(true);
            }
        }
        else
        {
            if (!noTrail)
            {
                hostTrail.transform.parent = null;
                ghost1Trail.transform.parent = null;
                ghost2Trail.transform.parent = null;
                ghost1Trail.Clear();
                ghost1Trail.gameObject.SetActive(false);
            }

            transform.position = ghostBall2.transform.position;
            if (rigidbody.velocity.y >= 0)
            {
                ghostBall1.transform.position = new Vector2(transform.position.x - cameraRadius * 2, transform.position.y + 1);  //ghost1 is always to the left

                ghostBall2.transform.position = new Vector2(transform.position.x + cameraRadius * 2, transform.position.y + 1); //ghost2 is always to the right
            }
            else
            {
                ghostBall1.transform.position = new Vector2(transform.position.x - cameraRadius * 2, transform.position.y - 1);  //ghost1 is always to the left

                ghostBall2.transform.position = new Vector2(transform.position.x + cameraRadius * 2, transform.position.y - 1); //ghost2 is always to the right
            }

            if (!noTrail)
            {
                tempTrail = hostTrail;
                tempTrail2 = ghost1Trail;

                ghost2Trail.transform.SetParent(transform, true);
                ghost2Trail.transform.localPosition = Vector2.zero;
                hostTrail = ghost2Trail;

                tempTrail.transform.SetParent(ghostBall1.transform, true);
                tempTrail.transform.localPosition = Vector2.zero;
                ghost1Trail = tempTrail;

                tempTrail2.transform.SetParent(ghostBall2.transform, false);
                tempTrail2.transform.localPosition = Vector2.zero;
                ghost2Trail = tempTrail2;
                ghost2Trail.transform.localPosition = Vector2.zero;
                ghost2Trail.gameObject.SetActive(true);
            }
        }
    }

    bool OnScreen(Vector2 pos)
    {
        if (pos.x > -(cameraRadius + ballRadius) && pos.x < cameraRadius + ballRadius)
        {
            return true;
        }
        return false;
    }

    public void Wrap()
    {
        PositionGhosts();
        if (OnScreen(transform.position))
        {
            wrappingRightNow = false;
            return;
        }


        if (!wrappingRightNow)
        {
            wrappingRightNow = true;
        }
        SwapWithGhost();
    }

    IEnumerator DropDelay()
    {
        yield return new WaitForSeconds(1);
        spawnedIn = true;
        rigidbody.velocity = initialVelocity * Vector2.down;
    }

    private void Update()
    {
        SetAnimBools("ShouldShrink", shouldAbsorb);

        if (transform.position.y < -4.95f)
        {
            gameObject.layer = ignoreObstaclesLayer;
        }
    }

    private void FixedUpdate()
    {
        ray = new Ray2D(transform.position - rayOffsetVector, -transform.up);

        if (shouldAbsorb)
        {
            Absorb();
        }

        if (wrappingEnabled)
        {
            Wrap();
        }
    }

    public void KillParticles()
    {
        if (hostTrail != null)
        {
            hostTrail.Clear();
            ghost1Trail.Clear();
            ghost2Trail.Clear();
        }

        for (int i = 0; i < mainMods.Length; i++)
        {
            if (i == 2)
            {
                try
                {
                    mainMods[i].simulationSpeed *= 1.5f;
                }
                catch (System.NullReferenceException) { }
            }
            else
            {
                try
                {
                    mainMods[i].simulationSpeed = 1.5f;
                }
                catch (System.NullReferenceException) { }
            }
        }
    }

    public void DieImmediately()
    {
        gameObject.layer = ignoreEverythingLayer;
        GoAway();
    }

    public void GoAway()
    {
        for (int i = 0; i < mainMods.Length; i++)
        {
            if (i == 2)
            {
                try
                {
                    mainMods[i].simulationSpeed /= 1.5f;
                }
                catch (System.NullReferenceException) { }
            }
            else
            {
                try
                {
                    mainMods[i].simulationSpeed = 1;
                }
                catch (System.NullReferenceException) { }
            }
        }

        rigidbody.velocity = Vector2.zero;

        ghostBall1.SetActive(false);
        ghostBall2.SetActive(false);

        gameObject.SetActive(false);
    }

    void Absorb()
    {
        if (canAbsorb)
        {
            if (isTargetHitMoving)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.GetCurrentTargetPos(targetHitIndex), Time.deltaTime * (targetTravelSpeed + 1));
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target.GetCurrentTargetPos(targetHitIndex), Time.deltaTime * absorbSpeed);
            }

            if (ballSprite.transform.localScale.x <= 0.05f)//(transform.position == target.GetCurrentTargetPos(targetHitIndex))
            {
                shouldAbsorb = false;
                canAbsorb = false;
            }

            if (!shouldAbsorb)
            {
                if (wallHit || wrappedAround)
                {
                    wallHit = false;
                    wrappedAround = false;

                    ballC.ShrinkTarget(targetHitIndex); // make sure you call this before absorbdone
                    AbsorbDoneAndRichochet();
                    return;
                }
                else
                {
                    ballC.ShrinkTarget(targetHitIndex); // make sure you call this before absorbdone
                    AbsorbDone();
                    return;
                }
            }
        }
    }

    public void PlayerMissedBackup()
    {
        if (callPlayerMissed)
        {
            PlayerMissed();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject.name + ", current ball layer = " + gameObject.layer);
        collisionTag = collision.gameObject.tag;

        if (collisionTag == "Paddle")
        {
            canAbsorb = true;
            gameObject.layer = ballButNoPaddleLayer; // this makes it so that the paddle cant hit the ball again before it hits another collider
        }
        else
        {
            gameObject.layer = ballLayer;
        }

        if (collisionTag == "floor")
        {
            canAbsorb = true;
        }

        if (firstCollision)
        {
            FirstCollision();
            firstCollision = false;
        }
        else
        {
            audioManager.PlayBallSound(impactSoundNum);
        }

        if (collisionTag == "Wall")
        {
            wallHit = true;
        }

        if (!shouldAbsorb)
        {
            collisionEffect.Play();
        }

        ContactPoint2D cp = collision.contacts[0]; // 0 indicates the first contact point between the colliders. Since there is only one contact point a higher index would cause a runtime error
        Vector2 reflectDir = Vector2.Reflect(ray.direction, cp.normal);

        float rotation = 90 + Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        failSafeVelocity = -transform.up.normalized * boostVelocity;
        rigidbody.velocity = failSafeVelocity;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!shouldAbsorb)
        {
            if (gameObject.layer == ballButNoPaddleLayer)
            {
                rigidbody.velocity = failSafeVelocity;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!shouldAbsorb)
        {
            if (gameObject.layer == ballButNoPaddleLayer)
            {
                rigidbody.velocity = failSafeVelocity;
            }
        }
    }

    void FirstCollision()
    {
        if (noFISound)
        {
            audioManager.PlayBallSound(impactSoundNum);
        }
        else
        {
            audioManager.PlayBallFISound(FISoundNum);
        }

        SetAnimTrigs("Boost");

        ballC.FlashWhite();

        ballC.CameraShake();

        SwitchSpriteColor(true);
        shouldBoost = true;
        if (!noFirstImpact)
        {
            firstImpact.Play();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (canAbsorb)
        {
            if (firstTriggerCollision)
            {
                if (collision.gameObject.layer == 8)
                {
                    audioManager.PlayMiscSound("suckIn");

                    invulnerable = true;
                    rigidbody.velocity = Vector2.zero;
                    rigidbody.angularVelocity = 0;
                    shouldAbsorb = true;

                    targetHitIndex = int.Parse(collision.gameObject.name[collision.gameObject.name.Length - 1].ToString());
                    isTargetHitMoving = target.IsMoving(targetHitIndex);
                    targetTravelSpeed = target.getTravelSpeed(targetHitIndex);

                    firstTriggerCollision = false;
                    gameObject.layer = ignoreEverythingLayer;
                }
            }
        }
        if (collision.gameObject.layer == 9)
        {
            if (!invulnerable)
            {
                if (!shouldBoost)
                {
                    GoAway();
                }

                rigidbody.velocity = Vector2.zero;
                rigidbody.simulated = false;
                SetAnimTrigs("ImmediateShrink");
                PlayerMissed();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (canAbsorb)
        {
            if (!shouldAbsorb)
            {
                if (collision.gameObject.layer == 8)
                {
                    audioManager.PlayMiscSound("suckIn");

                    targetHitIndex = int.Parse(collision.gameObject.name[collision.gameObject.name.Length - 1].ToString());
                    isTargetHitMoving = target.IsMoving(targetHitIndex);
                    targetTravelSpeed = target.getTravelSpeed(targetHitIndex);

                    invulnerable = true;
                    rigidbody.velocity = Vector2.zero;
                    rigidbody.angularVelocity = 0;
                    shouldAbsorb = true;
                    firstTriggerCollision = false;
                    gameObject.layer = ignoreEverythingLayer;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!insideCollider)
        {
            insideCollider = true;
            if (canAbsorb)
            {
                if (!shouldAbsorb)
                {
                    if (collision.gameObject.layer == 8)
                    {
                        audioManager.PlayMiscSound("suckIn");

                        targetHitIndex = int.Parse(collision.gameObject.name[collision.gameObject.name.Length - 1].ToString());
                        isTargetHitMoving = target.IsMoving(targetHitIndex);
                        targetTravelSpeed = target.getTravelSpeed(targetHitIndex);

                        invulnerable = true;
                        rigidbody.velocity = Vector2.zero;
                        rigidbody.angularVelocity = 0;
                        shouldAbsorb = true;
                        firstTriggerCollision = false;
                        gameObject.layer = ignoreEverythingLayer;
                    }

                }
            }
        }
    }

    void ChangeSpeedImmediately(float tSpeed, float bSpeed)
    {
        if (!canAbsorb)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * tSpeed;
        }
        else
        {
            rigidbody.velocity = rigidbody.velocity.normalized * bSpeed;
        }
    }

    public void TurnGray()
    {
        if (!ballC.TurnGray()) //returns isGray as a bool
        {
            impactSound = "YellowBall";
        }
        else impactSound = origImpactSound;
    }

    public void PlayLaserSound()
    {
        if (playingDeadeye)
        {
            audioManager.PlayMiscSound("ballFired");
        }
    }
}
