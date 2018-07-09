using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {
    Rigidbody2D rigidbody;
    public Vector2 startPos;
    public float speed;
    float codeSpeed;
    public float deflectionSpeed;
    Ray2D ray;
    Vector3 vector; // used to offset ray a bit so that it does not start from the enemy's transfrom.position which is also the contactpoint
    TargetController target;
    bool shouldAbsord;
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
    ParticleSystem particleSystem;
    string targetHit; //Name of the target that was hit;
    bool ShouldSpawn;
    bool ShouldShrink;

    public static EnemyBehavior Instance;

    public delegate void BallDelegate();
    public static event BallDelegate PlayerMissed;
    public static event BallDelegate AbsorbDone; // specific event for TargetController so its animation matches up with the balls
    public static event BallDelegate AbsorbDoneAndRichochet;

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        shouldAbsord = false;
        rigidbody = GetComponent<Rigidbody2D>();
        ballSpawner = GameObject.Find("BallSpawner");
        spawnerAnimator = ballSpawner.GetComponent<Animator>();
        codeSpeed = speed;
        atCenter = false;
        invulnerable = false;
        particleSystem = GetComponentInChildren<ParticleSystem>();
        Vector3 vector = new Vector2(0, GetComponent<CircleCollider2D>().radius * this.transform.localScale.x + 10);
        ShouldShrink = false;
        ShouldSpawn = false;
    }

    private void Start()
    {
        //whenever you are retrieving a singleton of another class make sure it is after the singleton is creaeted in that class
        //So pretty much always create a singleton in awake and then retrieve it in start
        target = TargetController.Instance;
        wallHit = false;
        game = GameManager.Instance;
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
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rigidbody.velocity = -transform.up.normalized * codeSpeed;
    }

    private void Update()
    {
        ray = new Ray2D(transform.position + vector, -transform.up);

        if (canAbsorb)
        {
            Physics2D.IgnoreLayerCollision(10, 11,false);
        }

        if(this.transform.localScale == Vector3.zero) //moves the ball each time it shrinks
        {
            this.transform.position = Vector2.right * 1000;
        }

    }

    private void FixedUpdate()
    {
        if (atCenter)
        {
            this.transform.position = target.GetCurrentTargetPos;
        }

        if (shouldAbsord)
        {
            Absorb();
        }

        animator.SetBool("ShouldSpawn", ShouldSpawn);
        animator.SetBool("ShouldShrink", ShouldShrink);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Paddle")
        {
            canAbsorb = true;
            StartCoroutine("CollisionDelay");
        }
        if (collision.gameObject.tag == "Wall")
        {
            wallHit = true;
        }

        if (!atCenter && !shouldAbsord)
        {
            particleSystem.Play();
        }

        ContactPoint2D cp = collision.contacts[0]; // 0 indicates the first contact point between the colliders. Since there is only one contact point a higher index would cause a runtime error
        Vector2 reflectDir = Vector2.Reflect(ray.direction, cp.normal);

        float rotation = 90 + Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        codeSpeed = deflectionSpeed;
        rigidbody.velocity = -transform.up.normalized * codeSpeed;

    }

    //if the player is moving the paddle quickly this will prevent the ball from stopping mid motion due to collision detection failure
    //good practice for faulty 2D collision detection with fast moving objects
    IEnumerator CollisionDelay()
    {
        Physics2D.IgnoreLayerCollision(11, 12);
        yield return new WaitForSeconds(0.08f);
        Physics2D.IgnoreLayerCollision(11, 12, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canAbsorb)
        {
            targetHit = collision.gameObject.name;
            if (collision.gameObject.layer == 8)
            {
                ShouldSpawn = false;
                invulnerable = true;
                rigidbody.velocity = Vector2.zero;
                shouldAbsord = true;
                targetTransform = collision.transform;
                ShouldShrink = true;
            }
        }
        if (collision.gameObject.layer == 9)
        {
            if (!invulnerable)
            {
                ShouldSpawn = false;
                rigidbody.velocity = Vector2.zero;
                this.transform.position = Vector2.right * 1000;
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
                shouldAbsord = false;
            }
            if (!shouldAbsord)
            {
                if (wallHit)
                {
                    AbsorbDoneAndRichochet();
                    Debug.Log("Richichet +2 points. also shouldAbsorb = " + shouldAbsord);
                    return;
                }
                else
                {
                    AbsorbDone();
                    Debug.Log("Straight Hit +1 points. also shouldAbsorb = " + shouldAbsord);
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
        Physics2D.IgnoreLayerCollision(10,11);
        ShouldSpawn = true;
        ShouldShrink = false;
    }

    void GameOverConfirmed()
    {
        this.transform.position = Vector2.right * 1000;
        this.rigidbody.velocity = Vector2.zero;
        codeSpeed = speed;

        animator.SetTrigger("GameOver");
    }

    void TransitionDone()
    {
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
        StartCoroutine(SpawnDelay());
        wallHit = false;
        codeSpeed = speed;
    }

    void Revive()
    {
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
        StartCoroutine(SpawnDelay());
        wallHit = false;
        codeSpeed = speed;
    }

    public string GetTargetHit
    {
        get
        {
            return targetHit;
        }
    }
}
