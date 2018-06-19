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
    Vector3 vector = new Vector2(0, 2); // used to offset ray a bit so that it does not start from the enemy's transfrom.position which is also the contactpoint
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
    }

    private void Start()
    {
        //whenever you are retrieving a singleton of another class make sure it is after the singleton is creaeted in that class
        // So pretty much always create a singleton in awake and then retrieve it in start
        target = TargetController.Instance;
        wallHit = false;
    }

    private void OnEnable()
    {
        GameManager.GameOverConfirmed += GameOverConfirmed;
        GameManager.GameStarted += GameStarted;
        LevelGenerator.TransitionDone += TransitionDone;
    }

    private void OnDisable()
    {
        GameManager.GameOverConfirmed -= GameOverConfirmed;
        GameManager.GameStarted -= GameStarted;
        LevelGenerator.TransitionDone -= TransitionDone;
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
    }

    private void FixedUpdate()
    {
        if (shouldAbsord)
        {
            Absorb();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Paddle")
        {
            canAbsorb = true;
        }
        if (collision.gameObject.tag == "Wall")
        {
            wallHit = true;
        }
        ContactPoint2D cp = collision.contacts[0]; // 0 indicates the first contact point between the colliders. Since there is only one contact point a higher index would cause a runtime error
        Vector2 reflectDir = Vector2.Reflect(ray.direction, cp.normal);

        float rotation = 90 + Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        codeSpeed = deflectionSpeed;
        rigidbody.velocity = -transform.up.normalized * codeSpeed;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canAbsorb)
        {
            if (collision.gameObject.layer == 8)
            {
                rigidbody.velocity = Vector2.zero;
                shouldAbsord = true;
                targetTransform = collision.transform;
            }

        }
        if (collision.gameObject.layer == 9)
        {
            rigidbody.velocity = Vector2.zero;
            this.transform.position = Vector2.right * 1000;
            PlayerMissed();
        }
    }

    void Absorb()
    {
        if (canAbsorb)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, targetTransform.position, Time.deltaTime * absorbSpeed);
            animator.SetTrigger("AtCenter");
            if (this.transform.position == targetTransform.position)
            {
                shouldAbsord = false;
            }
            if (!shouldAbsord)
            {
                if (wallHit)
                {
                    AbsorbDoneAndRichochet();
                    this.transform.position = Vector2.right * 1000;
                }
                else
                {
                    AbsorbDone();
                    this.transform.position = Vector2.right * 1000;
                }
            }
        }
    }

    void GameStarted()
    {
        this.transform.position = startPos;
        ballSpawner.transform.position = startPos;
        canAbsorb = false;
        animator.SetTrigger("GameStarted");
        spawnerAnimator.SetTrigger("GameStarted");
        StartCoroutine(SpawnDelay());
        wallHit = false;
    }

    void GameOverConfirmed()
    {
        this.transform.position = Vector2.right * 1000;
        codeSpeed = speed;
    }

    void TransitionDone()
    {
        animator.SetTrigger("NextLvl");
        spawnerAnimator.SetTrigger("GameStarted");
        animator.ResetTrigger("AtCenter");

        RandomXPos = new Vector2(target.RandomSpawnAreaXRange, startPos.y);
        ballSpawner.transform.position = RandomXPos;
        this.transform.position = RandomXPos;
        canAbsorb = false;
        StartCoroutine(SpawnDelay());
        wallHit = false;
        codeSpeed = speed;
    }
}
