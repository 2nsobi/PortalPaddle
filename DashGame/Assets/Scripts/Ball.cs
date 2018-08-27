using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public Color32 startColor; // start color should usually be this slightly grayish white : EAEAEA
    public Color32 boostColor;

    SpriteRenderer ballSprite;
    ParticleSystem collisionEffect;
    ParticleSystem fallEffect;
    ParticleSystem firstImpact;
    ParticleSystem boostEffect;
    Animator animator;
    GameManager game;
    Ray2D ray;
    Vector3 rayOffsetVector = new Vector3(0, 0.147f); // used to offset ray a bit so that it does not start from the enemy's transfrom.position which is also the contactpoint
    bool atCenter = false;
    Rigidbody2D rigidbody;
    TargetController target;
    bool shouldBoost = false;
    bool ShouldShrink = false;
    bool invulnerable = false;
    bool wallHit = false;
    bool cantCollide = false;
    bool shouldAbsorb = false;
    bool canAbsorb; //ball will only be absorbed after it is deflectd off of the paddle;
    BallController ballC;
    bool firstCollision = true; //first collision with paddle
    float initialVelocity, boostVelocity, absorbSpeed;
    bool firstTriggerCollision = true; //for first collision with a target
    GameObject ghostBall1;
    GameObject ghostBall2;
    string targetHit; //Name of the target that was hit;
    bool isWrapping = false;
    bool wrappingRightNow = false;
    bool pauseAllCoroutines = false;
    bool goAway = false;
    float cameraRadius;
    float ballWidth = 0.147f; //got from measuring ball collider radius
    Vector2 newPos;

    public delegate void BallDelegate();
    public static event BallDelegate PlayerMissed;
    public static event BallDelegate AbsorbDone; // specific event for TargetController so its animation matches up with the balls
    public static event BallDelegate AbsorbDoneAndRichochet;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        ballSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        collisionEffect = transform.GetChild(1).GetComponent<ParticleSystem>();
        fallEffect = transform.GetChild(2).GetComponent<ParticleSystem>();
        firstImpact = transform.GetChild(3).GetComponent<ParticleSystem>();

        ghostBall1 = transform.Find("GhostBall1").gameObject;
        ghostBall2 = transform.Find("GhostBall2").gameObject;

        rigidbody = GetComponent<Rigidbody2D>();

        game = GameManager.Instance;
        target = TargetController.Instance;
        ballC = BallController.Instance;

        cameraRadius = game.GetCameraRadius();
    }

    public void Spawn(float initialVel, float boostVel, float absorbSpd, Vector2 position, Quaternion rotation, bool wrapping)
    {
        transform.position = position;
        transform.rotation = rotation;

        initialVelocity = initialVel;
        boostVelocity = boostVel;
        absorbSpeed = absorbSpd;

        if (wrapping)
        {
            isWrapping = true;

            ghostBall1.SetActive(true);
            ghostBall2.SetActive(true);

            PositionGhosts();
        }
        else
        {
            ghostBall1.SetActive(false);
            ghostBall2.SetActive(false);
        }

        animator.SetTrigger("GameStarted");
        StartCoroutine(DropDelay());
    }

    void PositionGhosts()
    {
        ghostBall1.transform.localPosition = new Vector2(cameraRadius*2 / transform.localScale.x, 0);
        ghostBall1.transform.rotation = this.transform.rotation;

        ghostBall2.transform.localPosition = new Vector2(-cameraRadius*2 / transform.localScale.x, 0);
        ghostBall2.transform.rotation = this.transform.rotation;
    }

    void SwapWithGhost()
    {
        if (OnScreen(ghostBall1.transform.position))
        {
            transform.position = ghostBall1.transform.position;
        }
        else
        {
            transform.position = ghostBall2.transform.position;
        }
    }

    bool OnScreen(Vector2 pos)
    {
        if(pos.x > -cameraRadius - ballWidth || pos.x < cameraRadius + ballWidth)
        {
            return true;
        }
        return false;
    }

    public void Wrap()
    {
        if (OnScreen(transform.position))
        {
            wrappingRightNow = false;
            return;
        }

        newPos = transform.position; 

        if(!wrappingRightNow)
        {
            newPos = -newPos;

            wrappingRightNow = true;
        }

        SwapWithGhost();
    }

    IEnumerator DropDelay()
    {
        yield return new WaitForSeconds(1);
        while (pauseAllCoroutines || game.Paused)
        {
            yield return null;
        }
        rigidbody.velocity = initialVelocity * Vector2.down;
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
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnEnable()
    {
        rigidbody.simulated = true;
        ballSprite.color = startColor;
        ShouldShrink = false;
        Physics2D.IgnoreLayerCollision(10, 11);
        Physics2D.IgnoreLayerCollision(11, 12, false);
        atCenter = false;
        invulnerable = false;
        shouldBoost = false;
        canAbsorb = false;
        wallHit = false;
        firstCollision = true;
        firstTriggerCollision = true;
        cantCollide = false;
    }

    private void OnDisable()
    {
        rigidbody.velocity = Vector2.zero;
    }

    private void Update()
    {
        animator.SetBool("ShouldBoost", shouldBoost);
        animator.SetBool("ShouldShrink", ShouldShrink);
    }

    private void FixedUpdate()
    {
        if (game.IsGameRunning)
        {
            ray = new Ray2D(transform.position + rayOffsetVector, -transform.up);

            if (atCenter)
            {
                transform.position = target.GetCurrentTargetPos;
            }

            if (shouldAbsorb)
            {
                Absorb();
            }

            if(ballSprite.transform.localScale == Vector3.zero)
            {
                if (ShouldShrink)
                {
                    GoAway();
                }
            }

            if (isWrapping)
            {
                Wrap();
            }
        }
    }

    public void GoAway()
    {
        this.gameObject.SetActive(false);
    }

    void Absorb()
    {
        if (canAbsorb)
        {
            if (target.IsMoving())
            {
                transform.position = Vector2.MoveTowards(transform.position, target.GetCurrentTargetPos, Time.deltaTime * (target.getTravelSpeed + 1));
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target.GetCurrentTargetPos, Time.deltaTime * absorbSpeed);
            }

            if (transform.position == target.GetCurrentTargetPos)
            {
                atCenter = true;
                shouldAbsorb = false;
            }

            if (!shouldAbsorb)
            {
                if (wallHit)
                {
                    ballC.SetTargetHit(targetHit);
                    AbsorbDoneAndRichochet();
                    return;
                }
                else
                {
                    ballC.SetTargetHit(targetHit);
                    AbsorbDone();
                    return;
                }
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject.name);
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
                Physics2D.IgnoreLayerCollision(11, 12, false);
            }

            if (firstCollision)
            {
                FirstCollision();
                firstCollision = false;
            }

            if (collision.gameObject.tag == "Wall")
            {
                wallHit = true;
            }

            if (!atCenter && !shouldAbsorb)
            {
                collisionEffect.Play();
            }

            ContactPoint2D cp = collision.contacts[0]; // 0 indicates the first contact point between the colliders. Since there is only one contact point a higher index would cause a runtime error
            Vector2 reflectDir = Vector2.Reflect(ray.direction, cp.normal);

            float rotation = 90 + Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            rigidbody.velocity = -transform.up.normalized * boostVelocity;
        }
    }

    void FirstCollision()
    {
        ballC.FlashWhite();

        ballC.CameraShake();

        ballSprite.color = boostColor;
        fallEffect.Stop();
        fallEffect.Play();
        shouldBoost = true;
        firstImpact.Play();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (canAbsorb)
        {
            targetHit = collision.gameObject.name;
            if (firstTriggerCollision)
            {
                if (collision.gameObject.layer == 8)
                {
                    Physics2D.IgnoreLayerCollision(10, 11);
                    invulnerable = true;
                    rigidbody.velocity = Vector2.zero;
                    rigidbody.angularVelocity = 0;
                    shouldAbsorb = true;
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
                rigidbody.velocity = Vector2.zero;
                rigidbody.simulated = false;
                PlayerMissed();
            }
        }
    }
}
